﻿using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ProfileService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select ProfileService.svc or ProfileService.svc.cs at the Solution Explorer and start debugging.
	public class ProfileService : IProfileService
	{
		private IInternalPasswordResetLogic passwordResetLogic;

		public ProfileService(IInternalPasswordResetLogic passwordResetLogic)
		{
			this.passwordResetLogic = passwordResetLogic;
		}

		public void GeneratePasswordResetRequest(string email)
		{
			passwordResetLogic.GeneratePasswordResetLink(email);
		}
		
		public bool IsTokenValid(string token)
		{
			return passwordResetLogic.IsTokenValid(token);
		}


		public bool ResetPassword(Core.Models.Profile.ResetPasswordModel resetPassword)
		{
			return passwordResetLogic.ResetPassword(resetPassword);
		}
	}
}