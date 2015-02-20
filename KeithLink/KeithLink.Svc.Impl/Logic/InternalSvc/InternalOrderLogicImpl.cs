using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Core.Extensions.Orders.History;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceServer.Core;
using CommerceServer.Core.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;

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
                        ActualDeliveryTime = h.ActualDeliveryTime,
                        EstimatedDeliveryTime = h.EstimatedDeliveryTime,
                        ScheduledDeliveryTime = h.ScheduledDeliveryTime,
                        DeliveryOutOfSequence = h.DeliveryOutOfSequence,
                        RouteNumber = h.RouteNumber,
                        StopNumber = h.StopNumber
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



		public Core.Models.Orders.UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId)
		{
			var activeCart = userActiveCartRepository.Read(u => u.UserId == userId && u.CustomerId.Equals(catalogInfo.CustomerId) && u.BranchId.Equals(catalogInfo.BranchId)).FirstOrDefault();

			if (activeCart == null)
				return null;

			return new Core.Models.Orders.UserActiveCartModel() { UserId = activeCart.UserId, CartId = activeCart.CartId };

		}

		public void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId)
		{
			var activeCart = userActiveCartRepository.Read(u => u.UserId == userId && u.CustomerId.Equals(catalogInfo.CustomerId) && u.BranchId.Equals(catalogInfo.BranchId)).FirstOrDefault();

			if (activeCart == null)
				userActiveCartRepository.Create(new Core.Models.Orders.EF.UserActiveCart() { CartId = cartId, UserId = userId, CustomerId = catalogInfo.CustomerId, BranchId = catalogInfo.BranchId });
			else
			{
				activeCart.CartId = cartId;
				userActiveCartRepository.Update(activeCart);
			}

			unitOfWork.SaveChanges();
		}

        public List<Core.Models.Orders.OrderHeader> GetSubmittedUnconfirmedOrders()
        {
			//var manager = Helpers.CommerceServerCore.GetOrderManagementContext().PurchaseOrderManager;
			//System.Data.DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
			//SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
			//SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Status", "Submitted");
			//DataSet results = manager.SearchPurchaseOrders(clause, new SearchOptions() { NumberOfRecordsToReturn = 100, PropertiesToReturn = "OrderGroupId,LastModified,SoldToId" });

			//int c = results.Tables.Count;

			//// Get the value of the OrderGroupId property of each
			//// purchase order.
			//List<Guid> poIds = new List<Guid>();
			//foreach (DataRow row in results.Tables[0].Rows)
			//{
			//	poIds.Add(new Guid(row["OrderGroupId"].ToString()));
			//}

			//// Get the XML representation of the purchase orders.
			//System.Xml.XmlElement poXml = manager.GetPurchaseOrdersAsXml(poIds.ToArray());
			//System.Xml.XmlNodeList nodes = poXml.SelectNodes("/OrderGroups/PurchaseOrder");

			com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
			var poXml = client.GetUnconfirmatedOrders();
			System.Xml.XmlNodeList nodes = poXml.SelectNodes("/PurchaseOrder");
			//return poXml.SelectNodes("/OrderGroups/PurchaseOrder");
            List<Core.Models.Orders.OrderHeader> orders = new List<Core.Models.Orders.OrderHeader>();
            foreach (System.Xml.XmlNode p in nodes)
            {
                Core.Models.Orders.OrderHeader order = new Core.Models.Orders.OrderHeader();
                order.CustomerNumber = p.SelectNodes("WeaklyTypedProperties/WeaklyTypedProperty[@Name='CustomerId']")[0].Attributes["Value"].Value;
                order.Branch = p.SelectNodes("WeaklyTypedProperties/WeaklyTypedProperty[@Name='BranchId']")[0].Attributes["Value"].Value;
                order.ControlNumber = Convert.ToInt32(p.Attributes["TrackingNumber"].Value);
                order.OrderCreateDateTime = Convert.ToDateTime(p.Attributes["Created"].Value).ToUniversalTime();
                orders.Add(order);
            }
            
            return orders;
        }

        public Guid GetUserIdForControlNumber(int controlNumber)
        { // todo move this to a common location; confirmation logic does the same thing
            System.Data.DataSet searchableProperties = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchableProperties(System.Globalization.CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause trackingNumberClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "TrackingNumber", controlNumber.ToString("0000000.##"));

            // Create search options.

            SearchOptions options = new SearchOptions();
            options.PropertiesToReturn = "SoldToId";
            options.SortProperties = "SoldToId";
            options.NumberOfRecordsToReturn = 1;
            // Perform the search.
            System.Data.DataSet results = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().SearchPurchaseOrders(trackingNumberClause, options);

            if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0)
            {
                return Guid.Parse(results.Tables[0].Rows[0].ItemArray[2].ToString());
            }
            return Guid.Empty;
        }
	}
}
