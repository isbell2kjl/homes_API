namespace blog_API.Models;


public class SignInResponse
{

    public int Id { get; set; }
    // public string? FirstName { get; set; }
    // public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Token { get; set; }

    public SignInResponse(User user, string token)
    {
        Id = user.UserId;
        // FirstName = user.FirstName;
        // LastName = user.LastName;
        UserName = user.UserName;
        Token = token;
    }
}