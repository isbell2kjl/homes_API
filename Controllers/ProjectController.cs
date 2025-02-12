using homes_API.Models;
using homes_API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly ILogger<ProjectController> _logger;
    private readonly IProjectRepository _projectRepository;
    private readonly IQueryService _queryService;

    public ProjectController(ILogger<ProjectController> logger, IProjectRepository repository, IQueryService queryService)
    {
        _logger = logger;
        _projectRepository = repository;
        _queryService = queryService;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("{projectId:int}")]
    public ActionResult<Project> GetProjectById(int projectId)
    {
        var project = _projectRepository.GetProjectById(projectId);
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }

    [HttpGet]
    //this is the base route /api/project for the home page in angular.
    public ActionResult<Project> GetProjectFirstRow()
    {
        var project = _projectRepository.GetProjectFirstRow();
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public ActionResult<Project> CreateProject(Project project)
    {
        if (!ModelState.IsValid || project == null)
        {
            return BadRequest();
        }
        var result = _projectRepository.CreateProject(project);
        return Created(nameof(GetProjectById), result);
    }

    [HttpGet]
    [Route("check-email")]
    public async Task<IActionResult> CheckEmail(string email)
    {
        if (!await _projectRepository.EmailExists(email))
        {
            return BadRequest("Email does not exist.");
        }
        return Ok();
    }

    [HttpGet]
    [Route("check-email-true")]
    public async Task<IActionResult> CheckEmailTrue(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email parameter is required.");
        }

        if (await _projectRepository.EmailExists(email))
        {
            return BadRequest("Email is already in use.");
        }

        return Ok();
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("{projectId:int}")]
    public ActionResult<Project> UpdateProject(Project project)
    {
        if (!ModelState.IsValid || project == null)
        {
            return BadRequest();
        }
        return Ok(_projectRepository.UpdateProject(project));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    [Route("{projectId:int}")]
    public ActionResult DeleteProject(int projectId)
    {
        _projectRepository.DeleteProjectById(projectId);
        return NoContent();
    }

    // [HttpGet("test-send-email/{projectId}")]
    // public async Task<IActionResult> TestSendEmail(int projectId)
    // {
    //     await _queryService.GetQueryResultProperties(projectId);
    //     return Ok("Email test completed.");
    // }

    //  [HttpGet("test-send-email-details/{projectId}")]
    // public async Task<IActionResult> TestSendEmailDetails(int projectId)
    // {
    //     await _queryService.GetQueryResultTasks(projectId);
    //     return Ok("Email test completed.");
    // }

    [HttpGet("send-weekly-reports/{projectId}")]
    public async Task<IActionResult> QueryAndSendReports(int projectId)
    {
        await _queryService.QueryAndSendReports(projectId);
        return Ok(new { message = "Email successfully sent." });
    }

}