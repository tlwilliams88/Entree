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
		private readonly IUserActiveCartRepository userActiveCartRepository;


		public InternalOrderLogicImpl(IOrderHistoryHeaderRepsitory orderHistoryRepository, IUnitOfWork unitOfWork, IUserActiveCartRepository userActiveCartRepository)
		{
			this.orderHistoryHeaderRepository = orderHistoryRepository;
			this.unitOfWork = unitOfWork;
			this.userActiveCartRepository = userActiveCartRepository;
		}

		public DateTime? ReadLatestUpdatedDate(Core.Models.SiteCatalog.UserSelectedContext catalogInfo)
		{
			return orderHistoryHeaderRepository.ReadLatestOrderDate(catalogInfo);
					
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



		public Core.Models.Orders.UserActiveCartModel GetUserActiveCart(Guid userId)
		{
			var activeCart = userActiveCartRepository.Read(u => u.UserId == userId).FirstOrDefault();

			if (activeCart == null)
				return null;

			return new Core.Models.Orders.UserActiveCartModel() { UserId = activeCart.UserId, CartId = activeCart.CartId };

		}

		public void SaveUserActiveCart(Guid userId, Guid cartId)
		{
			var activeCart = userActiveCartRepository.Read(u => u.UserId == userId).FirstOrDefault();

			if (activeCart == null)
				userActiveCartRepository.Create(new Core.Models.Orders.EF.UserActiveCart() { CartId = cartId, UserId = userId });
			else
			{
				activeCart.CartId = cartId;
				userActiveCartRepository.Update(activeCart);
			}

			unitOfWork.SaveChanges();
		}
	}
}
