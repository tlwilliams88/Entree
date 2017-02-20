using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders.History.EF {
    public class OrderHistoyrHeaderRepositoryImpl : EFBaseRepository<OrderHistoryHeader>, IOrderHistoryHeaderRepsitory {
        #region attributes
        private const string PARMNAME_BRANCHID = "@BranchId";
        private const string PARMNAME_CUSTOMERNUMBER = "@CustomerNumber";
        private const string PARMNAME_ITEMNUMBER = "@ItemNumber";

        private const string SQL_GETLASTFIVEORDERS = "[orders].[usp_GetLastFiveOrdersForItem] @BranchId, @CustomerNumber, @ItemNumber";
        #endregion

        #region ctor
        public OrderHistoyrHeaderRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region methods
        public IEnumerable<OrderHistoryHeader> GetCustomerOrderHistoryHeaders(string branchId, string customerNumber) {
            return this.Entities.Where(o => o.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) && o.CustomerNumber.Equals(customerNumber)).OrderByDescending(o => o.CreatedUtc);
        }

        public List<OrderHistoryHeader> GetLastFiveOrdersByItem(string branchId, string customerNumber, string itemNumber) {
            SqlParameter[] parms = {
                new SqlParameter(PARMNAME_BRANCHID, branchId),
                new SqlParameter(PARMNAME_CUSTOMERNUMBER, customerNumber),
                new SqlParameter(PARMNAME_ITEMNUMBER, itemNumber)
            };

            try {
                return this.UnitOfWork.Context.Database.SqlQuery<OrderHistoryHeader>(SQL_GETLASTFIVEORDERS, parms).ToList();
            } catch (Exception) {
                return new List<OrderHistoryHeader>();
            }
        }

        public IEnumerable<OrderHistoryHeader> ReadByConfirmationNumber(string confirmationNumber, string orderSystem) {
			return Entities.Include(d => d.OrderDetails).Where(l => l.OrderSystem == orderSystem && (
                                                                        l.ControlNumber == confirmationNumber ||
                                                                        l.OriginalControlNumber == confirmationNumber)
                                                              );

        }

        public IEnumerable<OrderHistoryHeader> ReadForInvoice(string branchId, string invoiceNumber) {
            return Entities.Include(d => d.OrderDetails)
                           .Where(l => (l.BranchId.Equals(branchId) && l.InvoiceNumber.Equals(invoiceNumber)));
        }

        public IEnumerable<OrderHistoryHeader> ReadForInvoiceHeader(string branchId, string invoiceNumber)
        {
            return Entities.Where(l => (l.BranchId.Equals(branchId) && l.InvoiceNumber.Equals(invoiceNumber)));
        }

        public string ReadLatestOrderDate(Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return this.Entities.Where(o => o.CustomerNumber.Equals(catalogInfo.CustomerId, StringComparison.InvariantCultureIgnoreCase) &&
                o.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase)).Select(m => m.ModifiedUtc).DefaultIfEmpty().Max().ToLongDateFormatWithTime();
        }
        #endregion
	}
}
