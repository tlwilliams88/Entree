using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Common.Core.Parsing;


namespace KeithLink.Svc.Core.Extensions.Orders.Confirmations
{
    public static class ConfirmationHeaderExtension {

        #region attributes

        // Confirmation Parsing Layout : Header
        public const int CONFIRMATION_HEADER_DATE_INDEX = 0;
        public const int CONFIRMATION_HEADER_DATE_LENGTH = 14;

        public const int CONFIFMATION_HEADER_COMPANY_INDEX = 14;
        public const int CONFIRMATION_HEADER_COMPANY_LENGTH = 3;

        public const int CONFIRMATION_HEADER_DIVISION_INDEX = 17;
        public const int CONFIRMATION_HEADER_DIVISION_LENGTH = 3;

        public const int CONFIRMATION_HEADER_DEPARTMENT_INDEX = 20;
        public const int CONFIRMATION_HEADER_DEPARTMENT_LENGTH = 3;

        public const int CONFIRMATION_HEADER_CUSTOMER_NUMBER_INDEX = 23;
        public const int CONFIRMATION_HEADER_CUSTOMER_NUMBER_LENGTH = 10;

        public const int CONFIRMATION_HEADER_USER_ID_INDEX = 33;
        public const int CONFIRMATION_HEADER_USER_ID_LENGTH = 10;

        public const int CONFIRMATION_HEADER_PN_ORDER_NUMBER_INDEX = 43;
        public const int CONFIRMATION_HEADER_PN_ORDER_NUMBER_LENGTH = 7;

        public const int CONFIRMATION_HEADER_CONFIRMATION_NUMBER_INDEX = 50;
        public const int CONFIRMATION_HEADER_CONFIRMATION_NUMBER_LENGTH = 12;

        public const int CONFIRMATION_HEADER_INVOICE_NUMBER_INDEX = 62;
        public const int CONFIRMATION_HEADER_INVOICE_NUMBER_LENGTH = 8;

        public const int CONFIRMATION_HEADER_SHIP_DATE_INDEX = 70;
        public const int CONFIRMATION_HEADER_SHIP_DATE_LENGTH = 8;

        public const int CONFIRMATION_HEADER_ROUTE_NUMBER_INDEX = 78;
        public const int CONFIRMATION_HEADER_ROUTE_NUMBER_LENGTH = 5;

        public const int CONFIRMATION_HEADER_STOP_NUMBER_INDEX = 83;
        public const int CONFIRMATION_HEADER_STOP_NUMBER_LENGTH = 3;

        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_INDEX = 86;
        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_LENGTH = 40;

        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_INDEX = 126;
        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_LENGTH = 40;

        public const int CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_INDEX = 166;
        public const int CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_LENGTH = 7;

        public const int CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_INDEX = 173;
        public const int CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_LENGTH = 7;

        public const int CONFIRMATION_HEADER_TOTAL_INVOICE_INDEX = 180;
        public const int CONFIRMATION_HEADER_TOTAL_INVOICE_LENGTH = 16;

        public const int CONFIRMATION_HEADER_TOTAL_CUBE_INDEX = 196;
        public const int CONFIRMATION_HEADER_TOTAL_CUBE_LENGTH = 12;

        public const int CONFIRMATION_HEADER_TOTAL_WEIGHT_INDEX = 208;
        public const int CONFIRMATION_HEADER_TOTAL_WEIGHT_LENGTH = 12;

        public const int CONFIRMATION_HEADER_CONFIRMATION_MESSAGE_INDEX = 220;
        public const int CONFIRMATION_HEADER_CONFIRMATION_MESSAGE_LENGTH = 32;

        public const int CONFIRMATION_HEADER_CONFIRMATION_STATUS_INDEX = 252;
        public const int CONFIRMATION_HEADER_CONFIRMATION_STATUS_LENGTH = 1;

        #endregion

