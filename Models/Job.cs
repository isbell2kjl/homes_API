using System.ComponentModel.DataAnnotations;

namespace homes_API.Models;

public class Job
{
    [Required]
    public int JobId { get; set; }
    [Required]
    public string? Title { get; set; }
    public bool Completed { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int UserId_fk { get; set; }
    public User? User { get; set; }

}