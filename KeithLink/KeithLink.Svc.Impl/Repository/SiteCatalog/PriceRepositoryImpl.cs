using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class PriceRepositoryImpl : IPriceRepository
    {
        public PriceRepositoryImpl()
        {
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
                KeithLink.Svc.Core.Models.SiteCatalog.PriceReturn retVal = new PriceReturn();
                System.IO.StringWriter requestBody = new System.IO.StringWriter();
                GetRequestBody(branchId, customerNumber, shipDate, products).WriteXml(requestBody);

                // load the pricing service
                com.benekeith.PricingService.PricingSoapClient pricing = new com.benekeith.PricingService.PricingSoapClient();

                // call the pricing service and get the response XML
                Schemas.PricingResponseMain pricingResponse = GetResponse(pricing.Calculate(requestBody.ToString()));

                foreach (Schemas.PricingResponseMain._ItemRow item in pricingResponse._Item) {
                    Price itemPrice = new Price();

                    itemPrice.BranchId = branchId;
                    itemPrice.CustomerNumber = customerNumber;
                    itemPrice.ItemNumber = item.number;

                    Schemas.PricingResponseMain.PricesRow[] priceRows = item.GetPricesRows();

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
