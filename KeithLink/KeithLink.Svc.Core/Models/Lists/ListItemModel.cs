// KeithLink
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;

// Core
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Spreadsheet;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "ListItem")]
    public class ListItemModel : BaseProductInfo, IComparable, IExportableModel
    {

        #region properties
        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [DataMember(Name = "listitemid")]
        public long ListItemId { get; set; }

        [DataMember(Name = "detail")]
        public string Detail { get; set; }

        [Description("Item Order History")]
        public string OrderHistoryString { get; set; }

        [DataMember(Name = "each")]
        public bool? Each { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "parlevel")]
        [Description("PAR")]
        public decimal ParLevel { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "packsize")]
        [Description("Pack/Size")]
        public new string PackSize { get; set; }

        [DataMember(Name = "storagetemp", EmitDefaultValue = false)]
        public string StorageTemp { get; set; }

        [DataMember(Name = "category")]
        [Description("Contract Category")]
        public string Category { get; set; }

        [DataMember(Name = "fromdate")]
        [Description("From Date")]
        public DateTime? FromDate { get; set; }

        [DataMember(Name = "todate")]
        [Description("To Date")]
        public DateTime? ToDate { get; set; }

        [DataMember(Name = "delta")]
        public string Delta { get; set; }

        [DataMember(Name = "quantity")]
        public decimal Quantity { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime ModifiedUtc { get; set; }

        [DataMember(Name = "isdeleted")]
        public bool IsDelete { get; set; }

        [DataMember]
        public ListType Type { get; set; }

        [DataMember(Name = "itemstatistics")]
        public ItemHistoryModel ItemStatistics { get; set; }

        [DataMember(Name = "catalog_id")]
        public new string CatalogId { get; set; }

        [DataMember(Name = "custominventoryitemid")]
        public long CustomInventoryItemId { get; set; }

        // not exported
        public string ProprietaryCustomers { get; set; }

        /// <summary>
        /// This is for custom inventory specifically
        /// </summary>
        [DataMember(Name = "supplier")]
        public string Supplier { get; set; }

        #endregion

        #region functions

        public int CompareTo(object obj)
        {
            return this.Position.CompareTo(((ListItemModel)obj).Position);
        }

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
                                                                 Label = "Name"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Brand",
                                                                 Order = Constants.PLACEHOLDER_ORDER_03RD,
                                                                 Label = "Brand"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "ItemClass",
                                                                 Order = Constants.PLACEHOLDER_ORDER_04TH,
                                                                 Label = "Category"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Pack",
                                                                 Order = Constants.PLACEHOLDER_ORDER_05TH,
                                                                 Label = "Pack"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Size",
                                                                 Order = Constants.PLACEHOLDER_ORDER_06TH,
                                                                 Label = "Size"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Notes",
                                                                 Order = Constants.PLACEHOLDER_ORDER_08TH,
                                                                 Label = "Note"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "Each",
                                                                 Order = Constants.PLACEHOLDER_ORDER_10TH,
                                                                 Label = "Each"
                                                             });
            defaultConfig.Add(new ExportModelConfiguration() {
                                                                 Field = "CasePrice",
                                                                 Order = Constants.PLACEHOLDER_ORDER_11TH,
                                                                 Label = "Price"
                                                             });

            switch (this.Type)
            {
                case ListType.Favorite:
                    break;
                case ListType.Custom:
                    defaultConfig.Add(new ExportModelConfiguration() {
                                                                         Field = "label",
                                                                         Order = Constants.PLACEHOLDER_ORDER_07TH,
                                                                         Label = "Label"
                                                                     });
                    defaultConfig.Add(new ExportModelConfiguration() {
                                                                         Field = "parlevel",
                                                                         Order = Constants.PLACEHOLDER_ORDER_09TH,
                                                                         Label = "PAR"
                                                                     });
                    break;
                case ListType.Contract:
                case ListType.ContractItemsAdded:
                case ListType.ContractItemsDeleted:
                    defaultConfig.Add(new ExportModelConfiguration() {
                                                                         Field = "Category",
                                                                         Order = Constants.PLACEHOLDER_ORDER_07TH,
                                                                         Label = "Contract Category"
                                                                     });
                    break;
                default:
                    break;

            }

            return defaultConfig;

        }
        #endregion

        #region OpenXml exports
        public static int SetWidths(ExportModelConfiguration config, int width)
        {
            List<string> fields = new List<string>() { "Name", "Brand", "ItemClass", "Category", "label", "Notes" };
            if (fields.Contains(config.Field))
            {
                width = Constants.OPENXML_EXPORT_WIDTH_PIXELS_16;
            }
            return width;
        }

        public static uint SetStyleForHeader(string fieldName, uint styleInd)
        {
            styleInd = Constants.OPENXML_TEXT_WRAP_BOLD_CELL;
            switch (fieldName)
            {
                case "Pack":
                case "CasePrice":
                case "PackagePrice":
                case "parlevel":
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
                case "parlevel":
                    styleInd = Constants.OPENXML_RIGHT_ALIGNED_CELL;
                    break;
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
                case "CasePrice":
                case "PackagePrice":
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