using System.ComponentModel.DataAnnotations;

namespace homes_API.Models; 

public class ForgotPasswordRequest{
    [Required]
    [EmailAddress]

    public string? Email {get ; set;}
}
