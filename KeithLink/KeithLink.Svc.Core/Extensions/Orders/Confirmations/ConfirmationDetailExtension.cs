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
    public static class ConfirmationDetailExtension
    {
        public static void Parse(this ConfirmationDetail value, string Line)
        {
            value.RecordNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_RECORD_NUMBER_INDEX, 
                Constants.CONFIRMATION_DETAIL_RECORD_NUMBER_LENGTH, 
                Line);

            value.ItemNumber = StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_ITEM_NUMBER_INDEX, 
                Constants.CONFIRMATION_DETAIL_ITEM_NUMBER_LENGTH, 
                Line);

            value.QuantityOrdered = int.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_QUANTITY_ORDERED_INDEX, 
                Constants.CONFIRMATION_DETAIL_QUANTITY_ORDERED_LENGTH, 
                Line));

            value.BrokenCase = StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_BROKEN_CASE_INDEX, 
                Constants.CONFIRMATION_DETAIL_BROKEN_CASE_LENGTH, 
                Line);

            value.QuantityShipped = int.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_QUANTITY_SHIPPED_INDEX, 
                Constants.CONFIRMATION_DETAIL_QUANTITY_SHIPPED_LENGTH, 
                Line));

            value.ReasonNotShipped = StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_INDEX, 
                Constants.CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_LENGTH, 
                Line);

            value.ShipWeight = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_SHIP_WEIGHT_INDEX, 
                Constants.CONFIRMATION_DETAIL_SHIP_WEIGHT_LENGTH, 
                Line));

            value.CaseCube = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_CASE_CUBE_INDEX, 
                Constants.CONFIRMATION_DETAIL_CASE_CUBE_LENGTH, Line));

            value.CaseWeight = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_CASE_WEIGHT_INDEX, 
                Constants.CONFIRMATION_DETAIL_CASE_WEIGHT_LENGTH, 
                Line));

            value.SalesGross = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_SALES_GROSS_INDEX, 
                Constants.CONFIRMATION_DETAIL_SALES_GROSS_LENGTH, 
                Line));

            value.SalesNet = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_SALES_NET_INDEX, 
                Constants.CONFIRMATION_DETAIL_SALES_NET_LENGTH, 
                Line));

            value.PriceNet = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_PRICE_NET_INDEX, 
                Constants.CONFIRMATION_DETAIL_PRICE_NET_LENGTH, 
                Line));

            value.SplitPriceNet = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_SPLIT_PRICE_NET_INDEX, 
                Constants.CONFIRMATION_DETAIL_SPLIT_PRICE_NET_LENGTH, 
                Line));
            
            value.PriceGross = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_PRICE_GROSS_INDEX, 
                Constants.CONFIRMATION_DETAIL_PRICE_GROSS_LENGTH, 
                Line));

            value.SplitPriceGross = double.Parse(StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_INDEX, 
                Constants.CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_LENGTH, 
                Line));

            value.ConfirmationMessage = StringHelpers.GetField(
                Constants.CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_INDEX, 
                Constants.CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_LENGTH, 
                Line);
        }

        
    }
}
