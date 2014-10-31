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
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.ETL
{
    public class InvoiceLogicImpl : KeithLink.Svc.Core.ETL.IInvoiceLogic
    {
        private readonly IStagingRepository stagingRepository;
		private readonly IInternalInvoiceLogic internalInvoiceLogic;
		private readonly IEventLogRepository eventLog;


		public InvoiceLogicImpl(IStagingRepository stagingRepository, IInternalInvoiceLogic internalInvoiceLogic, IEventLogRepository eventLog)
        {
            this.stagingRepository = stagingRepository;
			this.internalInvoiceLogic = internalInvoiceLogic;
        }

		public void ImportInvoices()
		{
			DateTime startTime = DateTime.Now;
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

			eventLog.WriteInformationLog(string.Format("ImportInvoices Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
		}

        private InvoiceModel CreateInvoiceModelFromStagedData(DataRow row)
        {
            InvoiceModel invoiceModel = new InvoiceModel()
            {
				CustomerNumber = row.GetString("CustomerNumber"),
				InvoiceNumber = row.GetString("InvoiceNumber"),
				ShipDate = row["ShipDate"] == System.DBNull.Value ? new Nullable<DateTime>() : DateTime.ParseExact(row["ShipDate"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None),
				OrderDate = row["OrderDate"] == System.DBNull.Value ? new Nullable<DateTime>() : DateTime.ParseExact(row["OrderDate"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None)
				

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
				CatchWeightCode = row.GetString("CatchWeightCode").Equals("y", StringComparison.CurrentCultureIgnoreCase),
				ExtCatchWeight = row.GetNullableDecimal("ExtCatchWeight"),
				ExtSalesNet = row.GetNullableDecimal("ExtSalesNet"),
				ItemPrice = row.GetNullableDecimal("ItemPrice"),
				QuantityOrdered = row.GetNullableInt("QuantityOrdered"),
				QuantityShipped = row.GetNullableInt("QuantityShipped"),
				InvoiceNumber = row.GetString("InvoiceNumber"),
				ItemNumber = row.GetString("ItemNumber"),
				ClassCode = row.GetString("ClassCode")
			};
			return invoiceItemModel;
		}
        
    }
}
