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

        ISettingsRepository _repo;
        IUnitOfWork _uow; 

        #endregion

        #region constructor

        public SettingsLogicImpl( IUnitOfWork uow, ISettingsRepository repo ) {
            _repo = repo;
            _uow = uow;
        }

        #endregion


        #region methods / functions

        public List<SettingsModel> GetAllUserSettings( Guid userId ) {
            IQueryable<KeithLink.Svc.Core.Models.Profile.EF.Settings> settings = _repo.ReadByUser( userId );
            return settings.Select( s => s.ToModel() ).ToList();
        }

        public void CreateOrUpdateSettings( SettingsModel settings ) {
            KeithLink.Svc.Core.Models.Profile.EF.Settings mySettings = settings.ToEFSettings();

            _repo.Create( mySettings );
            _uow.SaveChanges();
        }

        #endregion

    }
}
