using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KeithLink.Svc.Impl.Repository.Orders.History.EF {
    public class OrderHistoyrHeaderRepositoryImpl : EFBaseRepository<OrderHistoryHeader>, IOrderHistoryHeaderRepsitory {
        #region ctor
        public OrderHistoyrHeaderRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region methods
        public IEnumerable<OrderHistoryHeader> GetCustomerOrderHistoryHeaders(string branchId, string customerNumber) {
            return this.Entities.Where(o => o.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) && o.CustomerNumber.Equals(customerNumber)).OrderByDescending(o => o.CreatedUtc);
        }

        public IEnumerable<OrderHistoryHeader> GetLastFiveOrdersByItem(string branchId, string customerNumber, string itemNumber) {
            var query = (from x in Entities
                         where x.BranchId.Equals(branchId) && x.CustomerNumber.Equals(customerNumber) && x.OrderDetails.Where(y => y.ItemNumber.Equals(itemNumber)).Count() > 0
                         orderby x.DeliveryDate descending
                         select x).Take( 5 );

            return query.ToList();
        }

        public IEnumerable<OrderHistoryHeader> ReadByConfirmationNumber(string confirmationNumber) {
			return Entities.Include(d => d.OrderDetails).Where(l => l.ControlNumber == confirmationNumber);
        }

        public IEnumerable<OrderHistoryHeader> ReadForInvoice(string branchId, string invoiceNumber) {
            return Entities.Include(d => d.OrderDetails).Where(l => (l.BranchId.Equals(branchId) && l.InvoiceNumber.Equals(invoiceNumber)));
        }

        public DateTime? ReadLatestOrderDate(Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return this.Entities.Where(o => o.CustomerNumber.Equals(catalogInfo.CustomerId, StringComparison.InvariantCultureIgnoreCase) &&
                o.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase)).Select(m => m.ModifiedUtc).DefaultIfEmpty().Max();
        }
        #endregion




	}
}
