// KeithLink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Core.Extensions;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class SettingsLogicImpl : ISettingsLogic {

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

        public List<SettingsModel> GetAllUserSettings( Guid userId ) {
            List<SettingsModel> returnValue = new List<SettingsModel>();

            IQueryable<Core.Models.Profile.EF.Settings> settings = _repo.ReadByUser( userId );

            foreach (Core.Models.Profile.EF.Settings s in settings) {
                returnValue.Add( s.ToModel() );
            }

            return returnValue;
        }

        public void CreateOrUpdateSettings( SettingsModel settings ) {
            KeithLink.Svc.Core.Models.Profile.EF.Settings mySettings = settings.ToEFSettings();

            _repo.CreateOrUpdate( mySettings );
            _uow.SaveChanges();
        }

        public void DeleteSettings(SettingsModel settings)
        {
            KeithLink.Svc.Core.Models.Profile.EF.Settings mySettings = settings.ToEFSettings();

            _repo.Delete(mySettings);
            _uow.SaveChanges();
        }

        #endregion

    }
}
