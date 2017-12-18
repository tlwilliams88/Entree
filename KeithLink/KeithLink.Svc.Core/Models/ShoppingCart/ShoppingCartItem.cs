using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
    [DataContract(Name = "ShoppingCartItem")]
    public class ShoppingCartItem : BaseProductInfo, IExportableModel
    {
        [DataMember(Name = "cartitemid")]
        public Guid CartItemId { get; set; }

        [DataMember(Name = "quantity")]
        public decimal Quantity { get; set; }

        [Description("Item Order History")]
        public string OrderHistoryString { get; set; }

        public string strPosition { get; set; }
        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "packsize")]
        public new string PackSize { get; set; }

        [DataMember(Name = "name")]
        public new string Name { get; set; }

        [DataMember(Name = "detail")]
        public new string Detail { get; set; }

        [DataMember(Name = "notes")]
        public new string Notes { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "iscombinedquantity")]
        public bool IsCombinedQuantity { get; set; }

        [DataMember(Name = "parlevel")]
        public decimal ParLevel { get; set; }

        [DataMember(Name = "each")]
        public bool Each { get; set; }

        [DataMember(Name = "storagetemp")]
        public string StorageTemp { get; set; }

        [DataMember(Name = "sourceProductList")]
        public string SourceProductList { get; set; }

        [DataMember(Name = "createddate")]
        public DateTime CreatedDate { get; set; }

        public double LineTotal(double Price)
        {

            if (this.CatchWeight)
            {
                if (this.Each) //package catchweight
                {
                    return ((this.AverageWeight / Int32.Parse(this.Pack)) * (double)this.Quantity) * Price;
                }
                else //case catchweight
                {
                    return (this.AverageWeight * (double)this.Quantity) * Price;
                }

            }
            else
            {
                return (double)this.Quantity * Price;
            }
        }
        public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
        {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item #" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 10, Label = "Name" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Brand", Order = 20, Label = "Brand" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemClass", Order = 30, Label = "Class" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Notes", Order = 40, Label = "Notes" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Pack", Order = 50, Label = "Pack" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Size", Order = 60, Label = "Size" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Quantity", Order = 70, Label = "Qty" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Each", Order = 80, Label = "Each" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "CasePrice", Order = 90, Label = "Price" });

            return defaultConfig;
        }

        #region OpenXml exports
        public static int SetWidths(ExportModelConfiguration config, int width)
        {
            switch (config.Field)
            {
                case "Name":
                case "BrandExtendedDescription":
                case "ItemClass":
                case "Notes":
                case "Status":
                    width = 20;
                    break;
                case "Detail":
                case "OrderHistoryString":
                    width = 80;
                    break;
                case "Pack":
                    width = 8;
                    break;
                case "Size":
                case "Quantity":
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
                case "Pack":
                case "Quantity":
                case "EachYN":
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
                case "Pack":
                case "Quantity":
                case "EachYN":
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
                case "Quantity":
                case "CasePrice":
                case "PackagePrice":
                    celltype = CellValues.Number;
                    break;
            }
            return celltype;
        }

        #endregion
    }
}