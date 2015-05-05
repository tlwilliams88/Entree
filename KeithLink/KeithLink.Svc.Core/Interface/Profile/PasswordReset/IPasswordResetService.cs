using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile.PasswordReset
{
	public interface IPasswordResetService
	{
		void GeneratePasswordResetRequest(string email);
		void GeneratePasswordForNewUser(string email);
		bool IsTokenValid(string token);
		bool ResetPassword(ResetPasswordModel resetPassword);
	}
}
