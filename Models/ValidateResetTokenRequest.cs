using System.ComponentModel.DataAnnotations;

namespace homes_API.Models;

public class ValidateResetTokenRequest
{
    [Required]
    public string? Token { get; set; }
}