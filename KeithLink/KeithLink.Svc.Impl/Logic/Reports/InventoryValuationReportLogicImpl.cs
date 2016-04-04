using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KeithLink.Svc.Impl.Logic.Reports
{
    public class InventoryValuationReportLogicImpl : IInventoryValuationReportLogic
    {
        private UserSelectedContext _context;
        private ICustomerRepository _customerRepo;
        public InventoryValuationReportLogicImpl(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }
        public MemoryStream GenerateReport(InventoryValuationRequestModel request)
        {
            _context = request.context;
            foreach (InventoryValuationModel item in request.ReportData)
            {
                if (item.PackSize.IndexOf('/') > -1)
                {
                    item.Pack = item.PackSize.Substring(0, item.PackSize.IndexOf('/')).Trim();
                    item.Size = item.PackSize.Substring(item.PackSize.IndexOf('/') + 1).Trim();
                }
            }
            if (request.ReportFormat.Equals("excel", StringComparison.InvariantCultureIgnoreCase))
                return GenerateExcelReport(request.ReportData);
            else if (request.ReportFormat.Equals("pdf", StringComparison.InvariantCultureIgnoreCase))
                return GeneratePDFReport(request.ReportData);
            else
                return GenerateTextReport(request);
        }

        private MemoryStream GeneratePDFReport(List<InventoryValuationModel> data)
        {
            Customer customer = _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId);
            ReportViewer rv = new ReportViewer();

            rv.ProcessingMode = ProcessingMode.Local;

            Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
            Stream rdlcStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Reports.InventoryValuation.rdlc");
            rv.LocalReport.LoadReportDefinition(rdlcStream);

            rv.LocalReport.SetParameters(new ReportParameter("Branch", customer.CustomerBranch));
            rv.LocalReport.SetParameters(new ReportParameter("CustomerName", customer.CustomerName));
            rv.LocalReport.SetParameters(new ReportParameter("CustomerNumber", customer.CustomerNumber));
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            var bytes = rv.LocalReport.Render("PDF");

            return new MemoryStream(bytes);
        }

        private MemoryStream GenerateTextReport(InventoryValuationRequestModel request)
        {
            StringBuilder sb = GenerateInventoryValuationTextReport(request);

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            sw.Write(sb.ToString());
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private StringBuilder GenerateInventoryValuationTextReport(InventoryValuationRequestModel request)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Inventory Valuation Report\n");
            WriteTextHeaders(request, sb);
            WriteTextData(request, sb);
            WriteTextSum(request, sb);
            return sb;
        }

        private void WriteTextSum(InventoryValuationRequestModel request, StringBuilder sb)
        {
            WriteTextValue("Total", request, sb, true);
            WriteTextValue(request.ReportData.Sum(x => x.ExtPrice).ToString("F2"), request, sb, false);
            sb.Append("\n");
        }

        private void WriteTextData(InventoryValuationRequestModel request, StringBuilder sb)
        {
            foreach (var datarow in request.ReportData)
            {
                WriteTextValue(datarow.ItemId, request, sb, true);
                WriteTextValue(datarow.Name, request, sb, true);
                WriteTextValue(datarow.Pack, request, sb, true);
                WriteTextValue(datarow.Size, request, sb, true);
                WriteTextValue(datarow.Label, request, sb, true);
                WriteTextValue((datarow.Each ? "Y" : "N"), request, sb, true);
                WriteTextValue(datarow.Price.ToString("F2"), request, sb, true);
                WriteTextValue(datarow.Quantity.ToString("F0"), request, sb, true);
                WriteTextValue(datarow.ExtPrice.ToString("F2"), request, sb, false);
                sb.Append("\n");
            }
        }

        private void WriteTextHeaders(InventoryValuationRequestModel request, StringBuilder sb)
        {
            WriteTextValue("Item", request, sb, true);
            WriteTextValue("Name", request, sb, true);
            WriteTextValue("Pack", request, sb, true);
            WriteTextValue("Size", request, sb, true);
            WriteTextValue("Label", request, sb, true);
            WriteTextValue("Each", request, sb, true);
            WriteTextValue("Price", request, sb, true);
            WriteTextValue("Quantity", request, sb, true);
            WriteTextValue("ExtPrice", request, sb, false);
            sb.Append("\n");
        }

        private void WriteTextValue(string Value, InventoryValuationRequestModel request, StringBuilder sb, bool endWithDelimiter)
        {
            sb.Append(Value);
            if (endWithDelimiter) UseAppropriateDelimiter(request, sb);
        }

        private void UseAppropriateDelimiter(InventoryValuationRequestModel request, StringBuilder sb)
        {
            switch (request.ReportFormat.ToLower())
            {
                case "csv":
                    sb.Append(",");
                    break;
                default:
                    sb.Append("\t");
                    break;
            }
        }

        #region Generate Excel Report
        private MemoryStream GenerateExcelReport(List<InventoryValuationModel> data)
        {
            MemoryStream stream = OpenXmlSpreadsheetUtilities.MakeSpreadSheet
                (OpenXmlSpreadsheetUtilities.SetColumnWidth(new DocumentFormat.OpenXml.Spreadsheet.Worksheet(), 2, 30),
                 WriteDataTableToExcelWorksheet(data),
                 "InventoryValuationModel");
            return stream;
        }

        private SheetData WriteDataTableToExcelWorksheet(List<InventoryValuationModel> data)
        {
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 1;

            string[] excelColumnNames = new string[9];
            excelColumnNames[0] = "Item";
            excelColumnNames[1] = "Name";
            excelColumnNames[2] = "Pack";
            excelColumnNames[3] = "Size";
            excelColumnNames[4] = "Label";
            excelColumnNames[5] = "Each";
            excelColumnNames[6] = "Quantity";
            excelColumnNames[7] = "Price";
            excelColumnNames[8] = "ExtPrice";

            rowIndex = OpenXmlSpreadsheetUtilities.AddTitleRow
                (rowIndex, "InventoryValuationModel", excelColumnNames, "Inventory Valuation Report", sheetData);
            rowIndex = OpenXmlSpreadsheetUtilities.AddCustomerRow
                (rowIndex, "InventoryValuationModel", excelColumnNames, _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId), sheetData);

            var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
            sheetData.Append(headerRow);

            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[0] + rowIndex.ToString(), "Item", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[1] + rowIndex.ToString(), "Name", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[2] + rowIndex.ToString(), "Pack", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[3] + rowIndex.ToString(), "Size", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[4] + rowIndex.ToString(), "Label", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[5] + rowIndex.ToString(), "Each", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), "Quantity", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), "Price", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[8] + rowIndex.ToString(), "Ext. Price", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);


            //
            //  Now, step through each row of data in our DataTable...
            foreach (var item in data)
            {
                rowIndex++;
                var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
                sheetData.Append(newExcelRow);
                if (item != null)
                {

                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[0] + rowIndex.ToString(), item.ItemId, newExcelRow);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[1] + rowIndex.ToString(), item.Name, newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[2] + rowIndex.ToString(), item.Pack, newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[3] + rowIndex.ToString(), item.Size, newExcelRow);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[4] + rowIndex.ToString(), item.Label, newExcelRow);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[5] + rowIndex.ToString(), item.Each ? "Y" : "N", newExcelRow);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), item.Quantity.ToString(), newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), Math.Round(item.Price, 2).ToString("F2"), newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[8] + rowIndex.ToString(), Math.Round(item.ExtPrice, 2).ToString("F2"), newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);

                }
            }

            rowIndex++;
            var newRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
            sheetData.Append(newRow);

            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[0] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[1] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[2] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[3] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[4] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[5] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), "Total", newRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), Math.Round(data.Sum(s => s.Price), 2).ToString("F2"), newRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[8] + rowIndex.ToString(), Math.Round(data.Sum(s => s.ExtPrice), 2).ToString("F2"), newRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
            return sheetData;
        }
        #endregion
    }
}