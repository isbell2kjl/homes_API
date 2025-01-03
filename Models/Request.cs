using System.ComponentModel.DataAnnotations;

namespace homes_API.Models;

public class Request
{
    [Key]
    public int RequestId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Possible values: 'Pending', 'Approved', 'Rejected'

    [Required]
    public DateTime RequestedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public User User { get; set; }

    public Project Project { get; set; }

}