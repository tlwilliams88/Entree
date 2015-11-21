// KeithLink
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Profile.EF;


// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface ISettingsRepository : IBaseEFREpository<Settings>  {
        IQueryable<Settings> ReadByUser( Guid userId );
    }
}
