using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

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
        DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr);

        [OperationContract]
        void DeleteDsrAlias(long dsrAliasId, string email);

		[OperationContract]
		void GeneratePasswordResetRequest(string email);

        [OperationContract]
		bool IsTokenValid(string token);

        [OperationContract]
        List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId);

        [OperationContract]
		bool ResetPassword(ResetPasswordModel resetPassword);

		[OperationContract]
		void GeneratePasswordForNewUser(string email);

		[OperationContract]
		void CreateMarketingPref(MarketingPreferenceModel preference);

		[OperationContract]
		List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to);

        [OperationContract]
        List<SettingsModel> ReadProfileSettings( Guid userId );

        [OperationContract]
        void SaveProfileSettings( SettingsModel settings );
		
	}
}
