using System.ComponentModel.DataAnnotations;

namespace homes_API.Models;

public class Comment
{
    [Required]
    public int ComId { get; set; }
    [Required]
    public string? Text { get; set; }
    public DateTime? ComDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int PostId_fk { get; set; }
    public int UsrId_fk { get; set; }
    public Post? Post { get; set; }
    public User? User { get; set; }


}