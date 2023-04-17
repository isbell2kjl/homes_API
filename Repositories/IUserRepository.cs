using blog_API.Models;

namespace blog_API.Repositories;

public interface IUserRepository
{

    IEnumerable<User> GetAllUsers();
    Task<IEnumerable<User>> Search(string name);
    User? GetUserById(int userId);
    void UpdateUser(int userId, UpdateRequest editUser);
    void DeleteUserById(int userId);

}