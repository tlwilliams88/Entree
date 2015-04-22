using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IProfileService" in both code and config file together.
	[ServiceContract]
	public interface IProfileService
	{
		[OperationContract]
		void GeneratePasswordResetRequest(string email);
		[OperationContract]
		bool IsTokenValid(string token);
		[OperationContract]
		bool ResetPassword(ResetPasswordModel resetPassword);
	}
}
