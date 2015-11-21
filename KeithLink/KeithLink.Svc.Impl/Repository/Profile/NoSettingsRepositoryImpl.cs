// KeithLink
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Profile.EF;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class NoSettingsRepositoryImpl : ISettingsRepository {

        public IQueryable<Settings> ReadByUser( Guid userId ) {
            throw new NotImplementedException();
        }

        #region IBaseEFREpository<Settings> Members

        public void Create(Settings domainObject) {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(Settings domainObject) {
            throw new NotImplementedException();
        }

        public void Update(Settings domainObject) {
            throw new NotImplementedException();
        }

        public void Delete(Settings domainObject) {
            throw new NotImplementedException();
        }

        public void Delete(System.Linq.Expressions.Expression<Func<Settings, bool>> where) {
            throw new NotImplementedException();
        }
        
        public Settings ReadById(long Id) {
            throw new NotImplementedException();
        }

        public IEnumerable<Settings> Read<TProperty>(System.Linq.Expressions.Expression<Func<Settings, bool>> where, System.Linq.Expressions.Expression<Func<Settings, TProperty>> include) where TProperty : class {
            throw new NotImplementedException();
        }

        public IEnumerable<Settings> Read(System.Linq.Expressions.Expression<Func<Settings, bool>> where) {
            throw new NotImplementedException();
        }

        public IEnumerable<Settings> ReadAll() {
            throw new NotImplementedException();
        }

        #endregion

    }
}
