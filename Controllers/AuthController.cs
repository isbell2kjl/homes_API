using homes_API.Models;
using homes_API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;


namespace homes_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly IForgotPasswordRepository _forgotPasswordRepository;
    private readonly IRecaptchaService _recaptchaService;

    public AuthController(ILogger<AuthController> logger, IAuthService service, IForgotPasswordRepository forgotPasswordRepository,
    IRecaptchaService recaptchaService)
    {
        _logger = logger;
        _authService = service;
        _forgotPasswordRepository = forgotPasswordRepository;
        _recaptchaService = recaptchaService;
    }

    [HttpPost]
    [Route("signup")]
    public ActionResult SignUp(User user)
    {
        if (user == null || !ModelState.IsValid)
        {
            return BadRequest();
        }
        _authService.SignUp(user);
        return NoContent();
    }
    [HttpPost]
    [Route("signin")]
    [EnableRateLimiting("LoginRateLimit")]
    public ActionResult<SignInResponse> SignIn(SignInRequest request)
    {
        var response = _authService.SignIn(request);
        setTokenCookie(response.RefreshToken);

        if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(new
        {
            userId = response.Id,
            username = response.UserName,
            token = response.Token
            // Other user details...
        });
    }

    [HttpPost("refresh-token")]
    public IActionResult RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = _authService.TokenRefresh(refreshToken);

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new { message = "Token is required" });

        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("revoke-token")]
    public IActionResult RevokeToken()
    {
        // Extract the refresh token from the HTTP-only cookie
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new { message = "Refresh token not found in cookies." });

        try
        {
            // Revoke the token in the database
            _authService.TokenRevoke(refreshToken);

            // Clear the refresh token cookie from the client
            clearTokenCookie();

            return Ok(new { message = "Token revoked successfully." });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    //Post route to request a token by email for password reset.
    [HttpPost]
    [Route("forgot-password")]
    public IActionResult ForgotPassword(ForgotPasswordRequest model)
    {
        _forgotPasswordRepository.ForgotPassword(model, Request.Headers["origin"]!);
        return Ok(new { message = "Please check your email for password reset instructions" });
    }

    //Post route to verify that provided token matches database and that 
    //expired time is later than current time.
    [HttpPost]
    [Route("validate-reset-token")]
    [EnableRateLimiting("LoginRateLimit")]
    public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
    {
        _forgotPasswordRepository.ValidateResetToken(model);
        return Ok(new { message = "Token is valid" });
    }

    //Post route to enter token and new password for reset.
    [HttpPost]
    [Route("reset-password")]
    public IActionResult ResetPassword(ResetPasswordRequest model)
    {
        _forgotPasswordRepository.ResetPassword(model);
        return Ok(new { message = "Password reset successful, you can now login" });

    }

    //helper methods

    private void setTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTime.Now.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    // This method clears the refresh token cookie
    private void clearTokenCookie()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTime.Now.AddSeconds(-1), // Set expiration to the past
        };

        Response.Cookies.Delete("refreshToken", cookieOptions);
    }

    [HttpPost]
    [Route("verify-recaptcha")]
    public async Task<IActionResult> VerifyRecaptcha(RecaptchaRequest request)
    {
        // _logger.LogInformation($"Received reCAPTCHA token: {request.Token}");  // Log received token

        var isValid = await _recaptchaService.VerifyRecaptcha(request.Token);
        // _logger.LogInformation($"reCAPTCHA validation result: {isValid}");

        if (!isValid)
        {
            // _logger.LogInformation("Invalid reCAPTCHA.");  // Log failure
            return BadRequest("Invalid reCAPTCHA.");
        }

        // _logger.LogInformation("reCAPTCHA verified successfully.");  // Log success
        // return Ok("reCAPTCHA verified successfully.");
        return Ok(new { message = "reCAPTCHA verified successfully." });
    }


}

