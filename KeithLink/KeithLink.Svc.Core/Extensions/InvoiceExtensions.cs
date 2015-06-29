using KeithLink.Svc.Core.Enumerations;
using KeithLink.Common.Core.Helpers;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Invoices;
using EFInvoice = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions
{
	public static class InvoiceExtensions
	{
		public static InvoiceModel ToInvoiceModel(this EFInvoice.Invoice value, KeithLink.Svc.Core.Models.Profile.Customer customer = null) {
            InvoiceModel retVal = new InvoiceModel();

            retVal.BranchId = value.Division.Substring(0, 3);
			retVal.InvoiceNumber = value.InvoiceNumber.Trim();
			retVal.Type = DetermineType(value.InvoiceType.Trim());
			retVal.TypeDescription = EnumUtils<InvoiceType>.GetDescription(DetermineType(value.InvoiceType.Trim()));
			retVal.Status = value.DetermineStatus();
			retVal.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(value.DetermineStatus());
			retVal.CustomerNumber = value.CustomerNumber;
			retVal.CustomerName = customer == null ? "N/A" : customer.CustomerName;
			retVal.Amount = value.AmountDue;
			retVal.DueDate = value.DueDate;
			retVal.InvoiceDate = value.InvoiceDate;
			retVal.OrderDate = value.InvoiceDate;
            retVal.IsPayable = value.InvoiceStatus.Equals("O", StringComparison.InvariantCultureIgnoreCase)  && customer != null && customer.KPayCustomer;

            return retVal;
		}

		public static InvoiceStatus DetermineStatus(this EFInvoice.Invoice value)
		{
			if (value.InvoiceType.Trim().Equals("cm", StringComparison.InvariantCultureIgnoreCase))
				return InvoiceStatus.Open;

			switch (value.InvoiceStatus.ToUpper())
			{
				case "O":
					if (value.DueDate >= DateTime.Now)
						return InvoiceStatus.Open;
					else
						return InvoiceStatus.PastDue;
				case "P":
					return InvoiceStatus.Pending;
				default:
					return InvoiceStatus.Paid;
			}
		}

		public static InvoiceTransactionModel ToTransationModel(this EFInvoice.Invoice value)
		{
			return new InvoiceTransactionModel()
			{
				BranchId = value.Division.Substring(0, 3),
				InvoiceNumber = value.InvoiceNumber.Trim(),
				Type = DetermineType(value.InvoiceType.Trim()),
				TypeDescription = EnumUtils<InvoiceType>.GetDescription(DetermineType(value.InvoiceType.Trim())),
				CustomerNumber = value.CustomerNumber,
				Amount = value.AmountDue,
				DueDate = value.DueDate,
				InvoiceDate = value.InvoiceDate,
				OrderDate = value.InvoiceDate
			};
		}

		public static InvoiceType DetermineType(string invoiceType)
		{
			switch (invoiceType)
			{
				case "WO":
					return InvoiceType.WriteOff;
				case "BOI":
					return InvoiceType.BillingOnlyInvoice;
				case "OA":
					return InvoiceType.OnAccount;
				case "WOC":
					return InvoiceType.WriteOffCredit;
				case "CM":
					return InvoiceType.CreditMemo;
				case "DM":
					return InvoiceType.DebitMemo;
				case "PY":
					return InvoiceType.Payment;
				case "MT":
					return InvoiceType.Maintenance;
				default:
					return InvoiceType.Invoice;
			}
		}

		public static Invoice ToEFInvoice(this InvoiceModel invoice)
		{
			return new KeithLink.Svc.Core.Models.EF.Invoice(){
				CustomerNumber = invoice.CustomerNumber,
				InvoiceNumber = invoice.InvoiceNumber,
				OrderDate = invoice.OrderDate,
				Id = invoice.Id,
				Type = invoice.Type,
				Amount = invoice.Amount,
				BranchId = invoice.BranchId,
				InvoiceDate = invoice.InvoiceDate,
				Items = invoice.Items == null ? null : invoice.Items.Select(i =>
					new KeithLink.Svc.Core.Models.EF.InvoiceItem(){
						CatchWeightCode = i.CatchWeightCode,
						ExtCatchWeight = i.ExtCatchWeight,
						ExtSalesNet = i.ExtSalesNet,
						ItemPrice = i.ItemPrice,
						QuantityOrdered = i.QuantityOrdered,
						QuantityShipped = i.QuantityShipped,
						ClassCode = i.ClassCode,
						Id = i.Id,
						InvoiceNumber = i.InvoiceNumber,
						ItemNumber = i.ItemNumber,
						LineNumber = i.LineNumber
					}).ToList()
			};
		}

		[Obsolete]
		public static InvoiceModel ToInvoiceModel(this Invoice invoice, bool headerOnly = false)
		{
			
			return new InvoiceModel()
			{
				CustomerNumber = invoice.CustomerNumber,
				InvoiceNumber = invoice.InvoiceNumber,
				OrderDate = invoice.OrderDate,
				Type = invoice.Type,
				InvoiceDate = invoice.InvoiceDate,
				Id = invoice.Id,
				Amount = invoice.Amount,
				DueDate = invoice.DueDate,
				Status = invoice.Status,
				TypeDescription = EnumUtils<InvoiceType>.GetDescription(invoice.Type, ""),
				Items = headerOnly ? new List<InvoiceItemModel>() : invoice.Items == null ? null : invoice.Items.Select(i => new InvoiceItemModel() {
					CatchWeightCode = i.CatchWeightCode,
					ExtCatchWeight = i.ExtCatchWeight.HasValue ? i.ExtCatchWeight.Value : 0,
					ExtSalesNet = i.ExtSalesNet.HasValue ? i.ExtSalesNet.Value : 0,
					ItemPrice = i.ItemPrice.HasValue ? i.ItemPrice.Value : 0,
					QuantityOrdered = i.QuantityOrdered.HasValue ? i.QuantityOrdered.Value : 0,
					QuantityShipped = i.QuantityShipped.HasValue ? i.QuantityShipped.Value : 0,
					ItemNumber = i.ItemNumber,
					ClassCode = i.ClassCode,
					InvoiceNumber = i.InvoiceNumber,
					Id = i.Id,
					LineNumber = i.LineNumber
				}).ToList()
			};
		}

		[Obsolete]
		public static InvoiceItem ToEFInvoiceItem(this InvoiceItemModel item)
		{
			return new KeithLink.Svc.Core.Models.EF.InvoiceItem()
			{
				Id = item.Id,
				CatchWeightCode = item.CatchWeightCode,
				ExtCatchWeight = item.ExtCatchWeight,
				ExtSalesNet = item.ExtSalesNet,
				ItemPrice = item.ItemPrice,
				QuantityOrdered = item.QuantityOrdered,
				QuantityShipped = item.QuantityShipped,
				InvoiceNumber = item.InvoiceNumber,
				ItemNumber = item.ItemNumber,
				ClassCode = item.ClassCode,
				LineNumber = item.LineNumber
			};
		}
	}
}
