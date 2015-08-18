// KeithLink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Core.Extensions;

// Core
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class SettingsLogicImpl : ISettingsLogicImpl {

        #region attributes 

        readonly ISettingsRepository _repo;
        readonly IUnitOfWork _uow; 

        #endregion

        #region constructor

        public SettingsLogicImpl( IUnitOfWork uow, ISettingsRepository repo ) {
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
        public List<SettingsModel> GetAllUserSettings( Guid userId ) {
            List<SettingsModel> returnValue = new List<SettingsModel>();

            IQueryable<Core.Models.Profile.EF.Settings> settings = _repo.ReadByUser( userId );

            foreach (Core.Models.Profile.EF.Settings s in settings) {
                returnValue.Add( s.ToModel() );
            }

            return returnValue;
        }

        /// <summary>
        /// Creates or Updates the passed in Settings Model
        /// </summary>
        /// <param name="settings"></param>
        public void CreateOrUpdateSettings( SettingsModel settings ) {
            Core.Models.Profile.EF.Settings mySettings = settings.ToEFSettings();

            _repo.CreateOrUpdate( mySettings );
            _uow.SaveChanges();
        }

        public void DeleteSettings(SettingsModel settings)
        {
            Core.Models.Profile.EF.Settings mySettings = settings.ToEFSettings();

            _repo.Delete(mySettings);
            _uow.SaveChanges();
        }

        /// <summary>
        /// This creates a default setting when a user is created.
        /// </summary>
        public void CreateDefaultSettings(Guid userId)
        {
            Core.Models.Profile.EF.Settings settings = new Core.Models.Profile.EF.Settings();
            
            // Create ItemsPerPage setting
            settings.Key = "ItemsPerPage";
            settings.Value = "50";
            _repo.CreateOrUpdate(settings);

            // TODO: Create other defaults as they become available.

            // Save the repository
            _uow.SaveChanges();

        }
        #endregion
    }
}
