using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Models.SiteCatalog.Schemas;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class PriceRepositoryImpl : IPriceRepository
    {

        public List<Price> GetNonBekItemPrices(string branchId, string customerNumber, DateTime shipDate, string source, List<Product> products) {
            if (products == null || products.Count == 0)
                return null;

            List<Price> prices = new List<Price>();

            if (customerNumber == null) {
                foreach (Product item in products) {
                    prices.Add(new Price() {
                        BranchId = branchId,
                        CustomerNumber = customerNumber,
                        ItemNumber = item.ItemNumber,
                        CasePrice = 0.00,
                        PackagePrice = 0.00
                    });
                }

            } else {
                // build the request XML
                System.IO.StringWriter requestBody = new System.IO.StringWriter();
                GetNonBekItemRequestBody(branchId, customerNumber, shipDate, source, products).WriteXml(requestBody);

                // load the pricing service
                com.benekeith.PricingService.PricingSoapClient pricing = new com.benekeith.PricingService.PricingSoapClient();

                // call the pricing service and get the response XML
                PricingResponseMain pricingResponse = GetResponse(pricing.CalculatePricingForNonBekItems(requestBody.ToString()));
                foreach (PricingResponseMain.ItemRow item in pricingResponse.Item) {
                    Price itemPrice = new Price();

                    itemPrice.BranchId = branchId;
                    itemPrice.CustomerNumber = customerNumber;
                    itemPrice.ItemNumber = item.number;
                    itemPrice.DeviatedCost = item.DeviatedCost;

                    PricingResponseMain.PricesRow[] priceRows = item.GetPricesRows();

                    itemPrice.CasePrice = (double)priceRows[0].NetCase;
                    itemPrice.PackagePrice = (double)priceRows[0].NetEach;

                    prices.Add(itemPrice);
                }
            }


            return prices;
        }

        private NonBekItemPricingRequest GetNonBekItemRequestBody(string branchId, string customerNumber, DateTime shipDate, string source, List<Product> products) {
            NonBekItemPricingRequest request = new NonBekItemPricingRequest();

            NonBekItemPricingRequest.PricingRequestRow reqRow = request.PricingRequest.NewPricingRequestRow();
            reqRow.BranchId = branchId;
            reqRow.CustomerNumber = customerNumber;
            reqRow.ShipDate = shipDate;
            request.PricingRequest.AddPricingRequestRow(reqRow);

            request.Items.AddItemsRow(request.Items.NewItemsRow());

            foreach (Product item in products) {
                NonBekItemPricingRequest._ItemRow itemRow = request._Item.New_ItemRow();
                itemRow.Source = source;
                itemRow.Category = item.CategoryName;
                itemRow.Number = item.ItemNumber;
                itemRow.IsCatchWeight = item.CatchWeight;
                itemRow.CaseCost = (decimal)item.CasePriceNumeric;
                itemRow.PackageCost = (decimal)item.PackagePriceNumeric;
                request._Item.Add_ItemRow(itemRow);
            }

            return request;
        }

        /// <summary>
        /// return the prices for the requested items
        /// </summary>
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="shipDate">the date to use for pricing</param>
        /// <param name="products">the list of items to price</param>
        /// <returns>PriceReturn with completed prices</returns>
        /// <remarks>
        /// jwames - 7/28/2014 - add pricing cache calls
        /// jwames - 9/25/2014 - test for empty customer number
        /// </remarks>
        public List<Price> GetPrices(string branchId, string customerNumber, DateTime shipDate, List<Product> products)
        {
			if (products == null || products.Count == 0)
				return null;

            List<Price> prices = new List<Price>();

            if (customerNumber == null) {
                foreach (Product item in products) {
                    prices.Add(new Price() { 
                        BranchId = branchId,
                        CustomerNumber = customerNumber,
                        ItemNumber = item.ItemNumber,
                        CasePrice = 0.00,
                        PackagePrice = 0.00
                    });
                }

            } else {
                // build the request XML
                System.IO.StringWriter requestBody = new System.IO.StringWriter();
                GetRequestBody(branchId, customerNumber, shipDate, products).WriteXml(requestBody);

                // load the pricing service
                com.benekeith.PricingService.PricingSoapClient pricing = new com.benekeith.PricingService.PricingSoapClient();

                // call the pricing service and get the response XML
				PricingResponseMain pricingResponse = GetResponse(pricing.Calculate(requestBody.ToString()));
				foreach (PricingResponseMain.ItemRow item in pricingResponse.Item) {
                    Price itemPrice = new Price();

                    itemPrice.BranchId = branchId;
                    itemPrice.CustomerNumber = customerNumber;
                    itemPrice.ItemNumber = item.number;
                    itemPrice.DeviatedCost = item.DeviatedCost;

                    PricingResponseMain.PricesRow[] priceRows = item.GetPricesRows();

                    itemPrice.CasePrice = (double)priceRows[0].NetCase;
                    itemPrice.PackagePrice = (double)priceRows[0].NetEach;

                    prices.Add(itemPrice);
                }           
            }

			 return prices;
        }

        /// <summary>
        /// build the xml request body that is sent to the pricing service
        /// </summary>
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="shipDate">the date to use for pricing</param>
        /// <param name="products">the list of items to price</param>
        /// <returns>PricingRequestMain data set</returns>
        /// <remarks>
        /// jwames - 7/23/2014 - original code
        /// </remarks>
        private PricingRequestMain GetRequestBody(string branchId, string customerNumber, DateTime shipDate, List<Product> products)
        {
            PricingRequestMain request = new PricingRequestMain();

            PricingRequestMain.PricingRequestRow reqRow = request.PricingRequest.NewPricingRequestRow();
            reqRow.ShipDate = shipDate;
            request.PricingRequest.AddPricingRequestRow(reqRow);

            PricingRequestMain.CustomerRow custRow = request.Customer.NewCustomerRow();
            custRow.Company = branchId;
            custRow.Division = branchId;
            custRow.Department = branchId;
            custRow.Number = customerNumber;
            request.Customer.AddCustomerRow(custRow);

            PricingRequestMain.ItemsRow items = request.Items.NewItemsRow();
            request.Items.AddItemsRow(items);

            foreach (Product item in products)
            {
                PricingRequestMain._ItemRow itemRow = request._Item.New_ItemRow();
                itemRow.number = item.ItemNumber;

                string source;
                if (item.CatalogId != null)
                {
                    source = GetSourceCatalog(item.CatalogId);
                }
                else
                {
                    source = GetSourceCatalog(branchId);
                }
                if (item.IsValid && !source.Equals(Constants.CATALOG_BEK))
                {
                    if (!item.IsSpecialtyCatalog && item.Unfi.StockedInBranches.IndexOf(branchId, StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                    //if (!item.IsSpecialtyCatalog && item.Brand.IndexOf(branchId, StringComparison.InvariantCultureIgnoreCase)> 0) {
                        source = Constants.CATALOG_BEK;
                    }
                }

                itemRow.Source = source;

                if (!source.Equals(Constants.CATALOG_BEK)) {
                    itemRow.Category = item.CategoryName;
                    itemRow.IsCatchWeight = item.CatchWeight;
                    if (item.Unfi == null) {
                        itemRow.CaseCost = (decimal)item.CasePriceNumeric;
                        itemRow.PackageCost = (decimal)item.PackagePriceNumeric;
                    } else {
                        itemRow.CaseCost = decimal.Parse(item.Unfi.CasePrice);
                        itemRow.PackageCost = decimal.Parse(item.Unfi.PackagePrice);
                    }
                }

                itemRow.SetParentRow(items);
                request._Item.Add_ItemRow(itemRow);
            }

            return request;
        }

        /// <summary>
        /// get the response from the service and format it into a xml response body
        /// </summary>
        /// <param name="RawXml">the xml file sent from the pricing service</param>
        /// <returns>PricingResponseMain data set</returns>
        /// <remarks>
        /// jwames - 7/23/2014 - original code
        /// </remarks>
        private PricingResponseMain GetResponse(string RawXml)
        {
            System.IO.StringReader responseBody = new System.IO.StringReader(RawXml);

            PricingResponseMain response = new PricingResponseMain();

            response.ReadXml(responseBody, System.Data.XmlReadMode.InferSchema);

            return response;
        }

        private string GetSourceCatalog(string catalogId) {
            switch (catalogId.ToLower()){
                case "fam":
                case "faq":
                case "far":
                case "fdf":
                case "fhs":
                case "flr":
                case "fok":
                case "fsa":
                    return Constants.CATALOG_BEK;
                case "unfi_5":
                case "unfi_7":
                    return Constants.CATALOG_UNFI;
                default:
                    return null;
            }
        }
    }
}
