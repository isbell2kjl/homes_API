using homes_API.Models;
using homes_API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MailKit.Net.Smtp;


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

        if (response == null)
        {
            //allow Fail2Ban on webserver to detect user and ip address of failed logins.
            var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogWarning("LOGIN FAILED from IP {IP} for username {Username}", ip, request.UserName);


            return Unauthorized(new { message = "Invalid login" });
        }

        setTokenCookie(response.RefreshToken);

        return Ok(new
        {
            userId = response.Id,
            username = response.UserName,
            token = response.Token
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
        try
        {
            _forgotPasswordRepository.ForgotPassword(model, Request.Headers["origin"]!);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }
        catch (FormatException ex)
        {
            // This often happens if the email address format is incorrect for MimeKit
            _logger.LogError(ex, "Invalid email format.");
            return BadRequest(new { message = "The email address format is invalid." });
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP command failed during password reset email.");
            return StatusCode(500, new { message = "Email sending failed due to server error. Please try again later." });
        }
        catch (Exception ex)
        {
            // General fallback
            _logger.LogError(ex, "An unexpected error occurred. Please try again later");
            return StatusCode(500, new { message = ex.Message });
        }
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

