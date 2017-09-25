using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Spreadsheet;

namespace KeithLink.Svc.Core.Models.Invoices
{
    [DataContract(Name = "InvoiceItem")]
    public class InvoiceItemModel : BaseProductInfo, IExportableModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "linenumber")]
        [Description("Line #")]
        public string LineNumber { get; set; }

        [DataMember(Name = "itemnumber")]
        [Description("Item #")]
        public new string ItemNumber { get; set; }

        [DataMember(Name = "detail")]
        public string Detail { get; set; }

        [Description("Item Order History")]
        public string OrderHistoryString { get; set; }

        [DataMember(Name = "quantityordered")]
        [Description("# Ordered")]
        public int? QuantityOrdered { get; set; }
        [DataMember(Name = "each")]
        [Description("Each")]
        public bool Each { get; set; }
        [DataMember(Name = "quantityshipped")]
        [Description("# Shipped")]
        public int? QuantityShipped { get; set; }
        [DataMember(Name = "catchweight")]
        public bool CatchWeightCode { get; set; }
        [DataMember(Name = "extcatchweight")]
        public decimal? ExtCatchWeight { get; set; }
        [DataMember(Name = "itemprice")]
        [Description("Price")]
        public decimal? ItemPrice { get; set; }
        [DataMember(Name = "salesnet")]
        [Description("Ext Price")]
        public decimal? ExtSalesNet
        {
            get
            {
                return (decimal)PricingHelper.GetFixedPrice(QuantityShipped.Value, (double)ItemPrice, CatchWeight, (double)ExtCatchWeight, AverageWeight);
            }
        }
        [DataMember(Name = "classcode")]
        public string ClassCode { get; set; }
        [DataMember(Name = "packsize")]
        [Description("Pack/Size")]
        public new string PackSize { get; set; }


        public string InvoiceNumber { get; set; }

        [Description("Contract Category")]
        public string Category { get; set; }

        public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration() {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "LineNumber",
                                                                 Order = Constants.PLACEHOLDER_ORDER_01ST
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "ItemNumber",
                                                                 Order = Constants.PLACEHOLDER_ORDER_02ND
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Name",
                                                                 Order = Constants.PLACEHOLDER_ORDER_03RD
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "BrandExtendedDescription",
                                                                 Order = Constants.PLACEHOLDER_ORDER_04TH
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "PackSize",
                                                                 Order = Constants.PLACEHOLDER_ORDER_05TH
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Notes",
                                                                 Order = Constants.PLACEHOLDER_ORDER_06TH
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "QuantityOrdered",
                                                                 Order = Constants.PLACEHOLDER_ORDER_07TH
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "QuantityShipped",
                                                                 Order = Constants.PLACEHOLDER_ORDER_08TH
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Each",
                                                                 Order = Constants.PLACEHOLDER_ORDER_09TH
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "ItemPrice",
                                                                 Order = Constants.PLACEHOLDER_ORDER_10TH
            });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "ExtSalesNet",
                                                                 Order = Constants.PLACEHOLDER_ORDER_11TH
            });

            return defaultConfig;
        }

        #region OpenXml exports
        public static int SetWidths(ExportModelConfiguration config, int width)
        {
            switch (config.Field)
            {
                case "Name":
                case "Notes":
                case "BrandExtendedDescription":
                    width = Constants.OPENXML_EXPORT_WIDTH_WIDTH_PIXELS_25;
                    break;
                case "Detail":
                case "OrderHistoryString":
                    width = Constants.OPENXML_EXPORT_WIDTH_WIDTH_PIXELS_80;
                    break;
                case "Pack":
                    width = Constants.OPENXML_EXPORT_WIDTH_WIDTH_PIXELS_08;
                    break;
                case "quantityordered":
                case "quantityshipped":
                    width = Constants.OPENXML_EXPORT_WIDTH_WIDTH_PIXELS_14;
                    break;
                case "ItemPrice":
                case "ExtSalesNet":
                case "PackSize":
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
                case "QuantityOrdered":
                case "QuantityShipped":
                case "ItemPrice":
                case "ExtSalesNet":
                case "Each":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                    break;
            }
            return styleInd;
        }

        public static uint SetStyleForCell(string fieldName, uint styleInd)
        {
            switch (fieldName)
            {
                case "Pack":
                case "QuantityOrdered":
                case "QuantityShipped":
                case "Each":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_CELL;
                    break;
                case "ItemPrice":
                case "ExtSalesNet":
                    styleInd = Constants.OPENXML_NUMBER_F2_CELL;
                    break;
            }
            return styleInd;
        }

        public static CellValues SetCellValueTypeForCells(string fieldName, CellValues celltype)
        {
            switch (fieldName)
            {
                case "ItemPrice":
                case "ExtSalesNet":
                case "Price":
                case "parlevel":
                    celltype = CellValues.Number;
                    break;
            }
            return celltype;
        }

        #endregion

    }
}