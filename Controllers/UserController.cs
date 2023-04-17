using blog_API.Models;
using blog_API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blog_API.Controllers;

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
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(_userRepository.GetAllUsers());
    }

//search idea from https://www.pragimtech.com/blog/blazor/search-in-asp.net-core-rest-api/
    [HttpGet]
    [Route("{search}")]
    public async Task<ActionResult<IEnumerable<User>>> Search(string name)
    {
        try
        {
            var result = await (_userRepository.Search(name));

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
        // var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")!;

        // var claimId = Int32.Parse(userIdClaim.Value);

        if (!ModelState.IsValid || editUser == null)
        {
            return BadRequest();
        }
        if (HttpContext.User == null)
        {
            return Unauthorized("Unable to find user, returns null");
        }
        // if (claimId == userId)
        // {
        _userRepository.UpdateUser(userId, editUser);
        return Ok(new { message = "User updated" });
        // }
        // else
        // {
        //     return Unauthorized("Not current user, can't edit");
        // }
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
