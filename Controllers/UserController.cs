using System.Security.Claims;
using homes_API.Models;
using homes_API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace homes_API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserController(ILogger<UserController> logger, IUserRepository repository)
    {
        _logger = logger;
        _userRepository = repository;
    }

    [HttpGet]
    [Route("project/{projectId:int}")]
    public ActionResult<IEnumerable<User>> GetUsers(int projectId)
    {
        return Ok(_userRepository.GetAllUsers(projectId));
    }

    //search idea from https://www.pragimtech.com/blog/blazor/search-in-asp.net-core-rest-api/
    [HttpGet]
    [Route("{search}")]
    public async Task<ActionResult<IEnumerable<User>>> Search([FromQuery] string name, [FromQuery] int projectId)
    {
        try
        {
            var result = await _userRepository.Search(name, projectId);

            if (result.Any())
            {
                return Ok(result);
            }
        }
        catch (Exception)
        {
            return NotFound();
        }
        return StatusCode(StatusCodes.Status500InternalServerError,
        "Error retrieving data from the database");

    }

    [HttpGet]
    [Route("check-username")]
    public async Task<IActionResult> CheckUserName(string username)
        {
            if (await _userRepository.UserNameExists(username))
            {
            return BadRequest("Username is already taken.");
            }
            return Ok();
        }

    [HttpGet]
    [Route("check-email")]
    public async Task<IActionResult> CheckEmail(string email)
        {
            if (await _userRepository.EmailExists(email))
            {
            return BadRequest("Email is already in use.");
            }
            return Ok();
        }
        

    [HttpGet]
    [Route("{userId:int}")]
    public ActionResult<User> GetUserById(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // GET / get current user
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("current")]

    public ActionResult<User> GetCurrentUser()
    {

        if (HttpContext.User == null)
        {
            return Unauthorized();
        }

        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

        
        var userId = Int32.Parse(userIdClaim!.Value);

        var user = _userRepository.GetUserById(userId);
        

        return Ok(user);

        // int id = Convert.ToInt32(HttpContext.User.FindFirstValue("UserId"));


        // return Ok(new {UserId = id});
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("{userId:int}")]
    public IActionResult UpdateUser(int userId, UpdateRequest editUser)
    {
        //Make sure no can make changes who is NOT logged in.
        if (HttpContext.User == null)
        {
            return Unauthorized("Unable to find user, returns null");
        }
        // // Make sure no one can edit another user's profile.
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")!;

        var claimId = Int32.Parse(userIdClaim.Value);

        if (!ModelState.IsValid || editUser == null)
        {
            return BadRequest();
        }
        if (HttpContext.User == null)
        {
            return Unauthorized("Unable to find user, returns null");
        }
        if (claimId == userId)
        {
        _userRepository.UpdateUser(userId, editUser);
        return Ok(new { message = "User updated" });
        }
        else
        {
            return Unauthorized("Not current user, can't edit");
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    [Route("{userId:int}")]
    public ActionResult DeleteUser(int userId)
    {
        _userRepository.DeleteUserById(userId);
        return NoContent();
    }
}
