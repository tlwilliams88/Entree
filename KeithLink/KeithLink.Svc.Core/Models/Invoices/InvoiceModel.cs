using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices
{
    [DataContract(Name = "Invoice")]
    public class InvoiceModel : IExportableModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "customername")]
        public string CustomerName { get; set; }

        [DataMember(Name = "invoicenumber")]
        [Description("Reference #")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "invoicedate")]
        [Description("Invoice Date")]
        public DateTime? InvoiceDate { get; set; }

        [DataMember(Name = "invoiceamount")]
        [Description("Invoice Amount")]
        public decimal InvoiceAmount { get; set; }

        [DataMember(Name = "orderdate")]
        public DateTime? OrderDate { get; set; }

        [DataMember(Name = "duedate")]
        [Description("Due Date")]
        public DateTime? DueDate { get; set; }

        [DataMember(Name = "status")]
        public InvoiceStatus Status { get; set; }

        [DataMember(Name = "statusdescription")]
        [Description("Status")]
        public string StatusDescription { get; set; }

        [DataMember(Name = "type")]
        public InvoiceType Type { get; set; }

        [DataMember(Name = "typedescription")]
        [Description("Type")]
        public string TypeDescription { get; set; }

        [DataMember(Name = "amount")]
        [Description("Amount Due")]
        public decimal Amount { get; set; }

        [DataMember(Name = "items")]
        public List<InvoiceItemModel> Items { get; set; }

        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }

        [DataMember(Name = "itemsequence")]
        public int ItemSequence { get; set; }

        [DataMember(Name = "ispayable")]
        public bool IsPayable { get; set; }

        [DataMember(Name = "hascreditmemos")]
        public bool HasCreditMemos { get; set; }

        [DataMember(Name = "invoicelink")]
        public Uri InvoiceLink { get; set; }

        [DataMember(Name = "transactions")]
        public List<InvoiceTransactionModel> Transactions { get; set; }

        [DataMember(Name = "transactioncount")]
        public int TransactionCount { get; set; }

        [DataMember(Name = "pendingtransaction")]
        public PaymentTransactionModel PendingTransaction { get; set; }

        [DataMember(Name = "ponumber")]
        [Description("PO Number")]
        public string PONumber { get; set; }

        public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
        {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceNumber", Label = "Reference #", Order = 10 });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "TypeDescription", Order = 20 });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "StatusDescription", Order = 30 });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "PONumber", Order = 40 });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceDate", Order = 50 });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "DueDate", Order = 60 });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceAmount", Order = 70 });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Amount", Order = 80 });

            return defaultConfig;
        }
    }
}
