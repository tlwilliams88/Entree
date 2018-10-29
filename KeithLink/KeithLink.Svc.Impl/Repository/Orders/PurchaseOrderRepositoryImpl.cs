using CommerceServer.Core;
using CommerceServer.Foundation;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Repository.DataConnection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;

namespace KeithLink.Svc.Impl.Repository.Orders
{
	public class PurchaseOrderRepositoryImpl: DapperDatabaseConnection, IPurchaseOrderRepository
    {
        private readonly IEventLogRepository _log;

        public PurchaseOrderRepositoryImpl(IEventLogRepository log) : base(Configuration.CSTransactionsDbConnection)
        {
            _log = log;
        }

        public Guid? GetSoldToIdForPurchaseOrderByInvoice(string poNumber)
        {
            DataSet searchableProperties = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchableProperties(System.Globalization.CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause poCluase = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "TrackingNumber", poNumber);
            //SearchClause customerClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "CustomerId", customerNumber);
            //SearchClause branchClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "BranchId", branchId);
            //SearchClause invoiceClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Masternumber", invoiceNumber);
            //SearchClause joinedClause = searchClauseFactory.IntersectClauses(branchClause, invoiceClause);

            // Create search options.
            SearchOptions options = new SearchOptions();
            options.PropertiesToReturn = "SoldToId";
            options.SortProperties = "SoldToId";
            options.NumberOfRecordsToReturn = 1;

            // Perform the search.
            DataSet results = CommerceServerCore.GetPoManager().SearchPurchaseOrders(poCluase, options);

            if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0)
            {
                // Enumerate the results of the search.
                return Guid.Parse(results.Tables[0].Rows[0].ItemArray[2].ToString());
            } 
            else
            {
                return null;
            }
        }

		public PurchaseOrder ReadPurchaseOrder(Guid customerId, string orderNumber)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");

            var searchModel = queryBaskets.SearchCriteria.Model;
            searchModel.Properties["UserId"] = customerId.ToCommerceServerFormat();
            searchModel.Properties["BasketType"] = 1;
            searchModel.Properties["OrderNumber"] = orderNumber;

            queryBaskets.QueryOptions.RefreshBasket = false;

            var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
						
