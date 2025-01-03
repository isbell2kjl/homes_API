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

    //modified 11/12/24 go filter by project.
    public IEnumerable<object> GetAllUsers(int projectId)
    {
        return _context!
            .Users!
             .Where(u => u!.ProjId_fk == projectId)
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

    public IEnumerable<User> GetAdminUsers()
    {
        // Fetch all users with role 1 (admin)
        return _context.Users.Where(c => c.Role == 1).ToList();
    }


    //search idea from https://www.pragimtech.com/blog/blazor/search-in-asp.net-core-rest-api/
    public async Task<IEnumerable<User>> Search(string name, int projectId)
    {
        IQueryable<User> query = _context.Users!;

        // Filter by projectId (required)
        query = query.Where(p => p.ProjId_fk == projectId);

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

    public async Task<bool> AssignProjectToUser(int projectId, string email)
    {
        // Find the non-logged-in user by email
        var nonLoggedInUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (nonLoggedInUser == null)
        {
            throw new ArgumentException("User with the given email does not exist.");
        }

        // Find the property
        var project = await _context.Users.SingleOrDefaultAsync(p => p.ProjId_fk == projectId);
        if (project == null)
        {
            throw new ArgumentException("Project not found.");
        }

        // Assign the property to the non-logged-in user
        project.ProjId_fk = nonLoggedInUser.ProjId_fk;

        // Save changes to the database
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> EmailExists(string email)
    {

        return await _context!.Users!.AnyAsync(u => u.Email == email)!;

    }

    public void UpdateUser(int userId, UpdateRequest editUser)
    {
        var originalUser = GetUserById(userId)!;

        //password can only be updated via the ForgotPassword route.

        // Update only if new values are provided
        if (!string.IsNullOrEmpty(editUser.UserName)) originalUser.UserName = editUser.UserName;
        if (!string.IsNullOrEmpty(editUser.Email)) originalUser.Email = editUser.Email;
        if (!string.IsNullOrEmpty(editUser.FirstName)) originalUser.FirstName = editUser.FirstName;
        if (!string.IsNullOrEmpty(editUser.LastName)) originalUser.LastName = editUser.LastName;
        if (!string.IsNullOrEmpty(editUser.City)) originalUser.City = editUser.City;
        if (!string.IsNullOrEmpty(editUser.State)) originalUser.State = editUser.State;
        if (!string.IsNullOrEmpty(editUser.Country)) originalUser.Country = editUser.Country;

        // Conditionally update ProjIdFk and Role
        if (editUser.ProjId_fk.HasValue) originalUser.ProjId_fk = editUser.ProjId_fk.Value;
        if (editUser.Role.HasValue) originalUser.Role = editUser.Role.Value;


        _context.Users!.Update(originalUser);
        _context.SaveChanges();

    }

}