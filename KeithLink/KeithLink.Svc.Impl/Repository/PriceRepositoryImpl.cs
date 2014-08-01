using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Catalog;

namespace KeithLink.Svc.Impl.Repository
{
    public class PriceRepositoryImpl : IPriceRepository
    {
        /// <summary>
        /// separate items that are cached from non-cached items
        /// </summary>  
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="fullList">the full list from the pricing request</param>
        /// <param name="cachedList">the list of prices that have been cached</param>
        /// <param name="newProductList">the list of products that have not been cached</param>
        /// <remarks>
        /// jwames - 7/28/2014 - original code
        /// </remarks>
        private void BuildCachedPriceList(string branchId, string customerNumber, List<Product> fullList,
                                          out List<Price> cachedList, out List<Product> newProductList)
        {
            cachedList = new List<Price>();
            newProductList = new List<Product>();

            PriceCacheRepositoryImpl cache = new PriceCacheRepositoryImpl();

            foreach (Product currentProduct in fullList)
            {
                Price tempPrice = cache.GetPrice(branchId, customerNumber, currentProduct.ItemNumber);

                if (tempPrice == null)
                {
                    newProductList.Add(currentProduct);
                }
                else
                {
                    cachedList.Add(tempPrice);
                }
            }
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
        /// </remarks>
        public KeithLink.Svc.Core.PriceReturn GetPrices(string branchId, string customerNumber, DateTime shipDate, List<Product> products)
        {
            List<Price> cachedPriceList = null;
            List<Product> uncachedProductList = null;

            BuildCachedPriceList(branchId, customerNumber, products, out cachedPriceList, out uncachedProductList);

            KeithLink.Svc.Core.PriceReturn retVal = new PriceReturn();

            if (uncachedProductList.Count == 0)
            {
                retVal.Prices.AddRange(cachedPriceList);
            }
            else
            {
                // build the request XML
                System.IO.StringWriter requestBody = new System.IO.StringWriter();
                GetRequestBody(branchId, customerNumber, shipDate, uncachedProductList).WriteXml(requestBody);

                // load the pricing service
                com.benekeith.PricingService.PricingSoapClient pricing = new com.benekeith.PricingService.PricingSoapClient();
         
                // call the pricing service and get the response XML
                Schemas.PricingResponseMain pricingResponse = GetResponse(pricing.Calculate(requestBody.ToString()));
            
                // build the output
                retVal.Prices.AddRange(cachedPriceList);

                PriceCacheRepositoryImpl cache = new PriceCacheRepositoryImpl();

                foreach (Schemas.PricingResponseMain._ItemRow item in pricingResponse._Item) {
                    Price itemPrice = new Price();

                    itemPrice.BranchId = branchId;
                    itemPrice.CustomerNumber = customerNumber;
                    itemPrice.ItemNumber = item.number;

                    Schemas.PricingResponseMain.PricesRow[] prices = item.GetPricesRows();

                    itemPrice.CasePrice = (double)prices[0].NetCase;
                    itemPrice.PackagePrice = (double)prices[0].NetEach;

                    retVal.Prices.Add(itemPrice);
                    cache.AddItem(itemPrice);
                }
            }
            
            return retVal;
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
        private KeithLink.Svc.Impl.Schemas.PricingRequestMain GetRequestBody(string branchId, string customerNumber, DateTime shipDate, List<Product> products)
        {
            Schemas.PricingRequestMain request = new Schemas.PricingRequestMain();

            Schemas.PricingRequestMain.PricingRequestRow reqRow = request.PricingRequest.NewPricingRequestRow();
            reqRow.ShipDate = shipDate;
            request.PricingRequest.AddPricingRequestRow(reqRow);

            Schemas.PricingRequestMain.CustomerRow custRow = request.Customer.NewCustomerRow();
            custRow.Company = branchId;
            custRow.Division = branchId;
            custRow.Department = branchId;
            custRow.Number = customerNumber;
            request.Customer.AddCustomerRow(custRow);

            request.Items.AddItemsRow(request.Items.NewItemsRow());

            foreach (Product item in products)
            {
                Schemas.PricingRequestMain._ItemRow itemRow = request._Item.New_ItemRow();
                itemRow.number = item.ItemNumber;
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
        private KeithLink.Svc.Impl.Schemas.PricingResponseMain GetResponse(string RawXml)
        {
            System.IO.StringReader responseBody = new System.IO.StringReader(RawXml);

            Schemas.PricingResponseMain response = new Schemas.PricingResponseMain();

            response.ReadXml(responseBody, System.Data.XmlReadMode.InferSchema);

            return response;
        }

    }
}
