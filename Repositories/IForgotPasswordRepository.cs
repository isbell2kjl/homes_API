using homes_API.Models;

namespace homes_API.Repositories;

public interface IForgotPasswordRepository
{
    void ForgotPassword(ForgotPasswordRequest model, string origin);
    void ValidateResetToken(ValidateResetTokenRequest model);
    void ResetPassword(ResetPasswordRequest model);

}