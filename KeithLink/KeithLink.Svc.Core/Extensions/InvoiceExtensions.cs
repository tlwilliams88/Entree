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
						LineNumber = i.LineNumber,
						PriceBook = i.PriceBook,
						PriceBookNumber = i.PriceBookNumber,
						QuantityOrdered = i.QuantityOrdered,
						QuantityShipped = i.QuantityShipped,
						VendorNumber = i.VendorNumber
					}).ToArray()
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
				IsKeithNet = invoice.Type == InvoiceType.KeithNet,
				IsKeithPay = invoice.Type == InvoiceType.KeithPay,
				MemoBillCode = invoice.MemoBillCode,
				OrderDate = invoice.OrderDate,
				OrderNumber = invoice.OrderNumber,
				RouteNumber = invoice.RouteNumber,
				SalesRep = invoice.SalesRep,
				ShipDate = invoice.ShipDate,
				StopNumber = invoice.StopNumber,
				TradeSWFlag = invoice.TradeSWFlag,
				WHNumber = invoice.WHNumber,
				Items = invoice.Items == null ? null : invoice.Items.Select(i => new InvoiceItemModel() {
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
					LineNumber = i.LineNumber,
					PriceBook = i.PriceBook,
					PriceBookNumber = i.PriceBookNumber,
					QuantityOrdered = i.QuantityOrdered,
					QuantityShipped = i.QuantityShipped,
					VendorNumber = i.VendorNumber
				}).ToList()
			};
		}
	}
}
