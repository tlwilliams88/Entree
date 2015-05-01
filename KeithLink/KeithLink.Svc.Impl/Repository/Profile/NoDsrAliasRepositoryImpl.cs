using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class NoDsrAliasRepositoryImpl : IDsrAliasRepository {
        #region IDsrAliasRepository Members

        public IQueryable<DsrAlias> ReadByUser(Guid userId) {
            throw new NotImplementedException();
        }

        #endregion

        #region IBaseEFREpository<DsrAlias> Members

        public void Create(DsrAlias domainObject) {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(DsrAlias domainObject) {
            throw new NotImplementedException();
        }

        public void Update(DsrAlias domainObject) {
            throw new NotImplementedException();
        }

        public void Delete(DsrAlias domainObject) {
            throw new NotImplementedException();
        }

        public void Delete(System.Linq.Expressions.Expression<Func<DsrAlias, bool>> where) {
            throw new NotImplementedException();
        }
        
        public DsrAlias ReadById(long Id) {
            throw new NotImplementedException();
        }

        public IEnumerable<DsrAlias> Read<TProperty>(System.Linq.Expressions.Expression<Func<DsrAlias, bool>> where, System.Linq.Expressions.Expression<Func<DsrAlias, TProperty>> include) where TProperty : class {
            throw new NotImplementedException();
        }

        public IEnumerable<DsrAlias> Read(System.Linq.Expressions.Expression<Func<DsrAlias, bool>> where) {
            throw new NotImplementedException();
        }

        public IEnumerable<DsrAlias> ReadAll() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
