using System.ComponentModel.DataAnnotations;

namespace blog_API.Models;

public class ValidateResetTokenRequest
{
    [Required]
    public string? Token { get; set; }
}