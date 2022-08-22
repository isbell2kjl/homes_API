using blog_API.Models;
using blog_API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace blog_API.Controllers;

[ApiController]
[Route("[controller]")]
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
    public ActionResult CreateUser(User user)
    {
        if (user == null || !ModelState.IsValid)
        {
            return BadRequest();
        }
        _authService.CreateUser(user);
        return NoContent();
    }
    [HttpPost]
    [Route("signin")]
    // public ActionResult<string> SignIn(string email, string password)
    public ActionResult<string> SignIn(SignInRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest();
        }

        var token = _authService.SignIn(request);



        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        return Ok(token);
    }
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(_authService.GetAllUsers());
    }

     [HttpGet]
    [Route("{userId:int}")]
    public ActionResult<User> GetUserById(int userId)
    {
        var user = _authService.GetUserById(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut]
    [Route("{userId:int}")]
    public ActionResult<User> UpdateUser(User user)
    {
        if (!ModelState.IsValid || user == null)
        {
            return BadRequest();
        }
        return Ok(_authService.UpdateUser(user));
    }
}