using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Customers {
    public class InternalUserAccessRepository : EFBaseRepository<InternalUserAccess>, IInternalUserAccessRepository {
        #region ctor
        public InternalUserAccessRepository( IUnitOfWork uow ) : base( uow ) { }
        #endregion

        #region methods
        public List<InternalUserAccess> GetAllCustomersForUser( string emailAddress ) {
            return this.Entities.Where( i => i.EmailAddress.Equals( emailAddress, StringComparison.InvariantCultureIgnoreCase ) ).ToList();
        }

        public List<InternalUserAccess> GetAllUsersWithAccessToCustomer( UserSelectedContext customer ) {
            return this.Entities.Where( i => i.BranchId.Equals( customer.BranchId, StringComparison.InvariantCultureIgnoreCase ) &&
                                             i.CustomerNumber.Equals( customer.CustomerId, StringComparison.InvariantCultureIgnoreCase ) ).ToList();
        }

        public void Save( InternalUserAccess model ) {
            this.CreateOrUpdate( model );
            this.UnitOfWork.SaveChanges();
        }
        #endregion
    }
}