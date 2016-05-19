﻿using DocumentFormat.OpenXml.Spreadsheet;
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
                if (item.PackSize != null && item.PackSize.IndexOf('/') > -1)
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

            string deviceInfo = KeithLink.Svc.Core.Constants.SET_REPORT_SIZE_LANDSCAPE;
            var bytes = rv.LocalReport.Render("PDF", deviceInfo);

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
            sb.AppendLine("Inventory Valuation Report");
            Customer customer = _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId);
            sb.AppendLine(string.Format("{0}{1}{2}{3}{4}",
                customer.CustomerName, UseAppropriateDelimiter(request),
                customer.CustomerNumber, UseAppropriateDelimiter(request),
                customer.CustomerBranch));
            sb.AppendLine(WriteTextHeaders(request).ToString());
            sb.Append(WriteTextData(request).ToString());
            sb.AppendLine(WriteTextSum(request).ToString());
            return sb;
        }

        private StringBuilder WriteTextSum(InventoryValuationRequestModel request)
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(WriteTextValue("Total", request, true));
            ret.Append(WriteTextValue(request.ReportData.Sum(x => x.ExtPrice).ToString("F2"), request, false));
            return ret;
        }

        private StringBuilder WriteTextData(InventoryValuationRequestModel request)
        {
            StringBuilder ret = new StringBuilder();
            foreach (var datarow in request.ReportData)
            {
                ret.Append(WriteTextValue(datarow.ItemId, request, true));
                ret.Append(WriteTextValue(datarow.Name, request, true));
                ret.Append(WriteTextValue(datarow.Brand, request, true));
                ret.Append(WriteTextValue(datarow.Category, request, true));
                ret.Append(WriteTextValue(datarow.Pack, request, true));
                ret.Append(WriteTextValue(datarow.Size, request, true));
                ret.Append(WriteTextValue(datarow.Label, request, true));
                ret.Append(WriteTextValue((datarow.Each ? "Y" : "N"), request, true));
                ret.Append(WriteTextValue(datarow.Price.ToString("F2"), request, true));
                ret.Append(WriteTextValue(datarow.Quantity.ToString("F0"), request, true));
                ret.AppendLine(WriteTextValue(datarow.ExtPrice.ToString("F2"), request, false));
            }
            return ret;
        }

        private StringBuilder WriteTextHeaders(InventoryValuationRequestModel request)
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(WriteTextValue("Item", request, true));
            ret.Append(WriteTextValue("Name", request, true));
            ret.Append(WriteTextValue("Brand", request, true));
            ret.Append(WriteTextValue("Category", request, true));
            ret.Append(WriteTextValue("Pack", request, true));
            ret.Append(WriteTextValue("Size", request, true));
            ret.Append(WriteTextValue("Label", request, true));
            ret.Append(WriteTextValue("Each", request, true));
            ret.Append(WriteTextValue("Price", request, true));
            ret.Append(WriteTextValue("Quantity", request, true));
            ret.Append(WriteTextValue("ExtPrice", request, false));

            return ret;
        }

        private string WriteTextValue(string Value, InventoryValuationRequestModel request, bool endWithDelimiter)
        {
            return (endWithDelimiter) ? Value + UseAppropriateDelimiter(request) : Value;
        }

        private string UseAppropriateDelimiter(InventoryValuationRequestModel request)
        {
            string ret;
            switch (request.ReportFormat.ToLower())
            {
                case "csv":
                    ret = ",";
                    break;
                default:
                    ret = "\t";
                    break;
            }
            return ret;
        }

        #region Generate Excel Report
        private MemoryStream GenerateExcelReport(List<InventoryValuationModel> data)
        {
            MemoryStream stream = OpenXmlSpreadsheetUtilities.MakeSpreadSheet
                (SetCustomColumnWidths(new DocumentFormat.OpenXml.Spreadsheet.Worksheet()),
                 WriteDataTableToExcelWorksheet(data),
                 "InventoryValuationModel");
            return stream;
        }

        private Worksheet SetCustomColumnWidths(Worksheet workSheet)
        {
            OpenXmlSpreadsheetUtilities.SetColumnWidth(workSheet, 2, 20);
            OpenXmlSpreadsheetUtilities.SetColumnWidth(workSheet, 3, 20);
            OpenXmlSpreadsheetUtilities.SetColumnWidth(workSheet, 4, 20);
            OpenXmlSpreadsheetUtilities.SetColumnWidth(workSheet, 7, 20);
            OpenXmlSpreadsheetUtilities.SetColumnWidth(workSheet, 11, 10);
            return workSheet;
        }

        private SheetData WriteDataTableToExcelWorksheet(List<InventoryValuationModel> data)
        {
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 1;

            string[] excelColumnNames = new string[11];
            excelColumnNames[0] = "Item";
            excelColumnNames[1] = "Name";
            excelColumnNames[2] = "Brand";
            excelColumnNames[3] = "Category";
            excelColumnNames[4] = "Pack";
            excelColumnNames[5] = "Size";
            excelColumnNames[6] = "Label";
            excelColumnNames[7] = "Each";
            excelColumnNames[8] = "Quantity";
            excelColumnNames[9] = "Price";
            excelColumnNames[10] = "ExtPrice";

            rowIndex = OpenXmlSpreadsheetUtilities.AddTitleRow
                (rowIndex, "InventoryValuationModel", excelColumnNames, "Inventory Valuation Report", sheetData);
            rowIndex = OpenXmlSpreadsheetUtilities.AddCustomerRow
                (rowIndex, "InventoryValuationModel", excelColumnNames, _customerRepo.GetCustomerByCustomerNumber(_context.CustomerId, _context.BranchId), sheetData);

            var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
            sheetData.Append(headerRow);

            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[0] + rowIndex.ToString(), "Item", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[1] + rowIndex.ToString(), "Name", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[2] + rowIndex.ToString(), "Brand", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[3] + rowIndex.ToString(), "Category", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[4] + rowIndex.ToString(), "Pack", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[5] + rowIndex.ToString(), "Size", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), "Label", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), "Each", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[8] + rowIndex.ToString(), "Quantity", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[9] + rowIndex.ToString(), "Price", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[10] + rowIndex.ToString(), "Ext. Price", headerRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);


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
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[2] + rowIndex.ToString(), item.Brand, newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[3] + rowIndex.ToString(), item.Category, newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.TEXT_WRAP_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[4] + rowIndex.ToString(), item.Pack, newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[5] + rowIndex.ToString(), item.Size, newExcelRow);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), item.Label, newExcelRow);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), item.Each ? "Y" : "N", newExcelRow);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[8] + rowIndex.ToString(), item.Quantity.ToString(), newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[9] + rowIndex.ToString(), Math.Round(item.Price, 2).ToString("F2"), newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
                    OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[10] + rowIndex.ToString(), Math.Round(item.ExtPrice, 2).ToString("F2"), newExcelRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);

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
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), "", newRow);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[8] + rowIndex.ToString(), "Total", newRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[9] + rowIndex.ToString(), Math.Round(data.Sum(s => s.Price), 2).ToString("F2"), newRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
            OpenXmlSpreadsheetUtilities.AppendTextCell(excelColumnNames[10] + rowIndex.ToString(), Math.Round(data.Sum(s => s.ExtPrice), 2).ToString("F2"), newRow, CellValues.String, OpenXmlSpreadsheetUtilities.RIGHT_ALIGNED_CELL);
            return sheetData;
        }
        #endregion
    }
}