using homes_API.Models;

namespace homes_API.Repositories;

public interface IUserRepository
{

    // IEnumerable<User> GetAllUsers();
    IEnumerable<object> GetAllUsers(int projectId);
    Task<IEnumerable<User>> Search(string name, int projectId);
    Task<bool> UserNameExists(string username);
    Task<bool> EmailExists(string email);
    User GetUserById(int userId);
    void UpdateUser(int userId, UpdateRequest editUser);
    void DeleteUserById(int userId);

}