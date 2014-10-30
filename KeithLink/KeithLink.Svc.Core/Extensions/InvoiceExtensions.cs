using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions
{
	public static class InvoiceExtensions
	{
		public static Invoice ToEFInvoice(this InvoiceModel invoice)
		{
			return new KeithLink.Svc.Core.Models.EF.Invoice(){
				ChainStoreCode = invoice.ChainStoreCode,
				Company = invoice.Company,
				CreditHoldFlag = invoice.CreditHoldFlag,
				CustomerGroup = invoice.CustomerGroup,
				CustomerNumber = invoice.CustomerNumber,
				DateTimeOfLastOrder = invoice.DateTimeOfLastOrder,
				Department = invoice.Department,
				Division = invoice.Division,
				DueDate = invoice.DueDate,
				InvoiceNumber = invoice.InvoiceNumber,
				MemoBillCode = invoice.MemoBillCode,
				OrderDate = invoice.OrderDate,
				OrderNumber = invoice.OrderNumber,
				RouteNumber = invoice.RouteNumber,
				SalesRep = invoice.SalesRep,
				ShipDate = invoice.ShipDate,
				StopNumber = invoice.StopNumber,
				TradeSWFlag = invoice.TradeSWFlag,
				Type = invoice.InvoiceType,
				WHNumber = invoice.WHNumber,
				Items = invoice.Items == null ? null : invoice.Items.Select(i =>
					new KeithLink.Svc.Core.Models.EF.InvoiceItem(){
						AmountDue = i.AmountDue,
						BrokenCaseCode = i.BrokenCaseCode,
						CatchWeightCode = i.CatchWeightCode,
						CombinedStatmentCustomer = i.CombinedStatmentCustomer,
						CustomerPO = i.CustomerPO,
						DeleteFlag = i.DeleteFlag,
						ExtCatchWeight = i.ExtCatchWeight,
						ExtSalesGross = i.ExtSalesGross,
						ExtSalesNet = i.ExtSalesNet,
						ExtSalesRepAmount = i.ExtSalesRepAmount,
						InvoiceDate = i.InvoiceDate,
						InvoiceType = i.InvoiceType,
						ItemPrice = i.ItemPrice,
						ItemPriceSalesRep = i.ItemPriceSalesRep,
						LineItem = i.LineItem,
						PriceBook = i.PriceBook,
						PriceBookNumber = i.PriceBookNumber,
						QuantityOrdered = i.QuantityOrdered,
						QuantityShipped = i.QuantityShipped,
						VendorNumber = i.VendorNumber
					}).ToList()
			};
		}

		public static InvoiceModel ToInvoiceModel(this Invoice invoice)
		{
			return new InvoiceModel()
			{
				ChainStoreCode = invoice.ChainStoreCode,
				Company = invoice.Company,
				CreditHoldFlag = invoice.CreditHoldFlag,
				CustomerGroup = invoice.CustomerGroup,
				CustomerNumber = invoice.CustomerNumber,
				DateTimeOfLastOrder = invoice.DateTimeOfLastOrder,
				Department = invoice.Department,
				Division = invoice.Division,
				DueDate = invoice.DueDate,
				InvoiceId =invoice.Id,
				InvoiceNumber = invoice.InvoiceNumber,
				MemoBillCode = invoice.MemoBillCode,
				OrderDate = invoice.OrderDate,
				OrderNumber = invoice.OrderNumber.HasValue ? invoice.OrderNumber.Value : 0,
				RouteNumber = invoice.RouteNumber.HasValue ? invoice.RouteNumber.Value : 0,
				SalesRep = invoice.SalesRep,
				ShipDate = invoice.ShipDate,
				StopNumber = invoice.StopNumber.HasValue ? invoice.StopNumber.Value : 0,
				TradeSWFlag = invoice.TradeSWFlag,
				WHNumber = invoice.WHNumber,
				Items = invoice.Items == null ? null : invoice.Items.Select(i => new InvoiceItemModel() {
					AmountDue = i.AmountDue.HasValue ? i.AmountDue.Value : 0,
					BrokenCaseCode = i.BrokenCaseCode,
					CatchWeightCode = i.CatchWeightCode,
					CombinedStatmentCustomer = i.CombinedStatmentCustomer,
					CustomerPO = i.CustomerPO,
					DeleteFlag = i.DeleteFlag.HasValue ? i.DeleteFlag.Value : 0,
					ExtCatchWeight = i.ExtCatchWeight.HasValue ? i.ExtCatchWeight.Value : 0,
					ExtSalesGross = i.ExtSalesGross.HasValue ? i.ExtSalesGross.Value: 0,
					ExtSalesNet = i.ExtSalesNet.HasValue ? i.ExtSalesNet.Value : 0,
					ExtSalesRepAmount = i.ExtSalesRepAmount.HasValue ? i.ExtSalesRepAmount.Value : 0,
					InvoiceDate = i.InvoiceDate,
					InvoiceType = i.InvoiceType,
					ItemPrice = i.ItemPrice.HasValue ? i.ItemPrice.Value : 0,
					ItemPriceSalesRep = i.ItemPriceSalesRep.HasValue ? i.ItemPriceSalesRep.Value : 0,
					LineItem = i.LineItem,
					PriceBook = i.PriceBook,
					PriceBookNumber = i.PriceBookNumber,
					QuantityOrdered = i.QuantityOrdered.HasValue ? i.QuantityOrdered.Value : 0,
					QuantityShipped = i.QuantityShipped.HasValue ? i.QuantityShipped.Value : 0,
					VendorNumber = i.VendorNumber.HasValue ? i.VendorNumber.Value : 0
				}).ToList()
			};
		}

		public static InvoiceItem ToEFInvoiceItem(this InvoiceItemModel item)
		{
			return new KeithLink.Svc.Core.Models.EF.InvoiceItem()
			{
				AmountDue = item.AmountDue,
				BrokenCaseCode = item.BrokenCaseCode,
				CatchWeightCode = item.CatchWeightCode,
				CombinedStatmentCustomer = item.CombinedStatmentCustomer,
				CustomerPO = item.CustomerPO,
				DeleteFlag = item.DeleteFlag,
				ExtCatchWeight = item.ExtCatchWeight,
				ExtSalesGross = item.ExtSalesGross,
				ExtSalesNet = item.ExtSalesNet,
				ExtSalesRepAmount = item.ExtSalesRepAmount,
				InvoiceDate = item.InvoiceDate,
				InvoiceType = item.InvoiceType,
				ItemPrice = item.ItemPrice,
				ItemPriceSalesRep = item.ItemPriceSalesRep,
				LineItem = item.LineItem,
				PriceBook = item.PriceBook,
				PriceBookNumber = item.PriceBookNumber,
				QuantityOrdered = item.QuantityOrdered,
				QuantityShipped = item.QuantityShipped,
				VendorNumber = item.VendorNumber,
				InvoiceNumber = item.InvoiceNumber
			};
		}
	}
}
