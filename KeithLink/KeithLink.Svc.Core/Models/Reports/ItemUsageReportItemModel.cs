using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System.ComponentModel;

using DocumentFormat.OpenXml.Spreadsheet;

namespace KeithLink.Svc.Core.Models.Reports
{
    [DataContract(Name = "reportitemusage")]
    public class ItemUsageReportItemModel : BaseProductInfo, IExportableModel
    {
        public ItemUsageReportItemModel()
        {
            PackSize = string.Empty;
        }

        [DataMember(Name = "itemnumber")]
        [Description("Item")]
        public new string ItemNumber { get; set; }

        [DataMember(Name = "detail")]
        [Description("Detail")]
        public string Detail { get; set; }

        [DataMember(Name = "name")]
        [Description("Name")]
        public new string Name { get; set; }

        [DataMember(Name = "class")]
        [Description("Category")]
        public string Class { get; set; }

        [DataMember(Name = "images")]
        public List<ProductImage> Images { get; set; }

        [Description("Item Order History")]
        public string OrderHistoryString { get; set; }

        [DataMember(Name = "totalquantityordered")]
        [Description("Qty Ordered")]
        public int TotalQuantityOrdered { get; set; }

        [DataMember(Name = "totalquantityshipped")]
        [Description("Qty Shipped")]
        public int TotalQuantityShipped { get; set; }

        [DataMember(Name = "packsize")]
        [Description("Pack/Size")]
        public new string PackSize { get; set; }

        public new string Pack { get; set; }
        public new string Size { get; set; }
        [DataMember(Name = "each")]
        [Description("Each")]
        public string Each { get; set; }

        [DataMember(Name = "averageprice")]
        [Description("Average Price")]
        public decimal AveragePrice { get; set; }


        [DataMember(Name = "totalcost")]
        [Description("Total Cost")]
        public decimal TotalCost { get; set; }



        public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration() {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "ItemNumber",
                                                                 Order = Constants.PLACEHOLDER_ORDER_01ST,
                                                                 Label = "Item"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Name",
                                                                 Order = Constants.PLACEHOLDER_ORDER_02ND,
                                                                 Label = "Name"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Brand",
                                                                 Order = Constants.PLACEHOLDER_ORDER_03RD,
                                                                 Label = "Brand"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Class",
                                                                 Order = Constants.PLACEHOLDER_ORDER_04TH,
                                                                 Label = "Category"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "ManufacturerName",
                                                                 Order = Constants.PLACEHOLDER_ORDER_05TH,
                                                                 Label = "Mfr Name"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "UPC",
                                                                 Order = Constants.PLACEHOLDER_ORDER_06TH,
                                                                 Label = "GTIN"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "VendorItemNumber",
                                                                 Order = Constants.PLACEHOLDER_ORDER_07TH,
                                                                 Label = "Vendor Item"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Pack",
                                                                 Order = Constants.PLACEHOLDER_ORDER_08TH,
                                                                 Label = "Pack"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Size",
                                                                 Order = Constants.PLACEHOLDER_ORDER_09TH,
                                                                 Label = "Size"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Each",
                                                                 Order = Constants.PLACEHOLDER_ORDER_10TH,
                                                                 Label = "Each"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "TotalQuantityOrdered",
                                                                 Order = Constants.PLACEHOLDER_ORDER_11TH,
                                                                 Label = "Qty Ordered"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "TotalQuantityShipped",
                                                                 Order = Constants.PLACEHOLDER_ORDER_12TH,
                                                                 Label = "Qty Shipped"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "AveragePrice",
                                                                 Order = Constants.PLACEHOLDER_ORDER_13TH,
                                                                 Label = "Average Price"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "TotalCost",
                                                                 Order = Constants.PLACEHOLDER_ORDER_14TH,
                                                                 Label = "Total Cost"
                                                             });



            return defaultConfig;
        }

        #region OpenXml exports
        public static int SetWidths(ExportModelConfiguration config, int width)
        {
            switch (config.Field)
            {
                case "Brand":
                case "Class":
                case "ManufacturerName":
                case "Name":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_25;
                    break;
                case "Detail":
                case "OrderHistoryString":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_80;
                    break;
                case "UPC":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_16;
                    break;
                case "AveragePrice":
                case "TotalCost":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_10;
                    break;
            }
            return width;
        }

        public static uint SetStyleForHeader(string fieldName, uint styleInd)
        {
            styleInd = Constants.OPENXML_TEXT_WRAP_BOLD_CELL;
            switch (fieldName)
            {
                case "Pack":
                case "TotalQuantityOrdered":
                case "TotalQuantityShipped":
                case "AveragePrice":
                case "TotalCost":
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
                case "TotalQuantityOrdered":
                case "TotalQuantityShipped":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_CELL;
                    break;
                case "AveragePrice":
                case "TotalCost":
                    styleInd = Constants.OPENXML_NUMBER_F2_CELL;
                    break;
            }
            return styleInd;
        }

        public static CellValues SetCellValueTypeForCells(string fieldName, CellValues celltype)
        {
            switch (fieldName)
            {
                case "Pack":
                case "TotalQuantityOrdered":
                case "TotalQuantityShipped":
                case "AveragePrice":
                case "TotalCost":
                    celltype = CellValues.Number;
                    break;
            }
            return celltype;
        }

        #endregion

    }
}