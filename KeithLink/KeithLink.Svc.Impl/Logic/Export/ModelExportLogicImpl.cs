using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Common.Impl.Repository.Settings;
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KeithLink.Svc.Impl.Logic.Export
{
    public class ModelExportLogicImpl<T> : IModelExportLogic<T> where T : class, IExportableModel
    {
        private IList<T> Model { get; set; }
        private List<ExportModelConfiguration> exportConfig = null;
        private ICustomerRepository _customerRepo;
        private UserSelectedContext _context;

        public ModelExportLogicImpl(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public System.IO.MemoryStream Export(IList<T> model, List<ExportModelConfiguration> exportConfig, string exportType, UserSelectedContext context)
        {
            this.Model = model;
            this.exportConfig = exportConfig;
            this._context = context;
            return Export(exportType);
        }

        public System.IO.MemoryStream Export(IList<T> model, string exportType, UserSelectedContext context)
        {
            this.Model = model;
            this.exportConfig = model.First().DefaultExportConfiguration();
            this._context = context;
            return Export(exportType);
        }

        private MemoryStream Export(string exportType)
        {
            StringBuilder sb = new StringBuilder();

            if (exportType.Equals("excel", StringComparison.CurrentCultureIgnoreCase))
                return this.GenerateExcelExport();

            if (exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) || exportType.Equals("tab", StringComparison.CurrentCultureIgnoreCase))
            {
                AddTitleToTextExport(exportType, sb);
                AddCustomerToTextExport(exportType, sb);
                this.WriteHeaderRecord(sb, exportType);
            }

            if (this.Model != null && this.Model.Count > 0) // is there any data to render
            {
                foreach (var item in this.Model)
                {
                    if (item != null)
                        this.WriteItemRecord(sb, item, exportType);
                }
                if (typeof(T).Name.Equals("ItemUsageReportItemModel"))
                {
                    WriteTotalRecord(sb, exportType);
                }
            }
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            sw.Write(sb.ToString());
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private void AddCustomerToTextExport(string exportType, StringBuilder sb)
        {
            List<string> exports = Configuration.ExportAddCustomer;
            foreach (string gettitle in exports)
            {
                if (gettitle.Equals(typeof(T).Name))
                {
                    Customer customer = _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId);
                    List<string> cust = new List<string>();
                    cust.Add("\"" + customer.CustomerBranch + "\"");
                    cust.Add("\"" + customer.CustomerNumber + "\"");
                    cust.Add("\"" + customer.CustomerName + "\"");
                    sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) ? "," : "\t", cust));
                }
            }
        }

        private void AddTitleToTextExport(string exportType, StringBuilder sb)
        {
            List<string> exports = Configuration.ExportAddTitle;
            foreach (string gettitle in exports)
            {
                if (gettitle.StartsWith(typeof(T).Name))
                {
                    sb.AppendLine("\"" + gettitle.Substring(gettitle.IndexOf(';') + 1) + "\"");
                }
            }
        }

        #region Excel Export
        private MemoryStream GenerateExcelExport()
        {
            MemoryStream stream = OpenXmlSpreadsheetUtilities.MakeSpreadSheet
                (SetCustomColumnWidths(typeof(T).Name, new DocumentFormat.OpenXml.Spreadsheet.Worksheet()),
                 WriteDataTableToExcelWorksheet(),
                 typeof(T).Name);
            return stream;
        }

        private Worksheet SetCustomColumnWidths(string modelName, Worksheet workSheet)
        {
            uint colIndex = 0;
            int width = 0;
            foreach (ExportModelConfiguration config in exportConfig.OrderBy(c => c.Order))
            {
                colIndex++;
                width = 0;
                try
                {
                    width = int.Parse(DBAppSettingsRepositoryImpl.GetValue("EW." + modelName + "." + config.Field, "0"));
                }
                catch { }
                if (modelName.Equals("OrderLine"))
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
                        case "Pack":
                            width = 8;
                            break;
                        case "Size":
                        case "QuantityOrdered":
                        case "QantityShipped":
                            width = 12;
                            break;
                    }
                }
                else if (modelName.Equals("Product"))
                {
                    switch (config.Field)
                    {
                        case "Name":
                        case "BrandExtendedDescription":
                            width = 20;
                            break;
                        case "Pack":
                            width = 8;
                            break;
                        case "UnitCost":
                            width = 14;
                            break;
                        case "CasePrice":
                        case "PackagePrice":
                        case "Size":
                            width = 12;
                            break;
                    }
                }
                else if (modelName.Equals("Order"))
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
                }
                else if (modelName.Equals("ItemUsageReportItemModel"))
                {
                    switch (config.Field)
                    {
                        case "Brand":
                        case "Class":
                        case "ManufacturerName":
                        case "Name":
                            width = 25;
                            break;
                        case "UPC":
                            width = 15;
                            break;
                        case "AveragePrice":
                        case "TotalCost":
                            width = 10;
                            break;
                    }
                }
                else if (modelName.Equals("InvoiceModel"))
                {
                    switch (config.Field)
                    {
                        case "InvoiceNumber":
                        case "PONumber":
                        case "TypeDescription":
                            width = 12;
                            break;
                        case "InvoiceAmount":
                        case "Amount":
                            width = 16;
                            break;
                        case "InvoiceDate":
                        case "DueDate":
                            width = 14;
                            break;
                    }
                }
                else if (modelName.Equals("ListItemModel"))
                {
                    switch (config.Field)
                    {
                        case "Name":
                        case "Brand":
                        case "ItemClass":
                        case "Category":
                        case "label":
                        case "Notes":
                            width = 16;
                            break;
                    }
                }

                if (width > 0)
                    OpenXmlSpreadsheetUtilities.SetColumnWidth(workSheet, colIndex, width);
            }
            return workSheet;
        }

        private SheetData WriteDataTableToExcelWorksheet()
        {
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            string cellValue = "";

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            int numberOfColumns = this.exportConfig.Count;
            bool[] IsNumericColumn = new bool[numberOfColumns];

            string[] excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            uint rowIndex = 1;
            //
            //  Create the Header row in our Excel Worksheet
            //
            rowIndex = AddTitleToExcelExport(sheetData, excelColumnNames, rowIndex);
            rowIndex = AddCustomerToExcelExport(sheetData, excelColumnNames, rowIndex);

            var headerRow = new Row { RowIndex = rowIndex };  // add a row at the to name the fields of spreadsheet
            sheetData.Append(headerRow);

            var properties = typeof(T).GetProperties();
            int columnIndex = 0;
            foreach (var config in exportConfig.OrderBy(e => e.Order))
            {
                var propertyName = config.Field.Split('.');
                uint styleInd = SetStyleForHeaderCell(typeof(T).Name, config.Field);

                if (propertyName.Length == 1)
                {

                    var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (property != null)
                    {
                        OpenXmlSpreadsheetUtilities.AppendTextCell
                            (excelColumnNames[columnIndex] + rowIndex.ToString(), GetPropertyDescription(property).Trim(), headerRow, CellValues.String, styleInd);
                        columnIndex++;
                    }
                }
                else
                {
                    var childProp = properties.Where(p => p.Name.Equals(propertyName[0])).FirstOrDefault();
                    var childProperties = childProp.PropertyType.GetProperties();
                    var subProperty = childProperties.Where(p => p.Name.Equals(propertyName[1])).FirstOrDefault();
                    if (subProperty != null)
                    {
                        OpenXmlSpreadsheetUtilities.AppendTextCell
                            (excelColumnNames[columnIndex] + rowIndex.ToString(), GetPropertyDescription(subProperty).Trim(), headerRow);
                        columnIndex++;
                    }
                }

            }

            //
            //  Now, step through each row of data in our DataTable...

            foreach (var item in this.Model)
            {
                rowIndex++;
                var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
                sheetData.Append(newExcelRow);
                if (item != null)
                {
                    columnIndex = 0;
                    foreach (var config in exportConfig.OrderBy(e => e.Order))
                    {
                        var propertyName = config.Field.Split('.');
                        if (propertyName.Length == 1)
                        {
                            ExportModelConfiguration thisConfig = new ExportModelConfiguration()
                            { // just a shallow copy
                                Field = config.Field,
                                Label = config.Label,
                                Order = config.Order,
                                Selected = config.Selected
                            };
                            SetPriceConfig(properties, item, thisConfig);
                            var property = properties.Where(p => p.Name.Equals(thisConfig.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                            uint styleInd = SetStyleForCell(typeof(T).Name, thisConfig.Field);
                            CellValues celltype = SetCellValuesForCell(typeof(T).Name, thisConfig.Field);
                            if (property != null)
                            {
                                OpenXmlSpreadsheetUtilities.AppendTextCell
                                    (excelColumnNames[columnIndex] + rowIndex.ToString(),
                                    this.GetFieldValue(item, property).Trim(), newExcelRow,
                                    celltype,
                                    styleInd);
                            }
                        }
                        else
                        {
                            var childProp = properties.Where(p => p.Name.Equals(propertyName[0])).FirstOrDefault();
                            var childProperties = childProp.PropertyType.GetProperties();
                            var subProperty = childProperties.Where(p => p.Name.Equals(propertyName[1])).FirstOrDefault();
                            if (subProperty != null)
                            {
                                OpenXmlSpreadsheetUtilities.AppendTextCell
                                    (excelColumnNames[columnIndex] + rowIndex.ToString(), this.GetFieldValue(childProp.GetValue(item), subProperty).Trim(), newExcelRow);
                            }

                        }

                        columnIndex++;
                    }
                }
            }
            if (typeof(T).Name.Equals("ItemUsageReportItemModel"))
            {
                AddTotalRowExcel(rowIndex, excelColumnNames, sheetData);
            }
            return sheetData;
        }

        private uint AddCustomerToExcelExport(SheetData sheetData, string[] excelColumnNames, uint rowIndex)
        {
            List<string> exports = Configuration.ExportAddCustomer;
            foreach (string gettitle in exports)
            {
                if (gettitle.Equals(typeof(T).Name))
                {
                    rowIndex = OpenXmlSpreadsheetUtilities.AddCustomerRow
                        (rowIndex, typeof(T).Name, excelColumnNames, _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId), sheetData);
                }
            }

            return rowIndex;
        }

        private static uint AddTitleToExcelExport(SheetData sheetData, string[] excelColumnNames, uint rowIndex)
        {
            List<string> exports = Configuration.ExportAddTitle;
            foreach (string gettitle in exports)
            {
                if (gettitle.StartsWith(typeof(T).Name))
                {
                    rowIndex = OpenXmlSpreadsheetUtilities.AddTitleRow
                        (rowIndex, typeof(T).Name, excelColumnNames, gettitle.Substring(gettitle.IndexOf(';') + 1), sheetData);
                }
            }

            return rowIndex;
        }

        private void SetPriceConfig(PropertyInfo[] properties, T item, ExportModelConfiguration thisConfig)
        {
            if (thisConfig.Label != null &&
                thisConfig.Label.Equals("Price") &&
                properties.Select(p => p.Name).Contains("Each", StringComparer.CurrentCultureIgnoreCase))
            {
                PropertyInfo eachProperty = properties.Where(p => p.Name.Equals("Each", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (eachProperty != null)
                {
                    string each = GetFieldValue(item, eachProperty).Trim();
                    if (each.Equals("Y"))
                    {
                        thisConfig.Field = "PackagePrice";
                    }
                }
            }
        }

        private uint SetStyleForHeaderCell(string modelName, string fieldName)
        {
            uint styleInd = OpenXmlSpreadsheetUtilities.DEFAULT_CELL;
            if (modelName.Equals("ItemUsageReportItemModel"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL;
                switch (fieldName)
                {
                    case "Pack":
                    case "TotalQuantityOrdered":
                    case "TotalQuantityShipped":
                    case "AveragePrice":
                    case "TotalCost":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                        break;
                }
            }
            else if (modelName.Equals("ListItemModel"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL;
                switch (fieldName)
                {
                    case "Pack":
                    case "CasePrice":
                    case "PackagePrice":
                    case "parlevel":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                        break;
                }
            }
            else if (modelName.Equals("Product"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL;
                switch (fieldName)
                {
                    case "Pack":
                    case "UnitCost":
                    case "CasePrice":
                    case "PackagePrice":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                        break;
                }
            }
            else if (modelName.Equals("OrderLine"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL;
                switch (fieldName)
                {
                    case "Pack":
                    case "QuantityOrdered":
                    case "QantityShipped":
                    case "EachYN":
                    case "Price":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                        break;
                }
            }
            else if (modelName.Equals("Order"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL;
                switch (fieldName)
                {
                    case "CreatedDate":
                    case "DeliveryDate":
                    case "ItemCount":
                    case "OrderTotal":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                        break;
                }
            }
            else if (modelName.Equals("InvoiceModel"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL;
                switch (fieldName)
                {
                    case "InvoiceDate":
                    case "DueDate":
                    case "InvoiceAmount":
                    case "Amount":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL;
                        break;
                }
            }
            return styleInd;
        }

        private uint SetStyleForCell(string modelName, string fieldName)
        {
            uint styleInd = OpenXmlSpreadsheetUtilities.DEFAULT_CELL;
            if (modelName.Equals("ItemUsageReportItemModel"))
            {
                switch (fieldName)
                {
                    //case "Name": // they don't want wrapped cells per danny
                    //case "Brand":
                    //case "Class":
                    //case "ManufacturerName":
                    //    styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                    //    break;
                    case "Pack":
                    case "TotalQuantityOrdered":
                    case "TotalQuantityShipped":
                    case "AveragePrice":
                    case "TotalCost":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL;
                        break;
                }
            }
            else if (modelName.Equals("ListItemModel"))
            {
                switch (fieldName)
                {
                    //case "Name": // they don't want wrapped cells per danny
                    //case "Brand":
                    //case "ItemClass":
                    //case "label":
                    //case "Category":
                    //case "Notes":
                    //    styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                    //    break;
                    case "Pack":
                    case "CasePrice":
                    case "PackagePrice":
                    case "parlevel":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL;
                        break;
                }
            }
            else if (modelName.Equals("Product"))
            {
                switch (fieldName)
                {
                    //case "Name": // they don't want wrapped cells per danny
                    //case "BrandExtendedDescription":
                    //case "Size":
                    //    styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                    //    break;
                    case "Pack":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL;
                        break;
                    case "UnitCost":
                    case "CasePrice":
                    case "PackagePrice":
                        styleInd = OpenXmlSpreadsheetUtilities.NUMBER_F2_CELL;
                        break;
                }
            }
            else if (modelName.Equals("OrderLine"))
            {
                switch (fieldName)
                {
                    //case "Name": // they don't want wrapped cells per danny
                    //case "ItemClass":
                    //case "BrandExtendedDescription":
                    //case "Notes":
                    //case "Status":
                    //    styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                    //    break;
                    case "Pack":
                    case "QuantityOrdered":
                    case "QantityShipped":
                    case "EachYN":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL;
                        break;
                    case "Price":
                        styleInd = OpenXmlSpreadsheetUtilities.NUMBER_F2_CELL;
                        break;
                }
            }
            else if (modelName.Equals("Order"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                switch (fieldName)
                {
                    //case "CreatedDate": // they don't want wrapped cells per danny
                    //case "DeliveryDate":
                    //case "ItemCount":
                    //    styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_CELL;
                    //    break;
                    case "OrderTotal":
                        styleInd = OpenXmlSpreadsheetUtilities.NUMBER_F2_CELL;
                        break;
                }
            }
            else if (modelName.Equals("InvoiceModel"))
            {
                styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                switch (fieldName)
                {
                    case "InvoiceDate":
                    case "DueDate":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL;
                        break;
                    case "InvoiceAmount":
                    case "Amount":
                        styleInd = OpenXmlSpreadsheetUtilities.NUMBER_F2_CELL;
                        break;
                }
            }
            return styleInd;
        }

        private CellValues SetCellValuesForCell(string modelName, string fieldName)
        {
            CellValues celltype = CellValues.String;
            if (modelName.Equals("OrderLine"))
            {
                switch (fieldName)
                {
                    case "QuantityOrdered":
                    case "QantityShipped":
                    case "CasePrice":
                    case "PackagePrice":
                        celltype = CellValues.Number;
                        break;
                }
            }
            else if (modelName.Equals("ListItemModel"))
            {
                switch (fieldName)
                {
                    case "Price":
                    case "parlevel":
                        celltype = CellValues.Number;
                        break;
                }
            }
            else if (modelName.Equals("Product"))
            {
                switch (fieldName)
                {
                    case "UnitCost":
                    case "CasePrice":
                    case "PackagePrice":
                        celltype = CellValues.Number;
                        break;
                }
            }
            else if (modelName.Equals("InvoiceModel"))
            {
                switch (fieldName)
                {
                    case "InvoiceAmount":
                    case "Amount":
                        celltype = CellValues.Number;
                        break;
                }
            }
            else if (modelName.Equals("Order"))
            {
                switch (fieldName)
                {
                    case "OrderTotal":
                        celltype = CellValues.Number;
                        break;
                }
            }
            return celltype;
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }
        #endregion

        #region CSV and Tab Export
        private void WriteItemRecord(StringBuilder sb, T item, string exportType)
        {
            List<string> itemRecord = new List<string>();

            var properties = item.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

            foreach (var config in exportConfig.OrderBy(e => e.Order))
            {
                //Base or Sub property?
                var propertyName = config.Field.Split('.');

                if (propertyName.Length == 1)
                { // fix in general for exports; this adds correction in pricing to both text-based and excel-based exports
                    ExportModelConfiguration thisConfig = new ExportModelConfiguration()
                    { // just a shallow copy
                        Field = config.Field,
                        Label = config.Label,
                        Order = config.Order,
                        Selected = config.Selected
                    };
                    SetPriceConfig(properties, item, thisConfig);
                    var property = properties.Where(p => p.Name.Equals(thisConfig.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (property != null)
                    {
                        itemRecord.Add(string.Format("\"{0}\"", this.GetFieldValue(item, property).Trim()));
                    }
                }
                else
                {
                    var childProp = properties.Where(p => p.Name.Equals(propertyName[0])).FirstOrDefault();
                    var childProperties = childProp.PropertyType.GetProperties();
                    var subProperty = childProperties.Where(p => p.Name.Equals(propertyName[1])).FirstOrDefault();
                    if (subProperty != null)
                    {
                        itemRecord.Add(string.Format("\"{0}\"", this.GetFieldValue(childProp.GetValue(item), subProperty).Trim()));
                    }

                }
            }

            if (itemRecord.Count > 0)
                sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) ? "," : "\t", itemRecord));
        }

        private void WriteTotalRecord(StringBuilder sb, string exportType)
        {
            sb.Append("\"Total:\"");
            sb.Append(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) ? "," : "\t");
            decimal total = 0;
            foreach (var item in this.Model)
            {
                if (item != null)
                {
                    var properties = item.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

                    if (typeof(T).Name.Equals("ItemUsageReportItemModel"))
                    {
                        var property = properties.Where(p => p.Name.Equals("TotalCost", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        total += decimal.Parse(this.GetFieldValue(item, property).Trim());
                    }
                }
            }
            sb.AppendLine("\"" + total.ToString() + "\"");
        }

        private uint AddTotalRowExcel(uint rowIndex, string[] excelColumnNames, SheetData sheetData)
        {
            rowIndex++;
            var totalRow = new Row { RowIndex = rowIndex };  // add a row at the to name the fields of spreadsheet
            OpenXmlSpreadsheetUtilities.AppendTextCell
                (excelColumnNames[12] + rowIndex.ToString(), "Total:", totalRow, CellValues.String, OpenXmlSpreadsheetUtilities.BOLD_CELL);
            decimal total = 0;
            foreach (var item in this.Model)
            {
                if (item != null)
                {
                    var properties = item.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

                    if (typeof(T).Name.Equals("ItemUsageReportItemModel"))
                    {
                        var property = properties.Where(p => p.Name.Equals("TotalCost", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        total += decimal.Parse(this.GetFieldValue(item, property).Trim());
                    }
                }
            }
            OpenXmlSpreadsheetUtilities.AppendTextCell
                (excelColumnNames[13] + rowIndex.ToString(), total.ToString(), totalRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
            sheetData.Append(totalRow);
            return rowIndex;
        }

        private string GetFieldValue(object item, PropertyInfo property)
        {
            if (item == null)
                return string.Empty;

            var value = property.GetValue(item);
            if (value == null)
                return string.Empty;

            if (value.GetType().IsEnum)
                return this.GetAttributeFieldValue(value.GetType(), value.ToString());

            if (property.PropertyType == typeof(Boolean))
            {
                return value.ToString().Equals("False") ? "N" : "Y";
            }
            else if (property.PropertyType == typeof(Boolean?) && value != null)
            {
                return value.ToString().Equals("False") ? "N" : "Y";
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                return ((DateTime)value).ToShortDateString();
            }
            else if (property.PropertyType == typeof(DateTime?) && value != null)
            {
                return ((DateTime)value).ToShortDateString();
            }
            return value.ToString();
        }

        private string GetAttributeFieldValue(Type enumerationType, string fieldName)
        {
            FieldInfo field = enumerationType.GetField(fieldName);
            if (field != null)
            {
                DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null)
                    return attr.Description;
            }
            return fieldName;
        }

        private void WriteHeaderRecord(StringBuilder sb, string exportType)
        {
            var headerRecord = new List<string>();

            var properties = typeof(T).GetProperties();

            foreach (var config in exportConfig.OrderBy(e => e.Order))
            {

                var propertyName = config.Field.Split('.');

                if (propertyName.Length == 1)
                {

                    var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (property != null)
                        headerRecord.Add(string.Format("\"{0}\"", GetPropertyDescription(property).Trim()));
                }
                else
                {
                    var childProp = properties.Where(p => p.Name.Equals(propertyName[0])).FirstOrDefault();
                    var childProperties = childProp.PropertyType.GetProperties();
                    var subProperty = childProperties.Where(p => p.Name.Equals(propertyName[1])).FirstOrDefault();
                    if (subProperty != null)
                        headerRecord.Add(string.Format("\"{0}\"", GetPropertyDescription(subProperty).Trim()));
                }
            }

            sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) ? "," : "\t", headerRecord));
        }

        private static string GetPropertyDescription(PropertyInfo property)
        {
            var description = property.GetCustomAttribute<DescriptionAttribute>();
            string value = string.Empty;
            if (description != null)
                value = description.Description;
            else
                value = property.Name;
            return value;
        }
        #endregion
    }
}