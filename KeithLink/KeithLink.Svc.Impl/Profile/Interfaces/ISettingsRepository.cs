// KeithLink
using Entree.Core.Models.EF;
using Entree.Core.Models.Profile.EF;


// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Profile {
    public interface ISettingsRepository : IBaseEFREpository<Settings>  {
        IQueryable<Settings> ReadByUser( Guid userId );
    }
}
