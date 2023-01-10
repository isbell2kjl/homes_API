using blog_API.Models;

namespace blog_API.Repositories;

public interface IAuthService
{
    User SignUp(User user);
    //https://jasonwatmore.com/post/2021/12/14/net-6-jwt-authentication-tutorial-with-example-api#user-cs
    SignInResponse SignIn(SignInRequest request);
}