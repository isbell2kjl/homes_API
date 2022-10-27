using blog_API.Models;

namespace blog_API.Repositories;

public interface IAuthService
{
    User CreateUser(User user);
    string SignIn(SignInRequest request);
    //  IEnumerable<User> GetAllUsers();
    IEnumerable<object> GetAllUsers();
    User? GetUserById(int userId);
    User? UpdateUser(User newUser);
}