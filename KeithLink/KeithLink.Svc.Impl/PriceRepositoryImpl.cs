using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Impl
{
    public class PriceRepositoryImpl : IPriceRepository
    {
        public KeithLink.Svc.Core.PriceReturn GetPrices(string branchId, string customerNumber, DateTime shipDate, List<string> itemNumbers)
        {
            // build the request XML
            System.IO.StringWriter requestBody = new System.IO.StringWriter();
            GetRequestBody(branchId, customerNumber, shipDate, itemNumbers).WriteXml(requestBody);

            // load the pricing service
            com.benekeith.PricingService.PricingSoapClient pricing = new com.benekeith.PricingService.PricingSoapClient();
         
            // call the pricing service and get the response XML
            Schemas.PricingResponseMain pricingResponse = GetResponse(pricing.Calculate(requestBody.ToString()));
            

            // build the output
            KeithLink.Svc.Core.PriceReturn retVal = new PriceReturn();

            foreach (Schemas.PricingResponseMain._ItemRow item in pricingResponse._Item) {
                Price itemPrice = new Price();

                itemPrice.BranchId = branchId;
                itemPrice.CustomerNumber = customerNumber;
                itemPrice.ItemNumber = item.number;

                Schemas.PricingResponseMain.PricesRow[] prices = item.GetPricesRows();

                itemPrice.CasePrice = (double)prices[0].NetCase;
                itemPrice.PackagePrice = (double)prices[0].NetEach;

                retVal.Prices.Add(itemPrice);
            }
            
            return retVal;
        }

        private KeithLink.Svc.Impl.Schemas.PricingRequestMain GetRequestBody(string branchId, string customerNumber, DateTime shipDate, List<string> itemNumbers)
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

            foreach (string item in itemNumbers)
            {
                Schemas.PricingRequestMain._ItemRow itemRow = request._Item.New_ItemRow();
                itemRow.number = item;
                request._Item.Add_ItemRow(itemRow);
            }

            return request;
        }

        private KeithLink.Svc.Impl.Schemas.PricingResponseMain GetResponse(string RawXml)
        {
            System.IO.StringReader responseBody = new System.IO.StringReader(RawXml);

            Schemas.PricingResponseMain response = new Schemas.PricingResponseMain();

            response.ReadXml(responseBody, System.Data.XmlReadMode.InferSchema);

            return response;
        }
    }
}
