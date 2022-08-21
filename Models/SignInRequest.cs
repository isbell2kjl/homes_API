using System.ComponentModel.DataAnnotations;

namespace blog_API.Models;

public class SignInRequest 
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}