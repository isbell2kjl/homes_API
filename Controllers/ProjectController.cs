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

    public ProjectController(ILogger<ProjectController> logger, IProjectRepository repository)
    {
        _logger = logger;
        _projectRepository = repository;
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
    public ActionResult<Project> GetProjectFirstRow()
    {
        var project = _projectRepository.GetProjectFirstRow();
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }

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

     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("{projectId:int}")]
    public ActionResult<Project> UpdatePost(Project project)
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

}