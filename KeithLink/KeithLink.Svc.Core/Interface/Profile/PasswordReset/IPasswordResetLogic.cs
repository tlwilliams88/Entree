﻿using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.Profile.PasswordReset {
    public interface IPasswordResetLogic {
        void GenerateNewUserPasswordLink(string emailAddress);

        void GeneratePasswordResetLink(string emailAddress);

        string IsTokenValid(string token);

        bool ResetPassword(ResetPasswordModel resetPassword);
    }
}
