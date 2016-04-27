using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                if (typeof(T).Name.Equals("ItemUsageReportItemModel"))
                {
                    sb.AppendLine("Item Usage Report");
                    Customer customer = _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId);
                    List<string> cust = new List<string>();
                    cust.Add(customer.CustomerBranch);
                    cust.Add(customer.CustomerNumber);
                    cust.Add(customer.CustomerName);
                    sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) ? "," : "\t", cust));
                }
                this.WriteHeaderRecord(sb, exportType);
            }

            if (this.Model != null && this.Model.Count > 0) // is there any data to render
            {
                foreach (var item in this.Model)
                {
                    if (item != null)
                        this.WriteItemRecord(sb, item, exportType);
                }

            }
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            sw.Write(sb.ToString());
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
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
            foreach (ExportModelConfiguration config in exportConfig)
            {
                colIndex++;
                width = 0;
                if (modelName.Equals("ItemUsageReportItemModel"))
                {
                    switch (config.Field)
                    {
                        case "Name":
                        case "Brand":
                        case "Class":
                        case "ManufacturerName":
                            width = 25;
                            break;
                        case "UPC":
                            width = 15;
                            break;
                        case "PackSize":
                            width = 12;
                            break;
                    }
                }
                if (modelName.Equals("ListItemModel"))
                {
                    switch (config.Field)
                    {
                        case "Name":
                        case "Brand":
                        case "ItemClass":
                        case "Notes":
                            width = 20;
                            break;
                        case "Pack":
                            width = 8;
                            break;
                        case "Size":
                            width = 12;
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
            rowIndex = OpenXmlSpreadsheetUtilities.AddTitleRow
                (rowIndex, typeof(T).Name, excelColumnNames, "Item Usage Report", sheetData);
            rowIndex = OpenXmlSpreadsheetUtilities.AddCustomerRow
                (rowIndex, typeof(T).Name, excelColumnNames, _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId), sheetData);

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
                            var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                            uint styleInd = SetStyleForCell(typeof(T).Name, config.Field);
                            if (property != null)
                            {
                                OpenXmlSpreadsheetUtilities.AppendTextCell
                                    (excelColumnNames[columnIndex] + rowIndex.ToString(), this.GetFieldValue(item, property).Trim(), newExcelRow, CellValues.String, styleInd);
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
            return sheetData;
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
                    case "Name":
                    case "Brand":
                    case "Class":
                    case "ManufacturerName":
                        styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                        break;
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
                    case "Name":
                    case "Brand":
                    case "ItemClass":
                    case "Notes":
                        styleInd = OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL;
                        break;
                    case "Pack":
                        styleInd = OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL;
                        break;
                }
            }
            return styleInd;
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
                {
                    var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
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
                return value.ToString().Equals("False") ? "N" : "Y";

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