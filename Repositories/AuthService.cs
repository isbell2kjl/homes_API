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
        // Hash Password
        
        var passwordHash = Bcrypt.HashPassword(user.Password);
        user.Password = passwordHash;

        _context!.Add(user);
        _context.SaveChanges();

        return user;
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
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "")
        };

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
            x.RefreshToken == token && x.RefreshTokenExpires > DateTime.UtcNow);
        if (user == null) throw new Exception("Invalid token");
        return user;
    }
    public SignInResponse TokenRefresh(string token)
    {

        var user = getUserByRefreshToken(token);

        if (user is null || user.RefreshTokenExpires <= DateTime.Now)
            throw new AppException("Invalid client request");

        var newAccessToken = BuildToken(user);
        var newRefreshToken = GenerateRefreshToken()!;
        user.RefreshToken = newRefreshToken;

        // save changes to db
        _context!.Users!.Update(user);
        _context.SaveChanges();

        return new SignInResponse(user, newAccessToken, newRefreshToken);
    }

    public void TokenRevoke(string token)
    {
        var user = getUserByRefreshToken(token);

        user.RefreshToken = null;
        //DateTime fields cannot be set to null.  Not sure of workaround.
        user.RefreshTokenExpires = DateTime.Now;

        // save changes to db
        _context!.Users!.Update(user);
        _context.SaveChanges();

    }

}