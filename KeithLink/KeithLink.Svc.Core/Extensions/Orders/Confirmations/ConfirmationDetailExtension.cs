using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core;
using KeithLink.Common.Core.Parsing;

namespace KeithLink.Svc.Core.Extensions.Orders.Confirmations
{
    public static class ConfirmationDetailExtension {
        #region attributes

        // Confirmation Parsing Layout : Detail

        public const int CONFIRMATION_DETAIL_RECORD_NUMBER_INDEX = 0;
        public const int CONFIRMATION_DETAIL_RECORD_NUMBER_LENGTH = 5;

        public const int CONFIRMATION_DETAIL_ITEM_NUMBER_INDEX = 5;
        public const int CONFIRMATION_DETAIL_ITEM_NUMBER_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_QUANTITY_ORDERED_INDEX = 15;
        public const int CONFIRMATION_DETAIL_QUANTITY_ORDERED_LENGTH = 7;

        public const int CONFIRMATION_DETAIL_BROKEN_CASE_INDEX = 22;
        public const int CONFIRMATION_DETAIL_BROKEN_CASE_LENGTH = 1;

        public const int CONFIRMATION_DETAIL_QUANTITY_SHIPPED_INDEX = 23;
        public const int CONFIRMATION_DETAIL_QUANTITY_SHIPPED_LENGTH = 7;

        public const int CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_INDEX = 30;
        public const int CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_LENGTH = 3;

        public const int CONFIRMATION_DETAIL_SHIP_WEIGHT_INDEX = 33;
        public const int CONFIRMATION_DETAIL_SHIP_WEIGHT_LENGTH = 12;

        public const int CONFIRMATION_DETAIL_CASE_CUBE_INDEX = 45;
        public const int CONFIRMATION_DETAIL_CASE_CUBE_LENGTH = 12;

        public const int CONFIRMATION_DETAIL_CASE_WEIGHT_INDEX = 57;
        public const int CONFIRMATION_DETAIL_CASE_WEIGHT_LENGTH = 12;

        public const int CONFIRMATION_DETAIL_SALES_GROSS_INDEX = 69;
        public const int CONFIRMATION_DETAIL_SALES_GROSS_LENGTH = 16;

        public const int CONFIRMATION_DETAIL_SALES_NET_INDEX = 85;
        public const int CONFIRMATION_DETAIL_SALES_NET_LENGTH = 16;

        public const int CONFIRMATION_DETAIL_PRICE_NET_INDEX = 101;
        public const int CONFIRMATION_DETAIL_PRICE_NET_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_NET_INDEX = 111;
        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_NET_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_PRICE_GROSS_INDEX = 121;
        public const int CONFIRMATION_DETAIL_PRICE_GROSS_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_INDEX = 131;
        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_INDEX = 141;
        public const int CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_LENGTH = 40;

        #endregion

        #region methods / functions

        public static void Parse(this ConfirmationDetail value, string Line)
        {
            value.RecordNumber = StringHelpers.GetField(
                CONFIRMATION_DETAIL_RECORD_NUMBER_INDEX, 
                CONFIRMATION_DETAIL_RECORD_NUMBER_LENGTH, 
                Line);

            value.ItemNumber = StringHelpers.GetField(
                CONFIRMATION_DETAIL_ITEM_NUMBER_INDEX, 
                CONFIRMATION_DETAIL_ITEM_NUMBER_LENGTH, 
                Line);

            value.QuantityOrdered = int.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_QUANTITY_ORDERED_INDEX, 
                CONFIRMATION_DETAIL_QUANTITY_ORDERED_LENGTH, 
                Line));

            value.BrokenCase = StringHelpers.GetField(
                CONFIRMATION_DETAIL_BROKEN_CASE_INDEX, 
                CONFIRMATION_DETAIL_BROKEN_CASE_LENGTH, 
                Line);