        public static void Parse(this ConfirmationHeader value, string Line)
        {
            value.ConfirmationDate = DateTime.ParseExact(StringHelpers.GetField(
                CONFIRMATION_HEADER_DATE_INDEX, 
                CONFIRMATION_HEADER_DATE_LENGTH, 
                Line), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            value.Branch = StringHelpers.GetField(
                CONFIRMATION_HEADER_DIVISION_INDEX, 
                CONFIRMATION_HEADER_DIVISION_LENGTH, 
                Line);

            value.CustomerNumber = StringHelpers.GetField(
                CONFIRMATION_HEADER_CUSTOMER_NUMBER_INDEX, 
                CONFIRMATION_HEADER_CUSTOMER_NUMBER_LENGTH, 
                Line);

            value.UserId = StringHelpers.GetField(
                CONFIRMATION_HEADER_USER_ID_INDEX, 
                CONFIRMATION_HEADER_USER_ID_LENGTH, 
                Line);

            value.RemoteOrderNumber = StringHelpers.GetField(
                CONFIRMATION_HEADER_PN_ORDER_NUMBER_INDEX, 
                CONFIRMATION_HEADER_PN_ORDER_NUMBER_LENGTH, 
                Line);

            value.ConfirmationNumber = StringHelpers.GetField(
                CONFIRMATION_HEADER_CONFIRMATION_NUMBER_INDEX, 
                CONFIRMATION_HEADER_CONFIRMATION_NUMBER_LENGTH, 
                Line);

            value.InvoiceNumber = StringHelpers.GetField(
                CONFIRMATION_HEADER_INVOICE_NUMBER_INDEX, 
                CONFIRMATION_HEADER_INVOICE_NUMBER_LENGTH, 
                Line);

            value.ShipDate = DateTime.ParseExact(StringHelpers.GetField(
                CONFIRMATION_HEADER_SHIP_DATE_INDEX, 
                CONFIRMATION_HEADER_SHIP_DATE_LENGTH, 
                Line), "yyyyMMdd", CultureInfo.InvariantCulture);

            value.RouteNumber = StringHelpers.GetField(
                CONFIRMATION_HEADER_ROUTE_NUMBER_INDEX, 
                CONFIRMATION_HEADER_ROUTE_NUMBER_LENGTH, 
                Line);

            value.StopNumber = StringHelpers.GetField(
                CONFIRMATION_HEADER_STOP_NUMBER_INDEX, 
                CONFIRMATION_HEADER_STOP_NUMBER_LENGTH, 
                Line);

            value.SpecialInstructions = StringHelpers.GetField(
                CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_INDEX, 
                CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_LENGTH, 
                Line);

            value.SpecialInstructionsExtended = StringHelpers.GetField(
                CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_INDEX, 
                CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_LENGTH, 
                Line);

            value.TotalQuantityOrdered = int.Parse(StringHelpers.GetField(
                CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_INDEX, 
                CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_LENGTH, 
                Line));

            value.TotalQuantityShipped = int.Parse(StringHelpers.GetField(
                CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_INDEX, 
                CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_LENGTH, 
                Line));

            value.TotalInvoice = double.Parse(StringHelpers.GetField(
                CONFIRMATION_HEADER_TOTAL_INVOICE_INDEX, 
                CONFIRMATION_HEADER_TOTAL_INVOICE_LENGTH, 
                Line));

            value.TotalCube = double.Parse(StringHelpers.GetField(
                CONFIRMATION_HEADER_TOTAL_CUBE_INDEX, 
                CONFIRMATION_HEADER_TOTAL_CUBE_LENGTH, 
                Line));

            value.TotalWeight = double.Parse(StringHelpers.GetField(
                CONFIRMATION_HEADER_TOTAL_WEIGHT_INDEX, 
                CONFIRMATION_HEADER_TOTAL_WEIGHT_LENGTH, 
                Line));

            value.ConfirmationMessage = StringHelpers.GetField(
                CONFIRMATION_HEADER_CONFIRMATION_MESSAGE_INDEX, 
                CONFIRMATION_HEADER_CONFIRMATION_MESSAGE_LENGTH, 
                Line);

            value.ConfirmationStatus = StringHelpers.GetField(
                CONFIRMATION_HEADER_CONFIRMATION_STATUS_INDEX, 
                CONFIRMATION_HEADER_CONFIRMATION_STATUS_LENGTH, 
                Line);
        }
    }
}
