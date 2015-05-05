using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile.PasswordReset
{
	public class NoPasswordResetServiceRepositoryImpl: IPasswordResetService
	{
		public void GeneratePasswordResetRequest(string email)
		{
			throw new NotImplementedException();
		}

		public void GeneratePasswordForNewUser(string email)
		{
			throw new NotImplementedException();
		}

		public bool IsTokenValid(string token)
		{
			throw new NotImplementedException();
		}

		public bool ResetPassword(Core.Models.Profile.ResetPasswordModel resetPassword)
		{
			throw new NotImplementedException();
		}
	}
}
