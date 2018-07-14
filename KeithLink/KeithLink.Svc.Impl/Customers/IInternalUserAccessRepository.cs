// KeithLink
using Entree.Core.Models.EF;
using Entree.Core.Models.Customers.EF;
using Entree.Core.Models.SiteCatalog;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Customers {
    public interface IInternalUserAccessRepository : IBaseEFREpository<InternalUserAccess> {
        List<InternalUserAccess> GetAllCustomersForUser( string emailAddress );
        List<InternalUserAccess> GetAllUsersWithAccessToCustomer( UserSelectedContext context );
        void Save( InternalUserAccess model );
    }
}
