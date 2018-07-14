using Entree.Core.Models.EF;
using Entree.Core.Models.Profile;
using Entree.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Entree.Core.Interface.Profile {
    public interface IDsrAliasRepository : IBaseEFREpository<DsrAlias> {
        IQueryable<DsrAlias> ReadByUser(Guid userId);
    }
}
