using homes_API.Models;
using homes_API.Repositories;
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

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProjectContent(int projectId)
    {
        var content = await _projectRepository.GetPojectAsync(projectId)!;
        if(content == null)
        {
            return NotFound();
        }
        return Ok(content);
    }

    [HttpPut("projectId")]
    public async Task<IActionResult> UpdateProjectContent(int projectId, [FromBody] Project updatedProject)
    {
        if (projectId != updatedProject.ProjectId)
        {
            return BadRequest("Project ID mismatch");
        }
        var existingContent = await _projectRepository.GetPojectAsync(projectId)!;
        if (existingContent == null)
        {
            return NotFound();
        }

        await _projectRepository.UpdateProjectAsync(updatedProject)!;
        return NoContent();
    }
}