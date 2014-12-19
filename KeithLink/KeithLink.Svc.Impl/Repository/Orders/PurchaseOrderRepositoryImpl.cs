using CommerceServer.Core;
using CommerceServer.Foundation;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders
{
	public class PurchaseOrderRepositoryImpl: IPurchaseOrderRepository
	{
        public Guid? GetSoldToIdForPurchaseOrderByInvoice(string poNumber) {
            System.Data.DataSet searchableProperties = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchableProperties(System.Globalization.CultureInfo.CurrentUICulture.ToString());
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
            System.Data.DataSet results = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().SearchPurchaseOrders(poCluase, options);

            if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0) {
                // Enumerate the results of the search.
                return Guid.Parse(results.Tables[0].Rows[0].ItemArray[2].ToString());
            } else {
                return null;
            }
        }

		public PurchaseOrder ReadPurchaseOrder(Guid userId, string orderNumber)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 1;
			queryBaskets.SearchCriteria.Model.Properties["OrderNumber"] = orderNumber;

            queryBaskets.QueryOptions.RefreshBasket = false;

            var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
						
			return ((PurchaseOrder)basketResponse.CommerceEntities[0]);
		}

        public PurchaseOrder ReadPurchaseOrderByTrackingNumber(string confirmationNumber) {
            Guid? userId = GetSoldToIdForPurchaseOrderByInvoice(confirmationNumber);

            if (userId.HasValue) {
                var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");
                
                queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.Value.ToString("B");
                queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 1;
                queryBaskets.SearchCriteria.Model.Properties["TrackingNumber"] = confirmationNumber;
                
                queryBaskets.QueryOptions.RefreshBasket = false;

                var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
                queryBaskets.RelatedOperations.Add(queryLineItems);

                var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

                if (response.OperationResponses.Count == 0)
                    return null;

                CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

                return ((PurchaseOrder)basketResponse.CommerceEntities[0]);
            } else {
                return null;
            }
        }

		public List<PurchaseOrder> ReadPurchaseOrders(Guid userId, string customerId)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 1;
			queryBaskets.SearchCriteria.Model.Properties["CustomerId"] = customerId;

			queryBaskets.QueryOptions.RefreshBasket = false;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

            return basketResponse.CommerceEntities.Cast<CommerceEntity>().Where(c => c.Properties["CustomerId"] != null && 
                                                                                     c.Properties["CustomerId"].ToString().Equals(customerId)
                                                                                ).Select(p => (PurchaseOrder)p).ToList();
            //return basketResponse.CommerceEntities.Cast<CommerceEntity>().Select(p => (PurchaseOrder)p).ToList();
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
