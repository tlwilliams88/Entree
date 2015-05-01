using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IDsrAliasService {
        void CreateDsrAlias(Guid userId, string email, Dsr dsr);
        void DeleteDsrAlias(int dsrAliasId);
        List<DsrAlias> GetAllDsrAliasesByUserId(Guid userId);
    }
}
