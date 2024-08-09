using blog_API.Models;

namespace blog_API.Repositories;

public interface IForgotPasswordRepository
{
    void ForgotPassword(ForgotPasswordRequest model, string origin);
    void ValidateResetToken(ValidateResetTokenRequest model);
    void ResetPassword(ResetPasswordRequest model);

}