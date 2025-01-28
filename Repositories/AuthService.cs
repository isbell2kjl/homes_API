using homes_API.Migrations;
using homes_API.Models;
using homes_API.Helpers;
using Bcrypt = BCrypt.Net.BCrypt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;


namespace homes_API.Repositories;


public class AuthService : IAuthService
{
    private static PostDbContext? _context;
    private static IConfiguration? _config;
    private readonly IForgotPasswordRepository _forgotPasswordRepository;
    private readonly IEmailRepository _emailRepository;

    public AuthService(PostDbContext context, IConfiguration config, IForgotPasswordRepository forgotPasswordRepository,
    IEmailRepository emailRepository)
    {
        _context = context;
        _config = config;
        _emailRepository = emailRepository;
        _forgotPasswordRepository = forgotPasswordRepository;
     
    }


    public User SignUp(User user)
    {
        // Generate a random temporary password
        var tempPassword = GenerateRandomPassword();
        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

        user.Password = passwordHash;

        // Save user to the database
        _context!.Add(user);
        _context.SaveChanges();

        // Replace the hashed password with the plain text password
        // to return it to the caller (e.g., for email purposes)
        user.Password = tempPassword;

        return user;
    }

    private string GenerateRandomPassword(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }


    private string BuildToken(User user)
    {
        var secret = _config.GetValue<String>("TokenSecret");
        var issuer = _config.GetValue<String>("Issuer");
        
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        // Create Signature using secret signing key
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // Create claims to add to JWT payload
        var claims = new Claim[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim("UserId", user.UserId.ToString()),
        new Claim("Role", user.Role.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "")
        };

        // Debugging: Log claims
        // Console.WriteLine("JWT Claims:");
        // foreach (var claim in claims)
        // {
        //     Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        // }

        // Create token
        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: signingCredentials);

        // Encode token
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }

    public string GenerateRefreshToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var tokenIsUnique = !_context!.Users!.Any(x => x.RefreshToken == token);
        if (!tokenIsUnique)
            return GenerateRefreshToken();

        return token;

    }

    //modified SignIn from tutorial:
    // //https://jasonwatmore.com/post/2021/12/14/net-6-jwt-authentication-tutorial-with-example-api#user-cs
    public SignInResponse SignIn(SignInRequest request)
    {

        var user = _context!.Users!.SingleOrDefault(x => x.UserName == request.UserName);
        var verified = false;

        if (user != null)
        {
            verified = Bcrypt.Verify(request.Password, user.Password);
        }

        if (user == null || !verified)
        {
            return null!;
        }

        // Create & return JWT
        var token = BuildToken(user)!;
        var refreshToken = GenerateRefreshToken()!;

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = DateTime.Now.AddDays(7);

        // save changes to db
        _context.Users!.Update(user);
        _context.SaveChanges();

        return new SignInResponse(user, token, refreshToken);
    }

    private static User getUserByRefreshToken(string token)
    {
        var user = _context!.Users!.SingleOrDefault(x =>
            x.RefreshToken == token && x.RefreshTokenExpires > DateTime.Now);
        if (user == null) throw new AppException("Invalid token");

        // Console.WriteLine($"Found user: {user.UserName} with token: {user.RefreshToken}");

        return user;
    }
    public SignInResponse TokenRefresh(string token)
    {

        var user = getUserByRefreshToken(token);

        if (user is null || user.RefreshTokenExpires <= DateTime.Now)
            throw new AppException("Invalid client request. Please sign in again.");

        var newAccessToken = BuildToken(user);

        // // Only issue a new refresh token if it's near expiry
        // if (user.RefreshTokenExpires - DateTime.Now <= TimeSpan.FromMinutes(30))
        // {
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpires = DateTime.Now.AddDays(7);

        // Save changes to DB
        _context.Users.Update(user);
        _context.SaveChanges();

        return new SignInResponse(user, newAccessToken, newRefreshToken);
        // }

        // Return without issuing a new refresh token
        // return new SignInResponse(user, newAccessToken, user.RefreshToken);
    }

    public void TokenRevoke(string token)
    {
        var user = getUserByRefreshToken(token);

        if (user == null) throw new AppException("Invalid token");

        // Remove refresh token from the database
        user.RefreshToken = null;
        user.RefreshTokenExpires = DateTime.Now.AddSeconds(-1);

        // Save changes to the database
        _context!.Users!.Update(user);
        _context.SaveChanges();

    }

}