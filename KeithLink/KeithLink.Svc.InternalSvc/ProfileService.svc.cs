using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
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
	public class ProfileService : IProfileService {
        #region attribuites
        private readonly IInternalPasswordResetLogic _passwordResetLogic;
        private readonly IDsrAliasLogic _aliasLogic;
		private readonly IInternalMarketingPreferenceLogic _marketingPrefLogic;
        private readonly ISettingsLogic _settingsLogic;
        #endregion

        #region ctor
        public ProfileService(IInternalPasswordResetLogic passwordResetLogic, IDsrAliasLogic dsrAliasLogic, IInternalMarketingPreferenceLogic marketingPrefLogic, ISettingsLogic settingsLogic)
		{
			_passwordResetLogic = passwordResetLogic;
            _aliasLogic = dsrAliasLogic;
			_marketingPrefLogic = marketingPrefLogic;
            _settingsLogic = settingsLogic;
		}
        #endregion

        #region methods
        public DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            return _aliasLogic.CreateDsrAlias(userId, email, dsr);
        }

        public void DeleteDsrAlias(long dsrAliasId, string email) {
            _aliasLogic.DeleteDsrAlias(dsrAliasId, email);
        }

        public void GeneratePasswordResetRequest(string email) {
			_passwordResetLogic.GeneratePasswordResetLink(email);
		}

		public void GeneratePasswordForNewUser(string email)
		{
			_passwordResetLogic.GenerateNewUserPasswordLink(email);
		}
		
		public bool IsTokenValid(string token) {
			return _passwordResetLogic.IsTokenValid(token);
		}

		public List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId)
		{
            return _aliasLogic.GetAllDsrAliasesByUserId(userId);
        }

		public bool ResetPassword(Core.Models.Profile.ResetPasswordModel resetPassword) {
			return _passwordResetLogic.ResetPassword(resetPassword);
		}

		public void CreateMarketingPref(MarketingPreferenceModel preference)
		{
			_marketingPrefLogic.CreateMarketingPreference(preference);
		}

		public List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to)
		{
			return _marketingPrefLogic.ReadMarketingPreferences(from, to);
		}

        public List<SettingsModel> ReadProfileSettings( Guid userId ) {
            return _settingsLogic.GetAllUserSettings( userId );
        }

        public void SaveProfileSettings( SettingsModel settings ) {
            _settingsLogic.CreateOrUpdateSettings( settings );
        }

        #endregion
	}
}
