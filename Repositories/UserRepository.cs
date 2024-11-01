using homes_API.Migrations;
using homes_API.Models;
using Microsoft.EntityFrameworkCore;

namespace homes_API.Repositories;

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


    //To avoid circular reference, combination of ideas from these websites:
    //https://khalidabuhakmeh.com/ef-core-and-aspnet-core-cycle-issue-and-solution
    //https://qawithexperts.com/article/asp.net/ways-to-fix-circular-reference-detected-error-in-entity-fram/63

    public IEnumerable<object> GetAllUsers()
    {
        return _context!
            .Users!
            .Include(u => u.Posts)
             

            .Select(u => new
            {
                u.UserId,
                u.UserName,
                u.Email,
                u.FirstName,
                u.LastName,
                u.City,
                u.State,
                u.Country,
                u.Created,
                u.ProjId_fk,

                Posts = u.Posts!.Select(p => new
                {
                    p.PostId,
                    p.Content,
                    p.Posted,
                    p.PhotoURL,
                    p.Visible,
                    p.UserId_fk,
                }),


            })
                .ToList();

    }
    public User GetUserById(int userId)
    {
        return _context!.Users!.SingleOrDefault(c => c.UserId == userId)!;
    }


//search idea from https://www.pragimtech.com/blog/blazor/search-in-asp.net-core-rest-api/
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

    public async Task<bool> UserNameExists(string username)
    { 
        
         return await _context!.Users!.AnyAsync(u => u.UserName == username)!;

    }

    public async Task<bool> EmailExists(string email)
    { 
        
         return await _context!.Users!.AnyAsync(u => u.Email == email)!;

    }

    public void UpdateUser(int userId, UpdateRequest editUser)
    {
        var originalUser = GetUserById(userId)!;

        //password can only be updated via the ForgotPassword route.

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