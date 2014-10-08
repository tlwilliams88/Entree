using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Models.Orders.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders {
    public class ShipDateRepositoryImpl : IShipDateRepository {
        #region attributes
        private const int MAX_SHIP_DAYS = 7;
        #endregion

        #region methods
        public ShipDateReturn GetShipDates(string branchId, string customerNumber) {
            ShipDateReturn retVal = new ShipDateReturn();

            try {
                System.IO.StringWriter requestBody = new System.IO.StringWriter();
                GetRequestBody(branchId, customerNumber).WriteXml(requestBody);

                com.benekeith.ShipDateService.ShipDateSoapClient shipdayService = new com.benekeith.ShipDateService.ShipDateSoapClient();
                    
                ShippingDateResponseMain response = GetResponse(shipdayService.GetShipDates(requestBody.ToString()));
                ShippingDateResponseMain.CustomerRow customerRow = response.Customer[0];

                retVal.CutOffTime = customerRow.CutOffTime;

                foreach (ShippingDateResponseMain.ShipDateRow shipDates in response.ShipDate) {
                    retVal.ShipDays.Add(shipDates.ShipDate_Column);
                }
            } catch {
            }
            
            return retVal;
        }

        /// <summary>
        /// build the xml request to send to the ship date service
        /// </summary>
        /// <param name="branchId">customer's branch</param>
        /// <param name="customerNumber">customer number</param>
        /// <returns>ShippingDateRequestMain schema</returns>
        /// <remarks>
        /// jwames - 10/7/2014 - original code
        /// </remarks>
        private ShippingDateRequestMain GetRequestBody(string branchId, string customerNumber) {
            ShippingDateRequestMain request = new ShippingDateRequestMain();

            ShippingDateRequestMain.CustomerRow custRow = request.Customer.NewCustomerRow();
            custRow.Company = branchId;
            custRow.Division = branchId;
            custRow.Department = branchId;
            custRow.Number = customerNumber;
            request.Customer.AddCustomerRow(custRow);

            ShippingDateRequestMain.ShipDateRequestRow mainRow = request.ShipDateRequest.NewShipDateRequestRow();
            mainRow.NumberOfShipDates = MAX_SHIP_DAYS;
            request.ShipDateRequest.AddShipDateRequestRow(mainRow);

            return request;
        }

        /// <summary>
        /// convert the output to an xml schema
        /// </summary>
        /// <param name="rawXml">the string returned from the service</param>
        /// <returns>ShippingDateResponseMain xml schema</returns>
        /// <remarks>
        /// jwames - 10/7/2014 - original code
        /// </remarks>
        private ShippingDateResponseMain GetResponse(string rawXml) {
            System.IO.StringReader responseBody = new System.IO.StringReader(rawXml);

            ShippingDateResponseMain response = new ShippingDateResponseMain();

            response.ReadXml(responseBody, System.Data.XmlReadMode.InferSchema);

            return response;
        }
        #endregion
    }
}
