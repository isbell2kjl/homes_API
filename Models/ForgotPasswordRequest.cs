using System.ComponentModel.DataAnnotations;

namespace blog_API.Models; 

public class ForgotPasswordRequest{
    [Required]
    [EmailAddress]

    public string? Email {get ; set;}
}
