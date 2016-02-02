using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
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
        private readonly ICustomerRepository _customerRepo;
        private readonly IUserProfileLogic _profileLogic;
        private readonly IEventLogRepository _eventLog;
        private readonly IInternalPasswordResetLogic _passwordResetLogic;
        private readonly IDsrAliasLogic _aliasLogic;
		private readonly IInternalMarketingPreferenceLogic _marketingPrefLogic;
        private readonly ISettingsLogicImpl _settingsLogic;
        private readonly IUserMessagingPreferenceRepository _userMessagingPreferenceRepository;
        #endregion

        #region ctor
        public ProfileService(IInternalPasswordResetLogic passwordResetLogic, IDsrAliasLogic dsrAliasLogic, IInternalMarketingPreferenceLogic marketingPrefLogic, 
                              ISettingsLogicImpl settingsLogic, IUserProfileLogic profileLogic, IEventLogRepository eventLog, 
                              IUserMessagingPreferenceRepository userMessagingPreferenceRepository, ICustomerRepository customerRepository)
		{
			_passwordResetLogic = passwordResetLogic;
            _aliasLogic = dsrAliasLogic;
			_marketingPrefLogic = marketingPrefLogic;
            _settingsLogic = settingsLogic;
            _profileLogic = profileLogic;
            _eventLog = eventLog;
            _userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            _customerRepo = customerRepository;
		}
        #endregion

        #region methods
        public DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            return _aliasLogic.CreateDsrAlias(userId, email, dsr);
        }

        public void CreateMarketingPref(MarketingPreferenceModel preference)
		{
			_marketingPrefLogic.CreateMarketingPreference(preference);
		}

        public void DeleteDsrAlias(long dsrAliasId, string email) {
            _aliasLogic.DeleteDsrAlias(dsrAliasId, email);
        }

		public void GeneratePasswordForNewUser(string email)
		{
			_passwordResetLogic.GenerateNewUserPasswordLink(email);
		}
		
        public void GeneratePasswordResetRequest(string email) {
			_passwordResetLogic.GeneratePasswordResetLink(email);
		}

		public List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId)
		{
            return _aliasLogic.GetAllDsrAliasesByUserId(userId);
        }

        public string IsTokenValid(string token) {
			return _passwordResetLogic.IsTokenValid(token);
		}

		public List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to)
		{
			return _marketingPrefLogic.ReadMarketingPreferences(from, to);
		}

		public bool ResetPassword(Core.Models.Profile.ResetPasswordModel resetPassword) {
			return _passwordResetLogic.ResetPassword(resetPassword);
		}

        public List<SettingsModelReturn> ReadProfileSettings( Guid userId ) {
            return _settingsLogic.GetAllUserSettings( userId );
        }

        public void SaveProfileSettings( SettingsModel settings ) {
            _settingsLogic.CreateOrUpdateSettings( settings );
        }

        public void DeleteProfileSetting( SettingsModel settings ) {
            _settingsLogic.DeleteSettings( settings );
        }

        public void SetDefaultApplicationSettings(string email)
        {
            UserProfileReturn profile = _profileLogic.GetUserProfile(email);
            SetDefaultApplicationNotifySetting(profile, Core.Enumerations.Messaging.NotificationType.OrderConfirmation);
            SetDefaultApplicationNotifySetting(profile, Core.Enumerations.Messaging.NotificationType.OrderShipped);
            SetDefaultApplicationNotifySetting(profile, Core.Enumerations.Messaging.NotificationType.InvoiceAttention);
            SetDefaultApplicationNotifySetting(profile, Core.Enumerations.Messaging.NotificationType.HasNews);
            SetDefaultApplicationNotifySetting(profile, Core.Enumerations.Messaging.NotificationType.PaymentConfirmation);
            SetDefaultApplicationProfileSetting(profile, 
                Core.Enumerations.Profile.SettingKeys.PageLoadSize, Core.Enumerations.Profile.DefaultSetting.PageLoadSize);
            SetDefaultApplicationProfileSetting(profile,
                Core.Enumerations.Profile.SettingKeys.Sort, Core.Enumerations.Profile.DefaultSetting.Sort);
        }

        private void SetDefaultApplicationProfileSetting(UserProfileReturn profile, string setKey, string setValue)
        {
            SettingsModel settings = new SettingsModel()
            {
                UserId = profile.UserProfiles[0].UserId,
                Key = setKey,
                Value = setValue
            };
            try
            {
                SaveProfileSettings(settings);
            }
            catch (Exception ex)
            {
                _eventLog.WriteErrorLog("Error saving profile settings for user: ", ex);
            }
        }

        private void SetDefaultApplicationNotifySetting(UserProfileReturn profile, NotificationType notifyType)
        {
            Core.Models.Messaging.EF.UserMessagingPreference pref = new Core.Models.Messaging.EF.UserMessagingPreference()
            {
                UserId = profile.UserProfiles[0].UserId,
                Channel = Core.Enumerations.Messaging.Channel.Web,
                NotificationType = notifyType
            };
            _userMessagingPreferenceRepository.Create(pref);
        }

        public void UpdateCustomerCanViewPricing(Guid customerId, bool canView) {
            _customerRepo.UpdateCustomerCanViewPricing(customerId, canView);
        }

        #endregion
	}
}
