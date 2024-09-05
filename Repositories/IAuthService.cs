using blog_API.Models;

namespace blog_API.Repositories;

public interface IAuthService
{
    
    //ideas from https://jasonwatmore.com/ and https://code-maze.com/

    User SignUp(User user);
    SignInResponse SignIn(SignInRequest request);
    string GenerateRefreshToken();
    SignInResponse TokenRefresh(string token);
    void TokenRevoke(string token);
    
}