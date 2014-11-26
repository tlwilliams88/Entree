using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Core.Extensions.Orders.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Orders.History;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalOrderLogicImpl: IInternalOrderLogic
	{
		private readonly IOrderHistoryHeaderRepsitory orderHistoryHeaderRepository;
		private readonly IUnitOfWork unitOfWork;

		public InternalOrderLogicImpl(IOrderHistoryHeaderRepsitory orderHistoryRepository, IUnitOfWork unitOfWork)
		{
			this.orderHistoryHeaderRepository = orderHistoryRepository;
			this.unitOfWork = unitOfWork;
		}

		public DateTime? ReadLatestUpdatedDate(Core.Models.SiteCatalog.UserSelectedContext catalogInfo)
		{
			var orders = orderHistoryHeaderRepository.Read(o => o.CustomerNumber.Equals(catalogInfo.CustomerId, StringComparison.InvariantCultureIgnoreCase) &&
				o.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase));

			if (orders.Any())
				return orders.Max(m => m.ModifiedUtc);
			else
				return null;
		}

        public List<OrderHistoryFile> GetLastFiveOrderHistory( Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber ) {
            List<OrderHistoryFile> returnValue = new List<OrderHistoryFile>();

            IEnumerable<Core.Models.Orders.History.EF.OrderHistoryHeader> history = orderHistoryHeaderRepository.GetLastFiveOrdersByItem( catalogInfo.BranchId, catalogInfo.CustomerId, itemNumber );

            foreach (Core.Models.Orders.History.EF.OrderHistoryHeader h in history) {
                OrderHistoryFile root = new OrderHistoryFile() {
                    Header = new OrderHistoryHeader() {
                        BranchId = h.BranchId,
                        CustomerNumber = h.CustomerNumber,
                        InvoiceNumber = h.CustomerNumber,
                        DeliveryDate = h.DeliveryDate,
                        PONumber = h.PONumber,
                        ControlNumber = h.ControlNumber,
                        OrderStatus = h.OrderStatus,
                        FutureItems = h.FutureItems,
                        ErrorStatus = h.ErrorStatus,
                        RouteNumber = h.RouteNumber,
                        StopNumber = h.StropNumber
                    }
                };

                foreach (Core.Models.Orders.History.EF.OrderHistoryDetail d in h.OrderDetails.Where( x => x.ItemNumber.Equals( itemNumber ) )) {
                    OrderHistoryDetail detail = new OrderHistoryDetail() {
                        LineNumber = d.LineNumber,
                        ItemNumber = d.ItemNumber,
                        OrderQuantity = d.OrderQuantity,
                        ShippedQuantity = d.ShippedQuantity,
                    };

                    root.Details.Add( detail );
                }

                returnValue.Add( root );
            }

            return returnValue;
        }


        public List<OrderHistoryHeader> GetCustomerOrderHistories(Core.Models.SiteCatalog.UserSelectedContext catalogInfo)
        {
            var histHeaders = orderHistoryHeaderRepository.GetCustomerOrderHistoryHeaders(catalogInfo.BranchId, catalogInfo.CustomerId).ToList();
            // convert to orderhistoryheader from ef orderhistoryheader
            List<OrderHistoryHeader> orderHistories = new List<OrderHistoryHeader>();

            foreach (var histHeader in histHeaders)
            {
                OrderHistoryHeader retHistHeader = histHeader.ToOrderHistoryHeader();
                retHistHeader.Items = histHeader.OrderDetails.Select(l => l.ToOrderHistoryDetail()).ToList();
                orderHistories.Add(retHistHeader);
            }

            return orderHistories;
        }

    }
}
