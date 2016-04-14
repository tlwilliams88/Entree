// KeithLink
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Customers {
    public interface IInternalUserAccessRepository : IBaseEFREpository<InternalUserAccess> {
        List<InternalUserAccess> GetAllCustomersForUser( string emailAddress );
        List<InternalUserAccess> GetAllUsersWithAccessToCustomer( UserSelectedContext context );
        void Save( InternalUserAccess model );
    }
}
