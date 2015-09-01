﻿// KeithLink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

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
        public List<SettingsModelReturn> GetAllUserSettings( Guid userId ) {
            List<SettingsModelReturn> returnValue = new List<SettingsModelReturn>();

            IQueryable<Core.Models.Profile.EF.Settings> settings = _repo.ReadByUser( userId );

            foreach (Core.Models.Profile.EF.Settings s in settings) {
                returnValue.Add( s.ToReturnModel() );
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

        public void DeleteSettings(SettingsModel settings)
        {
            Settings mySettings = _repo.Read( x => x.Key == settings.Key && x.UserId == settings.UserId ).FirstOrDefault();

            if (mySettings != null) {
                _repo.Delete( mySettings );
                _uow.SaveChanges();
            }
        }
        
        #endregion
    }
}
