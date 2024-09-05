 using blog_API.Models;
using blog_API.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace blog_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly IForgotPasswordRepository _forgotPasswordRepository;
    private readonly IEmailRepository _emailRepository;

    public AuthController(ILogger<AuthController> logger, IAuthService service, IForgotPasswordRepository forgotPasswordRepository,
    IEmailRepository emailRepository)
    {
        _logger = logger;
        _authService = service;
         _forgotPasswordRepository = forgotPasswordRepository;
         _emailRepository = emailRepository;
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
    public ActionResult<SignInResponse> SignIn(SignInRequest request)
    {
        var response = _authService.SignIn(request);
        setTokenCookie(response.RefreshToken);

         if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });
        
        return Ok(response);

    }

    [HttpPost("refresh-token")]
    public ActionResult<SignInResponse> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = _authService.TokenRefresh(refreshToken);

        setTokenCookie(response.RefreshToken);
        
        return Ok(response);
    }


[HttpPost("revoke-token")]
    public IActionResult RevokeToken(RevokeTokenRequest model)
    {
        // accept token from request body or cookie
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });


        _authService.TokenRevoke(token);
        return Ok(new { message = "Token revoked" });
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
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
    
}

