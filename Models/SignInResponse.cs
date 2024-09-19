using System.Text.Json.Serialization;

namespace homes_API.Models;


public class SignInResponse
{

    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Token { get; set; }
    [JsonIgnore]
    public string RefreshToken { get; set;}

    public SignInResponse(User user, string token, string refreshToken)
    {
        Id = user.UserId;
        UserName = user.UserName;
        Token = token;
        RefreshToken = refreshToken;
    }
}