using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using KeithLink.Svc.Core.Models.Confirmations;
using KeithLink.Common.Core.Parsing;


namespace KeithLink.Svc.Core.Extensions
{
    public static class ConfirmationHeaderExtension
    {
        public static void Parse(this ConfirmationHeader value, string Line)
        {
            value.ConfirmationDate = DateTime.ParseExact(StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_DATE_INDEX, 
                Constants.CONFIRMATION_HEADER_DATE_LENGTH, 
                Line), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            value.Branch = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_DIVISION_INDEX, 
                Constants.CONFIRMATION_HEADER_DIVISION_LENGTH, 
                Line);

            value.CustomerNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_CUSTOMER_NUMBER_INDEX, 
                Constants.CONFIRMATION_HEADER_CUSTOMER_NUMBER_LENGTH, 
                Line);

            value.UserId = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_USER_ID_INDEX, 
                Constants.CONFIRMATION_HEADER_USER_ID_LENGTH, 
                Line);

            value.RemoteOrderNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_PN_ORDER_NUMBER_INDEX, 
                Constants.CONFIRMATION_HEADER_PN_ORDER_NUMBER_LENGTH, 
                Line);

            value.ConfirmationNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_CONFIRMATION_NUMBER_INDEX, 
                Constants.CONFIRMATION_HEADER_CONFIRMATION_NUMBER_LENGTH, 
                Line);

            value.InvoiceNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_INVOICE_NUMBER_INDEX, 
                Constants.CONFIRMATION_HEADER_INVOICE_NUMBER_LENGTH, 
                Line);

            value.ShipDate = DateTime.ParseExact(StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_SHIP_DATE_INDEX, 
                Constants.CONFIRMATION_HEADER_SHIP_DATE_LENGTH, 
                Line), "yyyyMMdd", CultureInfo.InvariantCulture);

            value.RouteNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_ROUTE_NUMBER_INDEX, 
                Constants.CONFIRMATION_HEADER_ROUTE_NUMBER_LENGTH, 
                Line);

            value.StopNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_STOP_NUMBER_INDEX, 
                Constants.CONFIRMATION_HEADER_STOP_NUMBER_LENGTH, 
                Line);

            value.SpecialInstructions = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_INDEX, 
                Constants.CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_LENGTH, 
                Line);

            value.SpecialInstructionsExtended = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_INDEX, 
                Constants.CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_LENGTH, 
                Line);

            value.TotalQuantityOrdered = int.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_INDEX, 
                Constants.CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_LENGTH, 
                Line));

            value.TotalQuantityShipped = int.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_INDEX, 
                Constants.CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_LENGTH, 
                Line));

            value.TotalInvoice = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_TOTAL_INVOICE_INDEX, 
                Constants.CONFIRMATION_HEADER_TOTAL_INVOICE_LENGTH, 
                Line));

            value.TotalCube = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_TOTAL_CUBE_INDEX, 
                Constants.CONFIRMATION_HEADER_TOTAL_CUBE_LENGTH, 
                Line));

            value.TotalWeight = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_TOTAL_WEIGHT_INDEX, 
                Constants.CONFIRMATION_HEADER_TOTAL_WEIGHT_LENGTH, 
                Line));

            value.ConfirmationMessage = StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_INDEX, 
                Constants.CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_LENGTH, 
                Line);

            value.ConfirmationStatus = StringHelpers.GetField(
                Constants.CONFIRMATION_HEADER_CONFIRMATION_STATUS_INDEX, 
                Constants.CONFIRMATION_HEADER_CONFIRMATION_STATUS_LENGTH, 
                Line);
        }
    }
}
