using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Concurrent;
using KeithLink.Svc.Core.ETL;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.Invoices;

namespace KeithLink.Svc.Impl.ETL
{
    public class InvoiceLogicImpl : KeithLink.Svc.Core.ETL.IInvoiceLogic
    {
        private IStagingRepository stagingRepository;
		private IInternalInvoiceLogic internalInvoiceLogic;


        public InvoiceLogicImpl(IStagingRepository stagingRepository, IInternalInvoiceLogic internalInvoiceLogic)
        {
            this.stagingRepository = stagingRepository;
			this.internalInvoiceLogic = internalInvoiceLogic;
        }

		public void ImportInvoices()
		{
			DateTime start = DateTime.Now;
			DataTable invoices = stagingRepository.ReadInvoices();

			var invoicesForImport = new List<InvoiceModel>();
			var invoiceItemsForImport = new List<InvoiceItemModel>();

			var invoiceNumber = "";
			InvoiceModel currInvoice = null;

			internalInvoiceLogic.DeleteAll();

			foreach (var row in invoices.AsEnumerable())
			{
				var currentInvoiceNumber = row.GetString("InvoiceNumber");

				//create header invoice and invoice item
				if (invoiceNumber != currentInvoiceNumber)
				{
					invoiceNumber = currentInvoiceNumber;
					currInvoice = CreateInvoiceModelFromStagedData(row);
					invoicesForImport.Add(currInvoice);
					currInvoice.Items = new List<InvoiceItemModel>();
					invoiceItemsForImport.Add(CreateInvoiceItemModelFromStagedData(row));
				}
					//create just invoice item
				else
				{
					invoiceItemsForImport.Add(CreateInvoiceItemModelFromStagedData(row));
				}
			}

			internalInvoiceLogic.BulkImport(invoicesForImport, invoiceItemsForImport);
		}

        private InvoiceModel CreateInvoiceModelFromStagedData(DataRow row)
        {
            InvoiceModel invoiceModel = new InvoiceModel()
            {
				ChainStoreCode = row.GetString("ChainStoreCode"),
				Company = row.GetString("CompanyNumber"),
				CreditHoldFlag = row.GetString("CreditOFlag"),
				CustomerGroup = row.GetString("CustomerGroup"),
				CustomerNumber = row.GetInt("CustomerNumber"),
				DateTimeOfLastOrder = row["DateOfLastOrder"] == System.DBNull.Value ? new Nullable<DateTime>() : DateTime.ParseExact(row["DateOfLastOrder"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None),
				Department = row.GetString("DepartmentNumber"),
				Division = row.GetString("DivisionNumber"),
				DueDate = row.GetNullableDateTime("DueDate"),
				InvoiceNumber = row.GetString("InvoiceNumber"),
				MemoBillCode = row.GetString("MemoBillCode"),
				OrderDate = row["OrderDate"] == System.DBNull.Value ? new Nullable<DateTime>() : DateTime.ParseExact(row["OrderDate"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None),
				OrderNumber = row.GetNullableInt("OrderNumber"),
				RouteNumber = row.GetNullableInt("RouteNumber"),
				SalesRep = row.GetString("SalesRep"),
				ShipDate = row["ShipDate"] == System.DBNull.Value ? new Nullable<DateTime>() : DateTime.ParseExact(row["ShipDate"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None),
				StopNumber = row.GetNullableInt("StopNumber"),
				TradeSWFlag = row.GetString("TradeSWFlag"),
				InvoiceType = GetTypeFromString(row.GetString("Type")),
				WHNumber = row.GetString("WHNumber")

            };
			return invoiceModel;
        }

		private static InvoiceType GetTypeFromString(string value)
		{
			if (value == "KeithNet")
				return InvoiceType.KeithNet;
			else
				return InvoiceType.KeithPay;
		}

		private InvoiceItemModel CreateInvoiceItemModelFromStagedData(DataRow row)
		{
			InvoiceItemModel invoiceItemModel = new InvoiceItemModel()
			{
				AmountDue = row.GetNullableDecimal("AmountDue"),
				BrokenCaseCode = row.GetString("BrokenCaseCode"),
				CatchWeightCode = row.GetString("CatchWeightCode"),
				CombinedStatmentCustomer = row.GetString("CombStatementCustomer"),
				CustomerPO = row.GetString("CustomerPO"),
				DeleteFlag = row.GetNullableInt("DeleteFlag"),
				ExtCatchWeight = row.GetNullableDecimal("ExtCatchWeight"),
				ExtSalesGross = row.GetNullableDecimal("ExtSalesGross"),
				ExtSalesNet = row.GetNullableDecimal("ExtSalesNet"),
				ExtSalesRepAmount = row.GetNullableDecimal("ExtSRPAmount"),
				InvoiceDate = row.GetNullableDateTime("InvoiceDate"),
				InvoiceType = row.GetString("InvoiceType"),
				ItemPrice = row.GetNullableDecimal("ItemPrice"),
				ItemPriceSalesRep = row.GetNullableDecimal("ItemPriceSRP"),
				LineItem = row.GetInt("LineItem"),
				PriceBook = row.GetString("PriceBook"),
				PriceBookNumber = row.GetString("PriceBookNumber"),
				QuantityOrdered = row.GetNullableInt("QuantityOrdered"),
				QuantityShipped = row.GetNullableInt("QuantityShipped"),
				VendorNumber = row.GetNullableInt("VendorNumber"),
				InvoiceNumber = row.GetString("InvoiceNumber"),
				ItemNumber = row.GetString("ItemNumber")
			};
			return invoiceItemModel;
		}
        
    }
}
