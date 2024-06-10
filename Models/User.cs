using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace blog_API.Models;

public class User
{
    // [JsonIgnore]
    public int UserId { get; set; }
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; } = "USA";
    public DateTime? Created { get; set; } =  DateTime.Now;
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }

// [JsonIgnore]
    public List<Post>? Posts { get; set; }

// [JsonIgnore]

    public List<Comment>? Comments {get; set;}

}