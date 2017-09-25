using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System.ComponentModel;

using DocumentFormat.OpenXml.Spreadsheet;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "product")]
    [Serializable]
    public class Product : BaseProductInfo, IExportableModel
    {
        #region ctor
        public Product()
        {
            ProductImages = new List<ProductImage>();
            OrderHistory = new Dictionary<string, int>();
        }
        #endregion

        #region methods
        public List<ExportModelConfiguration> DefaultExportConfiguration()
        {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "ItemNumber",
                                                                 Order = Constants.PLACEHOLDER_ORDER_01ST,
                                                                 Label = "Item"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Name",
                                                                 Order = Constants.PLACEHOLDER_ORDER_02ND,
                                                                 Label = "Name" });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "BrandExtendedDescription",
                                                                 Order = Constants.PLACEHOLDER_ORDER_03RD,
                                                                 Label = "Brand"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Pack",
                                                                 Order = Constants.PLACEHOLDER_ORDER_04TH,
                                                                 Label = "Pack"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Size",
                                                                 Order = Constants.PLACEHOLDER_ORDER_05TH,
                                                                 Label = "Size"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "CaseOnly",
                                                                 Order = Constants.PLACEHOLDER_ORDER_06TH,
                                                                 Label = "Each"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "UnitCost",
                                                                 Order = Constants.PLACEHOLDER_ORDER_07TH,
                                                                 Label = "Cost"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "CasePrice",
                                                                 Order = Constants.PLACEHOLDER_ORDER_08TH,
                                                                 Label = "Price"
                                                             });


            return defaultConfig;
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            //Do not output a "blank" upc
            if (this.UPC != null)
            {
                if (this.UPC.Equals("00000000000000"))
                    this.UPC = string.Empty;
            }

        }

        #region OpenXml exports
        public static int SetWidths(ExportModelConfiguration config, int width)
        {
            switch (config.Field)
            {
                case "Name":
                case "BrandExtendedDescription":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_20;
                    break;
                case "Detail":
                case "OrderHistoryString":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_80;
                    break;
                case "Pack":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_08;
                    break;
                case "UnitCost":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_14;
                    break;
                case "CasePrice":
                case "PackagePrice":
                case "Size":
                    width = Constants.EXCEL_EXPORT_WIDTH_PIXELS_12;
                    break;
            }
            return width;
        }

        public static uint SetStyleForHeader(string fieldName, uint styleInd)
        {
            styleInd = Constants.OPENXML_TEXT_WRAP_BOLD_CELL;
            switch (fieldName)
            {
                case "ItemNumber":
                case "Pack":
                case "UnitCost":
                case "CasePrice":
                case "PackagePrice":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                    break;
            }
            return styleInd;
        }

        public static uint SetStyleForCell(string fieldName, uint styleInd)
        {
            switch (fieldName)
            {
                case "ItemNumber":
                case "Pack":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_CELL;
                    break;
                case "UnitCost":
                case "CasePrice":
                case "PackagePrice":
                    styleInd = Constants.OPENXML_NUMBER_F2_CELL;
                    break;
            }
            return styleInd;
        }

        public static CellValues SetCellValueTypeForCells(string fieldName, CellValues celltype)
        {
            switch (fieldName)
            {
                case "ItemNumber":
                case "UnitCost":
                case "CasePrice":
                case "PackagePrice":
                    celltype = CellValues.Number;
                    break;
            }
            return celltype;
        }

        #endregion

        #endregion

        #region properties
        [DataMember(Name = "ext_description")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ExtendedDescription { get; set; }

        [Description("Item Order History")]
        public string OrderHistoryString { get; set; }

        [DataMember(Name = "caseaverage")]
        public int CaseAverage { get; set; }

        [DataMember(Name = "packageaverage")]
        public int PackageAverage { get; set; }

        [DataMember(Name = "detail")]
        public string Detail { get; set; }

        [DataMember(Name = "cube")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CaseCube { get; set; }

        [DataMember(Name = "category")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [DataMember(Name = "productimages")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ProductImage> ProductImages { get; set; }

        [DataMember(Name = "isproprietary")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IsProprietary { get; set; }

        public string ProprietaryCustomers { get; set; }

        [DataMember(Name = "orderhistory")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, int> OrderHistory { get; set; }

        [DataMember(Name = "inhistory")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool InHistory { get; set; }
        #endregion
    }


}