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

        public DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            throw new NotImplementedException();
        }

      

        public List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId) {
            throw new NotImplementedException();
        }

        #endregion


		public void DeleteDsrAlias(long dsrAliasId, string email)
		{
			throw new NotImplementedException();
		}
	}
}
