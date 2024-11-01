using System.ComponentModel.DataAnnotations;

namespace homes_API.Models;

public class User
{
    public int UserId { get; set; }
    [Required]
    public int ProjId_fk { get; set; }
    [Required]
    [MinLength(6)]
    public string? UserName { get; set; }
    [Required]
    [MinLength(6)]
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
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpires { get; set; } = DateTime.Now;
    public sbyte? Terms { get; set;} = 0;
    public sbyte? Privacy {get; set;} = 0;
    public sbyte? Role {get; set;} = 0;

    public List<Post>? Posts { get; set; }
    public List<Comment>? Comments {get; set;}
    public Project? Project {get; set;}

}