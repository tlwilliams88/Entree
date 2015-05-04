using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IDsrAliasRepository : IBaseEFREpository<DsrAlias> {
        IQueryable<DsrAlias> ReadByUser(Guid userId);
    }
}
