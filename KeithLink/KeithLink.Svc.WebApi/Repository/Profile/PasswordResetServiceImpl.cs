using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi.Repository.Profile
{
	public class PasswordResetServiceImpl : IPasswordResetService
	{
		#region attributes
		private com.benekeith.ProfileService.IProfileService _client;
		#endregion

		#region ctor
		public PasswordResetServiceImpl(com.benekeith.ProfileService.IProfileService client)
		{
			_client = client;
		}
		#endregion

		public void GeneratePasswordResetRequest(string email)
		{
			_client.GeneratePasswordResetRequest(email);
		}

		public bool IsTokenValid(string token)
		{
			return _client.IsTokenValid(token);
		}

		public bool ResetPassword(Core.Models.Profile.ResetPasswordModel resetPassword)
		{
			return _client.ResetPassword(resetPassword);
		}

		public void GeneratePasswordForNewUser(string email)
		{
			_client.GeneratePasswordForNewUser(email);
		}
	}
}