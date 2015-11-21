// KeithLink
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using KeithLink.Svc.Impl.Repository.EF.Operational;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class SettingsRepositoryImpl : EFBaseRepository<Settings>, ISettingsRepository {

        #region constructor

        public SettingsRepositoryImpl( IUnitOfWork unitOfWork ) : base( unitOfWork ) { }

        #endregion

        #region methods / functions

        public IQueryable<Settings> ReadByUser( Guid userId ) {
            return this.Entities.Where( s => s.UserId.Equals( userId ) );
        }

        #endregion

    }
}
