using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class NoDsrAliasServiceImpl : IDsrAliasService {
        #region IDsrAliasService Members

        public void CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            throw new NotImplementedException();
        }

        public void DeleteDsrAlias(int dsrAliasId) {
            throw new NotImplementedException();
        }

        public List<DsrAlias> GetAllDsrAliasesByUserId(Guid userId) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
