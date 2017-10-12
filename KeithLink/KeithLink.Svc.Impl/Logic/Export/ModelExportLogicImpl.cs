using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;

using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Impl.Logic.Export
{
    public class ModelExportLogicImpl<T> : IModelExportLogic<T> where T : class, IExportableModel
    {
        private IList<T> Model { get; set; }
        private List<ExportModelConfiguration> exportConfig = null;
        private ICustomerRepository _customerRepo;
        private UserSelectedContext _context;
        private dynamic _headerInfo;

        public ModelExportLogicImpl(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public System.IO.MemoryStream Export(IList<T> model, List<ExportModelConfiguration> exportConfig, string exportType, UserSelectedContext context, dynamic headerInfo = null)
        {
            this.Model = model;
            this.exportConfig = exportConfig;
            this._context = context;
            if (headerInfo != null)
            {
                _headerInfo = headerInfo;
            }
            return Export(exportType);
        }

        public System.IO.MemoryStream Export(IList<T> model, string exportType, UserSelectedContext context, dynamic headerInfo = null)
        {
            this.Model = model;
            this.exportConfig = model.First().DefaultExportConfiguration();
            this._context = context;
            if (headerInfo != null)
            {
                _headerInfo = headerInfo;
            }
            return Export(exportType);
        }

        private MemoryStream Export(string exportType)
        {
            MemoryStream ms = null;

            if (exportType.Equals("excel", StringComparison.CurrentCultureIgnoreCase))
                ms = GenerateExcelExport();

            if (exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) || exportType.Equals("tab", StringComparison.CurrentCultureIgnoreCase))
            {
                ms = GenerateTextReport(exportType);
            }

            return ms;
        }

        #region Text Report
        private MemoryStream GenerateTextReport(string exportType)
        {
            StringBuilder sb = new StringBuilder();

            AddTitleToTextExport(exportType, sb);
            AddCustomerToTextExport(exportType, sb);
            if (typeof(T).Name.Equals("OrderLine"))
            {
                AddOrderHeaderToTextExport(exportType, sb);
            }
            if (typeof(T).Name.Equals("InvoiceItemModel"))
            {
                AddInvoiceHeaderToTextExport(exportType, sb);
            }

            WriteHeaderRecord(sb, exportType);

            if (this.Model != null &&
                this.Model.Count > 0) // is there any data to render
            {
                foreach (var item in this.Model)
                {
                    if (item != null)
                        WriteItemRecord(sb, item, exportType);
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

        private void AddOrderHeaderToTextExport(string exportType, StringBuilder sb)
        {
            Order headerInfo = (Order)_headerInfo;
            sb.AppendLine(string.Format("ORDER INFORMATION (System: {0})", headerInfo.OrderSystem));
            sb.AppendLine(string.Format("  Invoice # {0}", headerInfo.InvoiceNumber));
            sb.AppendLine(string.Format("  Order Date {0}", headerInfo.CreatedDate.ToString("ddd-M-dd-yy")));
            sb.AppendLine(string.Format("  Delivery Date {0}", DateTime.Parse(headerInfo.DeliveryDate).ToString("ddd-M-dd-yy")));
            sb.AppendLine(string.Format("  Subtotal ${0}", headerInfo.OrderTotal));
            sb.AppendLine(string.Format("  Invoice Status {0}", headerInfo.InvoiceStatus));
            sb.AppendLine(string.Format("  Order Status {0}", headerInfo.Status));
            sb.AppendLine(string.Format("  Requested Ship Date {0}", DateTime.Parse(headerInfo.RequestedShipDate).ToString("ddd-M-dd-yy")));
            sb.AppendLine(string.Format("  Delivered {0}", headerInfo.ActualDeliveryTime));
            sb.AppendLine(string.Format("  Items {0} Items / {1} Pieces", headerInfo.ItemCount, headerInfo.Items.Sum(i => i.Quantity)));
            sb.AppendLine(string.Format("  PO Number {0}", headerInfo.PONumber));
        }

        private void AddInvoiceHeaderToTextExport(string exportType, StringBuilder sb)
        {
            InvoiceModel headerInfo = (InvoiceModel)_headerInfo;
            sb.AppendLine("INVOICE INFORMATION");
            sb.AppendLine(string.Format("  Invoice,# {0}", headerInfo.InvoiceNumber));
            sb.AppendLine(string.Format("  Invoice Status,{0}", headerInfo.Status));
            sb.AppendLine(string.Format("  Amount Due,${0}", headerInfo.Amount));
            sb.AppendLine(string.Format("  Order Date,{0}", headerInfo.OrderDate.Value.ToString("ddd-M-dd-yy")));
            sb.AppendLine(string.Format("  PO Number,{0}", headerInfo.PONumber));
            sb.AppendLine(string.Format("  Type,{0}", headerInfo.Type));
            sb.AppendLine(string.Format("  Due Date,{0}", headerInfo.DueDate.Value.ToString("ddd-M-dd-yy")));
            sb.AppendLine(string.Format("  Items,{0}", headerInfo.Items.Count));
            sb.AppendLine(string.Format("  Ship Date,{0}", headerInfo.InvoiceDate.Value.ToString("ddd-M-dd-yy")));
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
            if (this.exportConfig.Where(e => e.Field == "TotalCost").Count() > 0 && this.exportConfig.Count > 1)
            {
                rowIndex++;
                var totalRow = new Row { RowIndex = rowIndex };  // add a row at the to name the fields of spreadsheet
                OpenXmlSpreadsheetUtilities.AppendTextCell
                    (excelColumnNames[this.exportConfig.Count - 2] + rowIndex.ToString(), "Sum Total Cost:", totalRow, CellValues.String, OpenXmlSpreadsheetUtilities.BOLD_CELL);
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
            }
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

        private string GetPropertyDescription(PropertyInfo property)
        {
            var description = property.GetCustomAttribute<DescriptionAttribute>();
            string value = string.Empty;
            if (description != null)
                value = description.Description;
            else
                value = property.Name;
            return value;
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
        #endregion

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
                switch (modelName)
                {
                    case ("OrderLine"):
                        width = OrderLine.SetWidths(config, width);
                        break;
                    case ("ShoppingCartItem"):
                        width = ShoppingCartItem.SetWidths(config, width);
                        break;
                    case ("InvoiceItemModel"):
                        width = InvoiceItemModel.SetWidths(config, width);
                        break;
                    case ("Product"):
                        width = Product.SetWidths(config, width);
                        break;
                    case ("Order"):
                        width = Order.SetWidths(config, width);
                        break;
                    case ("ItemUsageReportItemModel"):
                        width = ItemUsageReportItemModel.SetWidths(config, width);
                        break;
                    case ("InvoiceModel"):
                        width = InvoiceModel.SetWidths(config, width);
                        break;
                    case ("ListItemModel"):
                        width = ListItemModel.SetWidths(config, width);
                        break;
                }

                if (width > 0)
                    OpenXmlSpreadsheetUtilities.SetColumnWidth(workSheet, colIndex, width);
            }
            return workSheet;
        }

        private SheetData WriteDataTableToExcelWorksheet()
        {
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            int numberOfColumns = this.exportConfig.Count;

            string[] excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            uint rowIndex = 1;
            //
            //  Create the Header row in our Excel Worksheet
            //
            rowIndex = WriteHeaderToExcelWorksheet(rowIndex, sheetData, excelColumnNames);

            PropertyInfo[] properties = WriteDataFieldsHeaderToExcelWorksheet(rowIndex, sheetData, excelColumnNames);

            //
            //  Now, step through each row of data in our DataTable...

            rowIndex = WriteDataFieldsDataRowsToExcelWorksheet(rowIndex, sheetData, properties, excelColumnNames);
            if (typeof(T).Name.Equals("ItemUsageReportItemModel"))
            {
                AddTotalRowExcel(rowIndex, excelColumnNames, sheetData);
            }
            return sheetData;
        }

        private uint WriteDataFieldsDataRowsToExcelWorksheet(uint rowIndex, SheetData sheetData, PropertyInfo[] properties, string[] excelColumnNames)
        {
            int columnIndex;

            foreach (var item in this.Model)
            {
                rowIndex++;
                var newExcelRow = new Row { RowIndex = rowIndex }; // add a row at the top of spreadsheet
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
                            {
                                // just a shallow copy
                                Field = config.Field,
                                Label = config.Label,
                                Order = config.Order,
                                Selected = config.Selected
                            };
                            SetPriceConfig(properties, item, thisConfig);
                            var property = properties.Where(p => p.Name.Equals(thisConfig.Field, StringComparison.CurrentCultureIgnoreCase))
                                                     .FirstOrDefault();
                            uint styleInd = SetStyleForCell(typeof(T).Name, thisConfig.Field);
                            CellValues celltype = SetCellValueTypesForCell(typeof(T).Name, thisConfig.Field);
                            if (property != null)
                            {
                                OpenXmlSpreadsheetUtilities.AppendTextCell
                                        (excelColumnNames[columnIndex] + rowIndex.ToString(),
                                         this.GetFieldValue(item, property)
                                             .Trim(), newExcelRow,
                                         celltype,
                                         styleInd);
                            }
                        }
                        else
                        {
                            var childProp = properties.Where(p => p.Name.Equals(propertyName[0]))
                                                      .FirstOrDefault();
                            var childProperties = childProp.PropertyType.GetProperties();
                            var subProperty = childProperties.Where(p => p.Name.Equals(propertyName[1]))
                                                             .FirstOrDefault();
                            if (subProperty != null)
                            {
                                OpenXmlSpreadsheetUtilities.AppendTextCell
                                        (excelColumnNames[columnIndex] + rowIndex.ToString(), this.GetFieldValue(childProp.GetValue(item), subProperty)
                                                                                                  .Trim(), newExcelRow);
                            }
                        }

                        columnIndex++;
                    }
                }
            }
            return rowIndex;
        }

        private PropertyInfo[] WriteDataFieldsHeaderToExcelWorksheet(uint rowIndex, SheetData sheetData, string[] excelColumnNames)
        {
            var headerRow = new Row { RowIndex = rowIndex }; // add a row at the to name the fields of spreadsheet
            sheetData.Append(headerRow);

            var properties = typeof(T).GetProperties();
            int columnIndex = 0;
            foreach (var config in exportConfig.OrderBy(e => e.Order))
            {
                var propertyName = config.Field.Split('.');
                uint styleInd = SetStyleForHeaderCell(typeof(T).Name, config.Field);

                if (propertyName.Length == 1)
                {
                    var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase))
                                             .FirstOrDefault();
                    if (property != null)
                    {
                        OpenXmlSpreadsheetUtilities.AppendTextCell
                                (excelColumnNames[columnIndex] + rowIndex.ToString(), GetPropertyDescription(property)
                                         .Trim(), headerRow, CellValues.String, styleInd);
                        columnIndex++;
                    }
                }
                else
                {
                    var childProp = properties.Where(p => p.Name.Equals(propertyName[0]))
                                              .FirstOrDefault();
                    var childProperties = childProp.PropertyType.GetProperties();
                    var subProperty = childProperties.Where(p => p.Name.Equals(propertyName[1]))
                                                     .FirstOrDefault();
                    if (subProperty != null)
                    {
                        OpenXmlSpreadsheetUtilities.AppendTextCell
                                (excelColumnNames[columnIndex] + rowIndex.ToString(), GetPropertyDescription(subProperty)
                                         .Trim(), headerRow);
                        columnIndex++;
                    }
                }
            }
            return properties;
        }

        private uint WriteHeaderToExcelWorksheet(uint rowIndex, SheetData sheetData, string[] excelColumnNames)
        {
            rowIndex = AddTitleToExcelExport(sheetData, excelColumnNames, rowIndex);
            rowIndex = AddCustomerToExcelExport(sheetData, excelColumnNames, rowIndex);
            if (typeof(T).Name.Equals("OrderLine"))
            {
                rowIndex = AddOrderHeaderToExcelExport(sheetData, excelColumnNames, rowIndex);
            }
            if (typeof(T).Name.Equals("InvoiceItemModel"))
            {
                rowIndex = AddInvoiceHeaderToExcelExport(sheetData, excelColumnNames, rowIndex);
            }
            return rowIndex;
        }

        private uint AddCustomerToExcelExport(SheetData sheetData, string[] excelColumnNames, uint rowIndex)
        {
            List<string> exports = Configuration.ExportAddCustomer;
            foreach (string gettitle in exports)
            {
                if (gettitle.Equals(typeof(T).Name) && excelColumnNames.Length > 2)
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
                if (gettitle.StartsWith(typeof(T).Name) && excelColumnNames.Length > 0)
                {
                    rowIndex = OpenXmlSpreadsheetUtilities.AddTitleRow
                        (rowIndex, typeof(T).Name, excelColumnNames, gettitle.Substring(gettitle.IndexOf(';') + 1), sheetData);
                }
            }

            return rowIndex;
        }

        private uint AddOrderHeaderToExcelExport(SheetData sheetData, string[] excelColumnNames, uint rowIndex)
        {
            Order headerInfo = (Order)_headerInfo;
            List<string> list = new List<string>();
            list.Add(string.Format("ORDER INFORMATION (System: {0})", headerInfo.OrderSystem));
            list.Add(string.Format("Invoice # {0}", headerInfo.InvoiceNumber));
            list.Add(string.Format("Order Date {0}", headerInfo.CreatedDate.ToString("ddd, M-dd-yy")));
            list.Add(string.Format("Delivery Date {0}", DateTime.Parse(headerInfo.DeliveryDate).ToString("ddd, M-dd-yy")));
            list.Add(string.Format("Subtotal ${0}", headerInfo.OrderTotal));
            list.Add(string.Format("Invoice Status {0}", headerInfo.InvoiceStatus));
            list.Add(string.Format("Order Status {0}", headerInfo.Status));
            list.Add(string.Format("Requested Ship Date {0}", DateTime.Parse(headerInfo.RequestedShipDate).ToString("ddd, M-dd-yy")));
            list.Add(string.Format("Delivered {0}", headerInfo.ActualDeliveryTime));
            list.Add(string.Format("Items {0} Items / {1} Pieces", headerInfo.ItemCount, headerInfo.Items.Sum(i => i.Quantity)));
            list.Add(string.Format("PO Number {0}", headerInfo.PONumber));

            if (excelColumnNames.Length > 0)
            {
                rowIndex = OpenXmlSpreadsheetUtilities.AddHeaderRows
                    (rowIndex, typeof(T).Name, excelColumnNames, list, sheetData);
            }

            return rowIndex;
        }

        private uint AddInvoiceHeaderToExcelExport(SheetData sheetData, string[] excelColumnNames, uint rowIndex)
        {
            InvoiceModel headerInfo = (InvoiceModel)_headerInfo;
            List<string> list = new List<string>();
            list.Add("INVOICE INFORMATION");
            list.Add(string.Format("  Invoice,# {0}", headerInfo.InvoiceNumber));
            list.Add(string.Format("  Invoice Status,{0}", headerInfo.Status));
            list.Add(string.Format("  Amount Due,${0}", headerInfo.Amount));
            list.Add(string.Format("  Order Date,{0}", headerInfo.OrderDate.Value.ToString("ddd-M-dd-yy")));
            list.Add(string.Format("  PO Number,{0}", headerInfo.PONumber));
            list.Add(string.Format("  Type,{0}", headerInfo.Type));
            list.Add(string.Format("  Due Date,{0}", headerInfo.DueDate.Value.ToString("ddd-M-dd-yy")));
            list.Add(string.Format("  Items,{0}", headerInfo.Items.Count));
            list.Add(string.Format("  Ship Date,{0}", headerInfo.InvoiceDate.Value.ToString("ddd-M-dd-yy")));

            if (excelColumnNames.Length > 0)
            {
                rowIndex = OpenXmlSpreadsheetUtilities.AddHeaderRows
                    (rowIndex, typeof(T).Name, excelColumnNames, list, sheetData);
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
                styleInd = ItemUsageReportItemModel.SetStyleForHeader(fieldName, styleInd);
            }
            else if (modelName.Equals("ListItemModel"))
            {
                styleInd = ListItemModel.SetStyleForHeader(fieldName, styleInd);
            }
            else if (modelName.Equals("InvoiceItemModel"))
            {
                styleInd = InvoiceItemModel.SetStyleForHeader(fieldName, styleInd);
            }
            else if (modelName.Equals("Product"))
            {
                styleInd = Product.SetStyleForHeader(fieldName, styleInd);
            }
            else if (modelName.Equals("OrderLine"))
            {
                styleInd = OrderLine.SetStyleForHeader(fieldName, styleInd);
            }
            else if (modelName.Equals("ShoppingCartItem"))
            {
                styleInd = ShoppingCartItem.SetStyleForHeader(fieldName, styleInd);
            }
            else if (modelName.Equals("Order"))
            {
                styleInd = Order.SetStyleForHeader(fieldName, styleInd);
            }
            else if (modelName.Equals("InvoiceModel"))
            {
                styleInd = InvoiceModel.SetStyleForHeader(fieldName, styleInd);
            }
            return styleInd;
        }

        private uint SetStyleForCell(string modelName, string fieldName)
        {
            uint styleInd = OpenXmlSpreadsheetUtilities.DEFAULT_CELL;
            if (modelName.Equals("ItemUsageReportItemModel"))
            {
                styleInd = ItemUsageReportItemModel.SetStyleForCell(fieldName, styleInd);
            }
            else if (modelName.Equals("ListItemModel"))
            {
                styleInd = ListItemModel.SetStyleForCell(fieldName, styleInd);
            }
            else if (modelName.Equals("Product"))
            {
                styleInd = Product.SetStyleForCell(fieldName, styleInd);
            }
            else if (modelName.Equals("OrderLine"))
            {
                styleInd = OrderLine.SetStyleForCell(fieldName, styleInd);
            }
            else if (modelName.Equals("ShoppingCartItem"))
            {
                styleInd = ShoppingCartItem.SetStyleForCell(fieldName, styleInd);
            }
            else if (modelName.Equals("InvoiceItemModel"))
            {
                styleInd = InvoiceItemModel.SetStyleForCell(fieldName, styleInd);
            }
            else if (modelName.Equals("Order"))
            {
                styleInd = Order.SetStyleForCell(fieldName, styleInd);
            }
            else if (modelName.Equals("InvoiceModel"))
            {
                styleInd = InvoiceModel.SetStyleForCell(fieldName, styleInd);
            }
            return styleInd;
        }

        private CellValues SetCellValueTypesForCell(string modelName, string fieldName)
        {
            CellValues celltype = CellValues.String;
            if (modelName.Equals("OrderLine"))
            {
                celltype = OrderLine.SetCellValueTypeForCells(fieldName, celltype);
            }
            else if (modelName.Equals("ShoppingCartItem"))
            {
                celltype = ShoppingCartItem.SetCellValueTypeForCells(fieldName, celltype);
            }
            else if (modelName.Equals("ItemUsageReportItemModel"))
            {
                celltype = ItemUsageReportItemModel.SetCellValueTypeForCells(fieldName, celltype);
            }
            else if (modelName.Equals("InvoiceItemModel"))
            {
                celltype = InvoiceItemModel.SetCellValueTypeForCells(fieldName, celltype);
            }
            else if (modelName.Equals("ListItemModel"))
            {
                celltype = ListItemModel.SetCellValueTypeForCells(fieldName, celltype);
            }
            else if (modelName.Equals("Product"))
            {
                celltype = Product.SetCellValueTypeForCells(fieldName, celltype);
            }
            else if (modelName.Equals("InvoiceModel"))
            {
                celltype = InvoiceModel.SetCellValueTypeForCells(fieldName, celltype);
            }
            else if (modelName.Equals("Order"))
            {
                celltype = Order.SetCellValueTypeForCells(fieldName, celltype);
            }
            return celltype;
        }

        private string GetExcelColumnName(int columnIndex)
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
    }
}