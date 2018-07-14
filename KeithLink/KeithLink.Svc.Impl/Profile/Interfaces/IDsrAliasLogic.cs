using Entree.Core.Models.Profile;
using Entree.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Profile {
    public interface IDsrAliasLogic {
        DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr);

        void DeleteDsrAlias(long dsrAliasId, string email);

		List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId);
    }
}
