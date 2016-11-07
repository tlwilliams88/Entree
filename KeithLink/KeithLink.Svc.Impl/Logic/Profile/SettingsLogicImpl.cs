using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Extensions;

using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;

using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class SettingsLogicImpl : ISettingsLogic {

        #region attributes 
        private readonly IEventLogRepository _log;
        private readonly IUserMessagingPreferenceRepository _messagingPrefRepo;
        private readonly ISettingsRepository _repo;
        private readonly IUserProfileRepository _profileRepo;
        private readonly IUnitOfWork _uow; 
        #endregion

        #region constructor

        public SettingsLogicImpl(IUnitOfWork uow, ISettingsRepository repo, IUserProfileRepository userProfileRepository,
                                 IUserMessagingPreferenceRepository userMessagingPreferenceRepository, IEventLogRepository logRepo) {
            _log = logRepo;
            _messagingPrefRepo = userMessagingPreferenceRepository;
            _profileRepo = userProfileRepository;
            _repo = repo;
            _uow = uow;
        }
        #endregion

        #region methods / functions
        /// <summary>
        /// Finds all the settings for the customer.
        /// </summary>
        /// <param name="userId">Guid - userId</param>
        /// <returns>A collection (list) of SettingModel objects.</returns>
        public List<SettingsModelReturn> GetAllUserSettings( Guid userId ) {
            List<SettingsModelReturn> returnValue = new List<SettingsModelReturn>();

            IQueryable<Core.Models.Profile.EF.Settings> settings = _repo.ReadByUser( userId );

            foreach (Core.Models.Profile.EF.Settings s in settings) {
                if (s.Key.IndexOf("defaultorderlist,") < 0) // do not include defaultorderlist settings
                {
                    returnValue.Add(s.ToReturnModel());
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Creates or Updates the passed in Settings Model
        /// </summary>
        /// <param name="settings"></param>
        public void CreateOrUpdateSettings( SettingsModel settings ) {
            Settings mySettings = _repo.Read( x => x.Key == settings.Key && x.UserId == settings.UserId ).FirstOrDefault();

            if (mySettings != null) {
                mySettings.Value = settings.Value;
            } else {
                mySettings = settings.ToEFSettings();
            }

            _repo.CreateOrUpdate( mySettings );
            _uow.SaveChanges();
        }

        /// <summary>
        /// Finds all the settings for the customer.
        /// </summary>
        /// <param name="userId">Guid - userId</param>
        /// <returns>A collection (list) of SettingModel objects.</returns>
        public SettingsModelReturn GetUserCustomerDefaultOrderList(Guid userId, string customernumber, string branchid)
        {
            SettingsModelReturn returnValue = new SettingsModelReturn();

            IQueryable<Core.Models.Profile.EF.Settings> settings = _repo.ReadByUser(userId);

            foreach (Core.Models.Profile.EF.Settings s in settings)
            {
                if (s.Key.Equals(DefaultOrderListKey(customernumber, branchid), StringComparison.CurrentCultureIgnoreCase))
                {
                    returnValue = s.ToReturnModel();
                    returnValue.Key = "defaultorderlistid";
                }
            }

            return returnValue;
        }

        private string DefaultOrderListKey(string customernumber, string branchid)
        {
            return string.Format("{0},{1},{2}", "defaultorderlist", customernumber, branchid);
        }

        /// <summary>
        /// Creates or Updates the passed in Settings Model
        /// </summary>
        /// <param name="settings"></param>
        public void CreateOrUpdateUserCustomerDefaultOrderList(string customernumber, string branchid, SettingsModel settings)
        {
            string key = DefaultOrderListKey(customernumber, branchid);
            Settings mySettings = _repo.Read(x => 
                x.Key == key && 
                x.UserId == settings.UserId).FirstOrDefault();

            if (mySettings != null)
            {
                mySettings.Value = settings.Value;
            }
            else
            {
                mySettings = settings.ToEFSettings();
                mySettings.Key = DefaultOrderListKey(customernumber, branchid);
            }

            _repo.CreateOrUpdate(mySettings);
            _uow.SaveChanges();
        }

        public void DeleteSettings(SettingsModel settings)
        {
            Settings mySettings = _repo.Read( x => x.Key == settings.Key && x.UserId == settings.UserId ).FirstOrDefault();

            if (mySettings != null) {
                _repo.Delete( mySettings );
                _uow.SaveChanges();
            }
        }

        public void SetDefaultApplicationSettings(string email) {
            CS.UserProfile profile = _profileRepo.GetCSProfile(email);
            Guid userId = Guid.Parse(profile.Id);

            SetDefaultApplicationNotifySetting(userId, NotificationType.OrderConfirmation);
            SetDefaultApplicationNotifySetting(userId, NotificationType.OrderShipped);
            SetDefaultApplicationNotifySetting(userId, NotificationType.InvoiceAttention);
            SetDefaultApplicationNotifySetting(userId, NotificationType.HasNews);
            SetDefaultApplicationNotifySetting(userId, NotificationType.PaymentConfirmation);

            SetDefaultApplicationProfileSetting(userId, SettingKeys.PageLoadSize, DefaultSetting.PageLoadSize);
            SetDefaultApplicationProfileSetting(userId, SettingKeys.Sort, DefaultSetting.Sort);
        }

        private void SetDefaultApplicationProfileSetting(Guid userId, string setKey, string setValue) {
            SettingsModel settings = new SettingsModel() {
                UserId = userId,
                Key = setKey,
                Value = setValue
            };
            try {
                CreateOrUpdateSettings(settings);
            } catch(Exception ex) {
                _log.WriteErrorLog("Error saving profile settings for user: ", ex);
            }
        }

        private void SetDefaultApplicationNotifySetting(Guid userId, NotificationType notifyType) {
            Core.Models.Messaging.EF.UserMessagingPreference pref = new Core.Models.Messaging.EF.UserMessagingPreference() {
                UserId = userId,
                Channel = Channel.Web,
                NotificationType = notifyType
            };

            _messagingPrefRepo.Create(pref);
        }
        #endregion
    }
}
