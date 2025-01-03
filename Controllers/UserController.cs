
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
    private readonly IProjectRepository _projectRepository;

    public UserController(ILogger<UserController> logger, IUserRepository repository, IProjectRepository projectrepository)
    {
        _logger = logger;
        _userRepository = repository;
        _projectRepository = projectrepository;

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("project/{projectId:int}")]
    public ActionResult<IEnumerable<User>> GetUsers(int projectId)
    {
        return Ok(_userRepository.GetAllUsers(projectId));
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("admin-users")]
    public ActionResult<IEnumerable<User>> GetAdminUsers()
    {
        return Ok(_userRepository.GetAdminUsers());
    }

    //search idea from https://www.pragimtech.com/blog/blazor/search-in-asp.net-core-rest-api/
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    [HttpPost]
    [Route("request-to-join")]
    public async Task<IActionResult> RequestToJoinProject([FromBody] JoinRequest joinRequest)
    {
        try
        {
            int userId = GetCurrentUserId();
            var result = await _projectRepository.RequestToJoinProject(userId, joinRequest.ProjectEmail);

            if (result.Success)
                return Ok(new { message = result.Message });

            return BadRequest(new { error = result.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    // GET api/project/pending-requests

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("pending-requests/{adminProjectId}")]
    public async Task<IActionResult> GetPendingRequests(int adminProjectId)
    {
        var pendingRequests = await _projectRepository.GetPendingRequests(adminProjectId);
        if (pendingRequests == null || pendingRequests.Count == 0)
        {
            return Ok(new { hasPendingRequests = false });
        }
        return Ok(new { hasPendingRequests = true, requests = pendingRequests });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("user-requests/{userId}")]

    public async Task<IActionResult> GetUserRequests(int userId)
    {
        // var currentUser = RetrieveCurrentUser();
        var userRequests = await _projectRepository.GetUserRequests(userId);
        if (userRequests == null || userRequests.Count == 0)
        {
            return Ok(new { hasUserRequests = false });
        }
        return Ok(new { hasUserRequests = true, requests = userRequests });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("approve-request/{requestId}")]
    public async Task<IActionResult> ApproveJoinRequest(int requestId)
    {
        var result = await _projectRepository.ApproveJoinRequest(requestId);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("reject-request/{requestId}")]
    public async Task<IActionResult> RejectJoinRequest(int requestId)
    {
        var result = await _projectRepository.RejectJoinRequest(requestId);

        if (!result.Success)
        {
            return NotFound(new { Message = result.Message });
        }

        return Ok(new { Message = result.Message });
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


        var userId = Int32.Parse(userIdClaim.Value);

        var user = _userRepository.GetUserById(userId);


        return Ok(user);

        // int id = Convert.ToInt32(HttpContext.User.FindFirstValue("UserId"));


        // return Ok(new {UserId = id});
    }


    // Private helper method to get the current userId
    private int GetCurrentUserId()
    {
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found.");

        return int.Parse(userIdClaim.Value);
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("{userId:int}")]
    public IActionResult UpdateUser(int userId, UpdateRequest editUser)
    {
        // Make sure the user is logged in
        if (HttpContext.User == null)
        {
            return Unauthorized("Unable to find user, returns null");
        }

        // Get the 'role' and 'UserId' claims
        var currentRoleClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Role");
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

        // Check if the claims are missing
        if (currentRoleClaim == null)
        {
            return Unauthorized("Role claim is missing");
        }

        if (userIdClaim == null)
        {
            return Unauthorized("UserId claim is missing");
        }

        // Parse the claims
        int claimId;
        sbyte roleId;

        try
        {
            claimId = Int32.Parse(userIdClaim.Value);
            roleId = (sbyte)Int32.Parse(currentRoleClaim.Value);
        }
        catch (FormatException ex)
        {
            return BadRequest($"Invalid claim value. Error: {ex.Message}");
        }

        // Validate the model state
        if (!ModelState.IsValid || editUser == null)
        {
            return BadRequest("Invalid data.");
        }

        // Ensure the user is either updating their own profile or is an admin
        if (claimId == userId || roleId == 1)
        {
            // Perform the update
            _userRepository.UpdateUser(userId, editUser);
            return Ok(new { message = "User updated successfully" });
        }
        else
        {
            return Unauthorized("Not the current user or no permission to edit this profile.");
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
