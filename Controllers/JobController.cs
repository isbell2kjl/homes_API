using homes_API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace homes_API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class JobController : ControllerBase
{
    private readonly ILogger<JobController> _logger;
    private readonly IJobRepository _jobRepository;

    public JobController(ILogger<JobController> logger, IJobRepository repository)
    {
        _logger = logger;
        _jobRepository = repository;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public ActionResult<IEnumerable<Models.Job>> GetJobs()
    {
        return Ok(_jobRepository.GetAllJobs());
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("{jobId:int}")]
    public ActionResult<Models.Job> GetJobById(int jobId)
    {
        var job = _jobRepository.GetJobById(jobId);
        if (job == null)
        {
            return NotFound();
        }
        return Ok(job);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public ActionResult<Models.Job> CreateJob(Models.Job job)
    {
        if (!ModelState.IsValid || job == null)
        {
            return BadRequest();
        }
        var newJob = _jobRepository.CreateJob(job);
        return Created(nameof(GetJobById), newJob);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("{JobId:int}")]
    public ActionResult<Models.Job> UpdateJob(Models.Job job)
    {
        if (!ModelState.IsValid || job == null)
        {
            return BadRequest();
        }
        return Ok(_jobRepository.UpdateJob(job));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    [Route("{jobId:int}")]
    public ActionResult DeleteJob(int jobId)
    {
        _jobRepository.DeleteJobById(jobId);
        return NoContent();
    }

}