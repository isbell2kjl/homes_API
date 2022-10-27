using blog_API.Migrations;
using blog_API.Models;
using bcrypt = BCrypt.Net.BCrypt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace blog_API.Repositories;

public class AuthService : IAuthService
{
    private static PostDbContext? _context;
    private static IConfiguration? _config;

    public AuthService(PostDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public User CreateUser(User user)
    {
        // TODO: Hash Password
        var passwordHash = bcrypt.HashPassword(user.Password);
        user.Password = passwordHash;

        _context!.Add(user);
        _context.SaveChanges();
        return user;
    }

    private string BuildToken(User user)
    {
        var secret = _config.GetValue<String>("TokenSecret");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        // Create Signature using secret signing key
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // Create claims to add to JWT payload
        var claims = new Claim[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "")
        };

        // Create token
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: signingCredentials);

        // Encode token
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }


    public string SignIn(SignInRequest request)
    {

        var user = _context!.Users!.SingleOrDefault(x => x.UserName == request.UserName);
        var verified = false;




        if (user != null)
        {
            verified = bcrypt.Verify(request.Password, user.Password);
        }

        if (user == null || !verified)
        {
            return String.Empty;
        }

        // Create & return JWT
        return BuildToken(user)!;
    }

    // public IEnumerable<User> GetAllUsers()
    // {
    //     return _context!.Users!.ToList();
    // }
    //To avoid circular reference, combination of ideas from these websites:
    //https://khalidabuhakmeh.com/ef-core-and-aspnet-core-cycle-issue-and-solution
    //https://qawithexperts.com/article/asp.net/ways-to-fix-circular-reference-detected-error-in-entity-fram/63

    public IEnumerable<object> GetAllUsers()
    {
        return _context!
            .Users!
            .Include(u => u.Posts).
            Select(u => new
            {
                u.UserId,
                u.UserName,
                u.City,
                u.State,
                u.Country,
                u.Created,
                Posts = u.Posts!.Select(p => new{
                    p.PostId,
                    p.Content,
                    p.Posted
                })
            })
                .ToList();

    }
    public User? GetUserById(int userId)
    {
        return _context!.Users!.SingleOrDefault(c => c.UserId == userId);
    }

    public User? UpdateUser(User newUser)
    {
        var originalUser = _context!.Users!.Find(newUser.UserId);
        if (originalUser != null)
        {
            originalUser.UserName = newUser.UserName;
            originalUser.Password = newUser.Password;
            originalUser.Email = newUser.Email;
            originalUser.LastName = newUser.LastName;
            originalUser.FirstName = newUser.FirstName;
            originalUser.City = newUser.City;
            originalUser.State = newUser.State;
            originalUser.Country = newUser.Country;
            _context.SaveChanges();
        }
        return originalUser;
    }
}