			return ((PurchaseOrder)basketResponse.CommerceEntities[0]);
		}

        public PurchaseOrder ReadPurchaseOrderByTrackingNumber(string confirmationNumber) 
        {
            Guid? userId = GetSoldToIdForPurchaseOrderByInvoice(confirmationNumber);

            if (userId.HasValue) 
            {
                var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");

                var searchModel = queryBaskets.SearchCriteria.Model;
                searchModel.Properties["UserId"] = userId.Value.ToString("B");
                searchModel.Properties["BasketType"] = 1;
                searchModel.Properties["OrderNumber"] = confirmationNumber;
                
                queryBaskets.QueryOptions.RefreshBasket = false;

                var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
                queryBaskets.RelatedOperations.Add(queryLineItems);

                var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

                if (response.OperationResponses.Count == 0)
                    return null;

                CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

                return (basketResponse.CommerceEntities.Cast<CommerceEntity>()
                    .Select(entity => (PurchaseOrder)entity)
                    .First());
            }
            else
            {
                return null;
            }
        }

        public List<PurchaseOrder> ReadPurchaseOrderHeadersByCustomerId(Guid customerId)
        {
            var manager = CommerceServerCore.GetPoManager();
            DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
            // set what to search
            SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            // set what field/value to search for
            SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "SoldToId", customerId.ToCommerceServerFormat());
            // set what fields to return
            DataSet results = manager.SearchPurchaseOrders(clause,
                new SearchOptions() { PropertiesToReturn = "OrderGroupId,TrackingNumber,Created" });

            List<PurchaseOrder> Pos = new List<PurchaseOrder>();
            List<string> poTNs = new List<string>();
            foreach (DataRow row in results.Tables[0].Rows)
            {
                DateTime created = (DateTime)row["Created"];
                if (created > DateTime.Now.AddDays(int.Parse(Configuration.PurchaseOrdersGetLatestHowManyDays) * -1))
                {
                    poTNs.Add(row["TrackingNumber"].ToString());
                }
            }
            if (poTNs.Count > 0)
            {
                foreach (string trackingNumber in poTNs)
                {
                    PurchaseOrder po = ReadPurchaseOrderByTrackingNumber(trackingNumber);
                    Pos.Add(po);
                }
            }
            return Pos;
        }

        public List<PurchaseOrder> GetPurchaseOrdersByStatus(string queryStatus)
        {
            var manager = CommerceServerCore.GetPoManager();
            DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
            // set what to search
            SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            // Get a list of the returnable properties - debug only
            //DataSet ret = manager.GetReturnableProperties("EN");
            //StringBuilder props = new StringBuilder();
            //foreach (DataRow row in ret.Tables[0].Rows)
            //{
            //    if (props.Length > 0) props.Append(",");
            //    props.Append(row.ItemArray[0].ToString());
            //}
            // set what field/value to search for
            SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Status", queryStatus);
            // set what fields to return
            DataSet results = manager.SearchPurchaseOrders(clause, new SearchOptions() { NumberOfRecordsToReturn = 100, PropertiesToReturn = "OrderGroupId,TrackingNumber" });

            int c = results.Tables.Count;

            List<PurchaseOrder> Pos = new List<PurchaseOrder>();
            List<string> poTNs = new List<string>();
            foreach (DataRow row in results.Tables[0].Rows)
            {
                poTNs.Add(row["TrackingNumber"].ToString());
            }

            // Get the XML representation of the purchase orders.
            if (poTNs.Count > 0)
            {
                foreach (string trackingNumber in poTNs)
                {
                    PurchaseOrder po = ReadPurchaseOrderByTrackingNumber(trackingNumber);
                    Pos.Add(po);
                }
            }

            return Pos;
        }


		public List<PurchaseOrder> ReadPurchaseOrderHeadersInDateRange(Guid customerId, string customerNumber, DateTime startDate, DateTime endDate)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");

            var searchModel = queryBaskets.SearchCriteria.Model;
            searchModel.Properties["UserId"] = customerId.ToCommerceServerFormat();
            searchModel.Properties["BasketType"] = 1;
            searchModel.Properties["CustomerId"] = customerNumber;
            searchModel.Properties["CreatedDateStart"] = startDate;
            searchModel.Properties["CreatedDateEnd"] = endDate;

			queryBaskets.QueryOptions.RefreshBasket = false;

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

			return basketResponse.CommerceEntities.Cast<CommerceEntity>()
                .Where(entity => entity.Properties["CustomerId"] != null 
                              && entity.Properties["CustomerId"].ToString().Equals(customerNumber)
					          && entity.Properties["RequestedShipDate"].ToString().ToDateTime().Value >= startDate 
                              && entity.Properties["RequestedShipDate"].ToString().ToDateTime().Value <= endDate)
                .Select(entity => (PurchaseOrder)entity)
                .ToList();
		}

        public void UpdatePurchaseOrderPrices(string trackingNumber, IEnumerable<LineItem> lineItems)
        {
            //throw new NotImplementedException();
            if (trackingNumber == null)
                throw new ArgumentNullException("trackingNumber");
            if (lineItems == null)
                throw new ArgumentNullException("lineItems");

            string sqlCommand = @"
	            UPDATE [dbo].[LineItems]
                SET PlacedPrice = @PlacedPrice, ListPrice = @ListPrice
                FROM [dbo].[LineItems]
                JOIN [dbo].[PurchaseOrders] 
                ON LineItems.OrderGroupId = PurchaseOrders.OrderGroupId
                WHERE PurchaseOrders.TrackingNumber = @TrackingNumber
                AND ProductId = @ProductId
                ";

            lineItems.ToList().ForEach(lineItem =>
                {
                    var parameters =
                      new
                      {
                          TrackingNumber = trackingNumber,
                          ProductId = lineItem.ProductId,
                          PlacedPrice = lineItem.PlacedPrice,
                          ListPrice = lineItem.ListPrice,
                      };

                    Execute(sqlCommand, parameters);
                }
            );
        }

        public string UpdatePurchaseOrder(PurchaseOrder order)
        {
            throw new NotImplementedException();
        }

        public string SubmitChangeOrder(Guid userId, Guid orderGroupId)
        {
            throw new NotImplementedException();
        }
    }
}
