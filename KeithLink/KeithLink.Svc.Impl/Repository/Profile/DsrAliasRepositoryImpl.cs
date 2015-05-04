using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class DsrAliasRepositoryImpl : EFBaseRepository<DsrAlias>, IDsrAliasRepository {
        #region ctor
        public DsrAliasRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) {}
        #endregion

        #region methods
        public IQueryable<DsrAlias> ReadByUser(Guid userId) {
            return this.Entities.Where(d => d.UserId.Equals(userId));
        }
        #endregion
    }
}
