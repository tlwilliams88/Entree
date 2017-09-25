using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Spreadsheet;

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

        [DataMember(Name = "customerstreetaddress")]
        public string CustomerStreetAddress { get; set; }

        [DataMember(Name = "customercity")]
        public string CustomerCity { get; set; }

        [DataMember(Name = "customerregioncode")]
        public string CustomerRegionCode { get; set; }

        [DataMember(Name = "customerpostalcode")]
        public string CustomerPostalCode { get; set; }

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

        [DataMember(Name = "banks")]
        public List<CustomerBank> Banks { get; set; }

        public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
        {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "InvoiceNumber",
                                                                 Label = "Reference #",
                                                                 Order = Constants.PLACEHOLDER_ORDER_01ST
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "TypeDescription",
                                                                 Order = Constants.PLACEHOLDER_ORDER_02ND
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "StatusDescription",
                                                                 Order = Constants.PLACEHOLDER_ORDER_03RD
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "PONumber",
                                                                 Order = Constants.PLACEHOLDER_ORDER_04TH
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "InvoiceDate",
                                                                 Order = Constants.PLACEHOLDER_ORDER_05TH
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "DueDate",
                                                                 Order = Constants.PLACEHOLDER_ORDER_06TH
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "InvoiceAmount",
                                                                 Order = Constants.PLACEHOLDER_ORDER_07TH
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Amount",
                                                                 Order = Constants.PLACEHOLDER_ORDER_08TH
                                                             });

            return defaultConfig;
        }

        #region OpenXml exports
        public static int SetWidths(ExportModelConfiguration config, int width)
        {
            switch (config.Field)
            {
                case "InvoiceNumber":
                case "PONumber":
                case "TypeDescription":
                    width = Constants.OPENXML_EXPORT_WIDTH_PIXELS_12;
                    break;
                case "InvoiceAmount":
                case "Amount":
                    width = Constants.OPENXML_EXPORT_WIDTH_PIXELS_16;
                    break;
                case "InvoiceDate":
                case "DueDate":
                    width = Constants.OPENXML_EXPORT_WIDTH_PIXELS_14;
                    break;
            }
            return width;
        }

        public static uint SetStyleForHeader(string fieldName, uint styleInd)
        {
            styleInd = Constants.OPENXML_TEXT_WRAP_BOLD_CELL;
            switch (fieldName)
            {
                case "InvoiceDate":
                case "DueDate":
                case "InvoiceAmount":
                case "Amount":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                    break;
            }
            return styleInd;
        }

        public static uint SetStyleForCell(string fieldName, uint styleInd)
        {
            styleInd = Constants.OPENXML_TEXT_WRAP_CELL;
            switch (fieldName)
            {
                case "InvoiceDate":
                case "DueDate":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_CELL;
                    break;
                case "InvoiceAmount":
                case "Amount":
                    styleInd = Constants.OPENXML_NUMBER_F2_CELL;
                    break;
            }
            return styleInd;
        }

        public static CellValues SetCellValueTypeForCells(string fieldName, CellValues celltype)
        {
            switch (fieldName)
            {
                case "InvoiceAmount":
                case "Amount":
                    celltype = CellValues.Number;
                    break;
            }
            return celltype;
        }

        #endregion
    }
}