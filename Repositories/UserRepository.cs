using blog_API.Migrations;
using blog_API.Models;
using bcrypt = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;

namespace blog_API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostDbContext _context;

    public UserRepository(PostDbContext context)
    {
        _context = context;
    }

    public void DeleteUserById(int userId)
    {
        var user = _context.Users!.Find(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }

    public IEnumerable<User> GetAllUsers()
    {

        return _context.Users!.ToList();

    }

    //To avoid circular reference, combination of ideas from these websites:
    //https://khalidabuhakmeh.com/ef-core-and-aspnet-core-cycle-issue-and-solution
    //https://qawithexperts.com/article/asp.net/ways-to-fix-circular-reference-detected-error-in-entity-fram/63

    // public IEnumerable<object> GetAllUsers()
    // {
    //     return _context!
    //         .Users!
    //         .Include(u => u.Posts)

    //         .Select(u => new
    //         {
    //             u.UserId,
    //             u.UserName,
    //             u.Email,
    //             u.FirstName,
    //             u.LastName,
    //             u.City,
    //             u.State,
    //             u.Country,
    //             u.Created,

    //             Posts = u.Posts!.Select(p => new
    //             {
    //                 p.PostId,
    //                 p.Content,
    //                 p.Posted,
    //                 p.UserId_fk
    //             }),


    //         })
    //             .ToList();

    // }
    public User? GetUserById(int userId)
    {
        return _context!.Users!.SingleOrDefault(c => c.UserId == userId);
    }

    public async Task<IEnumerable<User>> Search(string name)
    {
        IQueryable<User> query = _context.Users!;

        if (!string.IsNullOrEmpty(name))
        {
            query = query!.Where(u => u.FirstName!.Contains(name)
                    || u.LastName!.Contains(name));
        }

        return await query.ToListAsync();

    }

    public void UpdateUser(int userId, UpdateRequest editUser)
    {
        var originalUser = GetUserById(userId)!;

        //not working right.  To change password in Postman, temporarily enable the following:
        //  However this won't work on the frontend.

        // hash password if it was entered
        // if (!string.IsNullOrEmpty(editUser.Password))
        // {
        //     originalUser.Password = bcrypt.HashPassword(editUser.Password);
        // }

        // copy model to user and save

        originalUser.UserName = editUser.UserName;
        originalUser.Email = editUser.Email;
        originalUser.FirstName = editUser.FirstName;
        originalUser.LastName = editUser.LastName;
        originalUser.City = editUser.City;
        originalUser.State = editUser.State;
        originalUser.Country = editUser.Country;

        _context.Users!.Update(originalUser);
        _context.SaveChanges();

    }
}