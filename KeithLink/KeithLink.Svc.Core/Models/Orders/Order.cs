using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
    [DataContract(Name = "Order")]
    public class Order : IExportableModel
    {
        [DataMember(Name = "ordernumber")]
        [Description("Order #")]
        public string OrderNumber { get; set; }

        [DataMember(Name = "catalog_id")]
        public string CatalogId { get; set; }

        [DataMember(Name = "catalogtype")]
        public string CatalogType { get; set; }

        [DataMember(Name = "relatedordernumbers")]
        public string RelatedOrderNumbers { get; set; }

        [DataMember(Name = "relatedinvoicenumbers")]
        public string RelatedInvoiceNumbers { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "deliverydate")]
        [Description("Delivery Date")]
        public string DeliveryDate { get; set; }

        [DataMember(Name = "invoicenumber")]
        [Description("Invoice #")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "listId")]
        public long? ListId { get; set; }

        [DataMember(Name = "invoicestatus")]
        [Description("Invoice Status")]
        public string InvoiceStatus { get; set; }

        [DataMember(Name = "itemcount")]
        [Description("Item Count")]
        public int ItemCount { get; set; }
        //public int ItemCount { get { return this.Items != null ? this.Items.Sum(l => l.Quantity) : 0; } } // to do: allow setting item count for order history?

        [DataMember(Name = "ordertotal")]
        [Description("Subtotal")]
        public double OrderTotal { get; set; }

        [DataMember(Name = "createddate")]
        [Description("Order Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = "requestedshipdate")]
        public string RequestedShipDate { get; set; }

        [DataMember(Name = "ischangeorderallowed")]
        public bool IsChangeOrderAllowed { get; set; }

        [DataMember(Name = "items")]
        public List<OrderLine> Items { get; set; }

        [DataMember(Name = "commerceid")]
        public Guid CommerceId { get; set; }

        [DataMember(Name = "estimateddeliverytime")]
        public string EstimatedDeliveryTime { get; set; }

        [DataMember(Name = "scheduleddeliverytime")]
        public string ScheduledDeliveryTime { get; set; }

        [DataMember(Name = "actualdeliverytime")]
        public string ActualDeliveryTime { get; set; }

        [DataMember(Name = "deliveryoutofsequence")]
        public bool? DeliveryOutOfSequence { get; set; }

        [DataMember(Name = "ponumber")]
        public string PONumber { get; set; }

        [DataMember(Name = "ordersystem")]
        public string OrderSystem { get; set; }

        [DataMember(Name = "isspecialorder")]
        public bool IsSpecialOrder { get; set; }

        public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
        {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceNumber", Order = 1, Label = "Invoice #" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "CreatedDate", Order = 10, Label = "Order Date" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Status", Order = 20, Label = "Status" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "DeliveryDate", Order = 30, Label = "Delivery Date" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemCount", Order = 40, Label = "Item Count" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "OrderTotal", Order = 50, Label = "Subtotal" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceStatus", Order = 70, Label = "Invoice Status" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "PONumber", Order = 80, Label = "PO #" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "OrderSystem", Order = 90, Label = "Order System" });

            return defaultConfig;
        }

        #region OpenXml exports
        public static int SetWidths(ExportModelConfiguration config, int width)
        {
            switch (config.Field)
            {
                case "Status":
                case "InvoiceStatus":
                case "PONumber":
                case "OrderSystem":
                case "DeliveryDate":
                    width = 20;
                    break;
                case "InvoiceNumber":
                case "CreatedDate":
                case "ItemCount":
                case "OrderTotal":
                    width = 12;
                    break;
            }
            return width;
        }

        public static uint SetStyleForHeader(string fieldName, uint styleInd)
        {
            styleInd = Constants.OPENXML_TEXT_WRAP_BOLD_CELL;
            switch (fieldName)
            {
                case "CreatedDate":
                case "DeliveryDate":
                case "ItemCount":
                case "OrderTotal":
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
                case "OrderTotal":
                    styleInd = Constants.OPENXML_NUMBER_F2_CELL;
                    break;
            }
            return styleInd;
        }

        public static CellValues SetCellValueTypeForCells(string fieldName, CellValues celltype)
        {
            switch (fieldName)
            {
                case "OrderTotal":
                    celltype = CellValues.Number;
                    break;
            }
            return celltype;
        }

        #endregion

    }
}