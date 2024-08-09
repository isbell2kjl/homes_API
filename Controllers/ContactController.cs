using blog_API.Models;
using blog_API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace blog_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly IContactRepository _contactRepository;

    public ContactController(ILogger<ContactController> logger, IContactRepository repository)
    {
        _logger = logger;
        _contactRepository = repository;
    }

    //Route to send contact information from contact form.
    [HttpPost]
    public IActionResult SendContact(Contact model)
    {
        _contactRepository.SendContact(model, Request.Headers["origin"]!);
        return Ok(new { message = "Thanks for your request.  We will get back to you soon" });

    }

}