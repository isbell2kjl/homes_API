using homes_API.Models;

namespace homes_API.Repositories;

public interface IUserRepository
{

    // IEnumerable<User> GetAllUsers();
    IEnumerable<object> GetAllUsers();
    Task<IEnumerable<User>> Search(string name);
    User GetUserById(int userId);
    void UpdateUser(int userId, UpdateRequest editUser);
    void DeleteUserById(int userId);

}