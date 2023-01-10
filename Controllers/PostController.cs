using blog_API.Models;
using blog_API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blog_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _postRepository;

    public PostController(ILogger<PostController> logger, IPostRepository repository)
    {
        _logger = logger;
        _postRepository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<object>> GetPosts()
    {
        return Ok(_postRepository.GetAllPosts());
    }

    [HttpGet]
    [Route("userPost/{userId:int}")]
    public ActionResult<IEnumerable<object>> GetUPosts(int userId)
    {
        return Ok(_postRepository.GetUserPosts(userId));
    }

    [HttpGet]
    [Route("{postId:int}")]
    public ActionResult<Post> GetPostById(int postId)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public ActionResult<Post> CreatePost(Post post)
    {
        if (!ModelState.IsValid || post == null)
        {
            return BadRequest();
        }
        var result = _postRepository.CreatePost(post);
        return Created(nameof(GetPostById), result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("{postId:int}")]
    public ActionResult<Post> UpdatePost(Post post)
    {
        if (!ModelState.IsValid || post == null)
        {
            return BadRequest();
        }
        return Ok(_postRepository.UpdatePost(post));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    [Route("{postId:int}")]
    public ActionResult DeletePost(int postId)
    {
        _postRepository.DeletePostById(postId);
        return NoContent();
    }

}