            value.QuantityShipped = int.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_QUANTITY_SHIPPED_INDEX, 
                CONFIRMATION_DETAIL_QUANTITY_SHIPPED_LENGTH, 
                Line));

            value.ReasonNotShipped = StringHelpers.GetField(
                CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_INDEX, 
                CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_LENGTH, 
                Line);

            value.ShipWeight = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_SHIP_WEIGHT_INDEX, 
                CONFIRMATION_DETAIL_SHIP_WEIGHT_LENGTH, 
                Line));

            value.CaseCube = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_CASE_CUBE_INDEX, 
                CONFIRMATION_DETAIL_CASE_CUBE_LENGTH, Line));

            value.CaseWeight = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_CASE_WEIGHT_INDEX, 
                CONFIRMATION_DETAIL_CASE_WEIGHT_LENGTH, 
                Line));

            value.SalesGross = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_SALES_GROSS_INDEX, 
                CONFIRMATION_DETAIL_SALES_GROSS_LENGTH, 
                Line));

            value.SalesNet = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_SALES_NET_INDEX, 
                CONFIRMATION_DETAIL_SALES_NET_LENGTH, 
                Line));

            value.PriceNet = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_PRICE_NET_INDEX, 
                CONFIRMATION_DETAIL_PRICE_NET_LENGTH, 
                Line));

            value.SplitPriceNet = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_SPLIT_PRICE_NET_INDEX, 
                CONFIRMATION_DETAIL_SPLIT_PRICE_NET_LENGTH, 
                Line));
            
            value.PriceGross = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_PRICE_GROSS_INDEX, 
                CONFIRMATION_DETAIL_PRICE_GROSS_LENGTH, 
                Line));

            value.SplitPriceGross = double.Parse(StringHelpers.GetField(
                CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_INDEX, 
                CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_LENGTH, 
                Line));

            value.ConfirmationMessage = StringHelpers.GetField(
                CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_INDEX, 
                CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_LENGTH, 
                Line);
        }

        public static string DisplayStatus(this ConfirmationDetail value) {
            switch (value.ReasonNotShipped.Trim().ToUpper()) {
                case Constants.CONFIRMATION_DETAIL_FILLED_CODE:
                    return Constants.CONFIRMATION_DETAIL_FILLED_STATUS;
                case Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_CODE: // partial ship
                    return Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_STATUS;
                case Constants.CONFIRMATION_DETAIL_OUT_OF_STOCK_CODE: // out of stock
                    return Constants.CONFIRMATION_DETAIL_OUT_OF_STOCK_STATUS;
                case Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_CODE: // item replaced
                    return Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_STATUS;
                case Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_OUT_OF_STOCK_CODE: // item replaced, but replacement currently out of stock
                    return Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_OUT_OF_STOCK_STATUS;
                case Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_REPLACED_CODE: // Item replaced, partial fill
                    return Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_REPLACED_STATUS;
                case Constants.CONFIRMATION_DETAIL_ITEM_SUBBED_CODE: // item subbed
                    return Constants.CONFIRMATION_DETAIL_ITEM_SUBBED_STATUS;
                default:
                    return string.Empty;
            }
        }

        public static string SubstitutedItemNumber(this ConfirmationDetail incomingDetail, CommerceServer.Core.Runtime.Orders.LineItem lineItem) {
            string confirmationStatus = incomingDetail.ReasonNotShipped.Trim().ToUpper();

            if ((incomingDetail.ReasonNotShipped == Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_CODE || 
                incomingDetail.ReasonNotShipped == Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_OUT_OF_STOCK_CODE ||
                incomingDetail.ReasonNotShipped == Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_REPLACED_CODE || 
                incomingDetail.ReasonNotShipped == Constants.CONFIRMATION_DETAIL_ITEM_SUBBED_CODE)
                // check incoming confirmation item number against current item number; 
                // if current item number doesn't match incoming, then move current to substituted
                && !incomingDetail.ItemNumber.Trim().Equals(lineItem.ProductId.Trim(), StringComparison.InvariantCultureIgnoreCase))
                return lineItem.ProductId;

            return string.Empty;
        }
        
        #endregion
    }
}
