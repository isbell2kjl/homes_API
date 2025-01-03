using System.ComponentModel.DataAnnotations;

namespace homes_API.Models; 
public class JoinRequest
{
    [Required]
    [EmailAddress]
    public string ProjectEmail { get; set; }
}
