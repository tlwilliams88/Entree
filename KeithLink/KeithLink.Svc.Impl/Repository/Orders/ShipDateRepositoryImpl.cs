using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;
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
        public ShipDateReturn GetShipDates(UserSelectedContext customerInfo) {
            ShipDateReturn retVal = new ShipDateReturn();

            try {
                System.IO.StringWriter requestBody = new System.IO.StringWriter();
                GetRequestBody(customerInfo.BranchId, customerInfo.CustomerId).WriteXml(requestBody);

                com.benekeith.ShipDateService.ShipDateSoapClient shipdayService = new com.benekeith.ShipDateService.ShipDateSoapClient();
                    
                ShippingDateResponseMain response = GetResponse(shipdayService.GetShipDates(requestBody.ToString()));
                ShippingDateResponseMain.CustomerRow customerRow = response.Customer[0];

                foreach (ShippingDateResponseMain.ShipDateRow shipDates in response.ShipDate) {
                    DateTime workDate = DateTime.Parse(shipDates.ShipDate_Column);

                    retVal.ShipDates.Add(new ShipDate() {
                                                            CutOffDateTime = GetCutOffTime(workDate, customerRow.CutOffTime), 
                                                            Date = workDate.ToString("yyyy-MM-dd"),
                                                            DayOfWeek = GetDayOfWeek(workDate)
                                                        });
                }
            } catch {
            }
            
            return retVal;
        }

        private string GetCutOffTime(DateTime currentDate, string cutOffTime) {
            const int TIME_LOCATION_HOUR = 0;
            const int TIME_LOCATION_MINUTE = 1;

            string[] timePieces = cutOffTime.Split(':');
            int hours, mins;
            
            int.TryParse(timePieces[TIME_LOCATION_HOUR], out hours);
            int.TryParse(timePieces[TIME_LOCATION_MINUTE], out mins);

            DateTime outputDate;

            if (hours == 0 && mins == 0) {
                outputDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
            } else {
                DateTime workDate = currentDate.AddDays(-1);

                outputDate = new DateTime(workDate.Year, workDate.Month, workDate.Day, hours, mins, 0);
            }

            return outputDate.ToString("yyyy-MM-dd HH:mm");
        }

        private string GetDayOfWeek(DateTime currentDate){
            switch (currentDate.DayOfWeek) 	{
		        case DayOfWeek.Friday:
                    return "Fri";
                case DayOfWeek.Monday:
                    return "Mon";
                case DayOfWeek.Saturday:
                    return "Sat";
                case DayOfWeek.Sunday:
                    return "Sun";
                case DayOfWeek.Thursday:
                    return "Thu";
                case DayOfWeek.Tuesday:
                    return "Tue";
                case DayOfWeek.Wednesday:
                    return "Wed";
                default:
                    return "???";
	            }
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

            response.ReadXml(responseBody, System.Data.XmlReadMode.IgnoreSchema);

            return response;
        }
        #endregion
    }
}
