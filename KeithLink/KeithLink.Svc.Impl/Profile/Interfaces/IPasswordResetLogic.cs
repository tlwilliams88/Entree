using Entree.Core.Models.Profile;

namespace Entree.Core.Interface.Profile.PasswordReset {
    public interface IPasswordResetLogic {
        void GenerateNewUserPasswordLink(string emailAddress);

        void GeneratePasswordResetLink(string emailAddress);

        ValidateTokenReturn IsTokenValid(string token);

        bool ResetPassword(ResetPasswordModel resetPassword);
    }
}
