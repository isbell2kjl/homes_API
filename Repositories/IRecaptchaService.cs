namespace homes_API.Repositories;

public interface IRecaptchaService
{
    Task<bool> VerifyRecaptcha(string token);
}