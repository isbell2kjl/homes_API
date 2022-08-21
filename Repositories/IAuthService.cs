using blog_API.Models;

namespace blog_API.Repositories;

public interface IAuthService
{
    User CreateUser(User user);
    string SignIn(SignInRequest request);
}