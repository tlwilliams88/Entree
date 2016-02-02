using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile.PasswordReset
{
	public interface IInternalPasswordResetLogic
	{
		void GeneratePasswordResetLink(string emailAddress);
		void GenerateNewUserPasswordLink(string emailAddress);
		string IsTokenValid(string token);
		bool ResetPassword(ResetPasswordModel resetPassword);
	}
}
