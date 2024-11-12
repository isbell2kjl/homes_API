using homes_API.Models;
using homes_API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace homes_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentRepository _commentRepository;

    public CommentController(ILogger<CommentController> logger, ICommentRepository repository)
    {
        _logger = logger;
        _commentRepository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<object>> GetComments()
    {
        return Ok(_commentRepository.GetAllComments());
    }

    [HttpGet]
    [Route("postComment/{postId:int}")]
    public ActionResult<IEnumerable<object>> GetPComments(int postId)
    {
        return Ok(_commentRepository.GetPostComments(postId));
    }

    [HttpGet]
    [Route("{comId:int}")]
    public ActionResult<Comment> GetCommentById(int comId)
    {
        var comment = _commentRepository.GetCommentById(comId);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public ActionResult<Comment> CreateComment(Comment comment)
    {
        if (!ModelState.IsValid || comment == null)
        {
            return BadRequest();
        }
        var result = _commentRepository.CreateComment(comment);
        return Created(nameof(GetCommentById), result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("{comId:int}")]
    public ActionResult<Comment> UpdateComment(int comId, CommentUpdate editComment)
    {
        if (!ModelState.IsValid || editComment == null)
        {
            return BadRequest();
        }
        return Ok(_commentRepository.UpdateComment(comId, editComment));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    [Route("{comId:int}")]
    public ActionResult DeleteComment(int comId)
    {
        _commentRepository.DeleteCommentById(comId);
        return NoContent();
    }

}