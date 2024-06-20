using System.ComponentModel.DataAnnotations;

namespace blog_API.Models;

public class Post
{
    [Required]
    public int PostId { get; set; }
    public string? Title { get; set; }
    public string? PhotoURL { get; set; }
    [Required]
    public string? Content { get; set; }
    public DateTime? Posted { get; set; } =  DateTime.Now;
    public sbyte? Visible { get; set;} = 0;

    public int UserId_fk { get; set; }
    public User? User { get; set; }
    public List<Comment>? Comments { get; set; }
}