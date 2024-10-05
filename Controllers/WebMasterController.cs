using homes_API.Models;
using homes_API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace homes_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebMasterController : ControllerBase
{
    private readonly ILogger<WebMasterController> _logger;
    private readonly IWebMasterRepository _webMasterRepository;

    public WebMasterController(ILogger<WebMasterController> logger, IWebMasterRepository repository)
    {
        _logger = logger;
        _webMasterRepository = repository;
    }

    //Route to send contact information from contact form.
    [HttpPost]
    public IActionResult SendWebMaster(Contact model)
    {
        _webMasterRepository.SendWebMaster(model, Request.Headers["origin"]!);
        return Ok(new { message = "Thanks for your request.  We will get back to you soon" });

    }

}