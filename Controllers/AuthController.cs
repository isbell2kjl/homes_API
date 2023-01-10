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

    public AuthController(ILogger<AuthController> logger, IAuthService service)
    {
        _logger = logger;
        _authService = service;
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
    // public ActionResult<string> SignIn(string email, string password)
    public ActionResult SignIn(SignInRequest request)
    {
        var response = _authService.SignIn(request);

         if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });
        
        return Ok(response);

    }
    
}

