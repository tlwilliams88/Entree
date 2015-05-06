using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Reports;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Reports
{
	public class InventoryValuationReportLogicImpl: IInventoryValuationReportLogic
	{
		public MemoryStream GenerateReport(InventoryValuationRequestModel request)
		{
			if (request.ReportFormat.Equals("excel", StringComparison.InvariantCultureIgnoreCase))
				return GenerateExcelReport(request.ReportData);
			else
				return GeneratePDFReport(request.ReportData);
		}

		private MemoryStream GeneratePDFReport(List<InventoryValuationModel> data)
		{
			ReportViewer rv = new ReportViewer();

			rv.ProcessingMode = ProcessingMode.Local;

			Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
			Stream rdlcStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Reports.InventoryValuation.rdlc");
			rv.LocalReport.LoadReportDefinition(rdlcStream);
			
			rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

			var bytes = rv.LocalReport.Render("PDF");

			return new MemoryStream(bytes);
		}

		#region Generate Excel Report
		private MemoryStream GenerateExcelReport(List<InventoryValuationModel> data)
		{
			MemoryStream stream = new MemoryStream();
			using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, true))
			{
				document.AddWorkbookPart();
				document.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

				document.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

				WorkbookStylesPart workbookStylesPart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
				Stylesheet stylesheet = new Stylesheet();
				workbookStylesPart.Stylesheet = stylesheet;

				uint worksheetNumber = 1;

				string workSheetID = "rId" + worksheetNumber.ToString();
				string worksheetName = "Export";

				WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
				newWorksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

				newWorksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

				WriteDataTableToExcelWorksheet(newWorksheetPart, data);
				newWorksheetPart.Worksheet.Save();

				if (worksheetNumber == 1)
					document.WorkbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

				document.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
				{
					Id = document.WorkbookPart.GetIdOfPart(newWorksheetPart),
					SheetId = (uint)worksheetNumber,
					Name = worksheetName
				});
				
				document.WorkbookPart.Workbook.Save();

				
			}

			stream.Flush();
			stream.Position = 0;
			return stream;
		}

		private void WriteDataTableToExcelWorksheet(WorksheetPart worksheetPart, List<InventoryValuationModel> data)
		{
			var worksheet = worksheetPart.Worksheet;
			var sheetData = worksheet.GetFirstChild<SheetData>();

			string[] excelColumnNames = new string[8];
			excelColumnNames[0] = "Item";
			excelColumnNames[1] = "Name";
			excelColumnNames[2] = "PackSize";
			excelColumnNames[3] = "Label";
			excelColumnNames[4] = "Each";
			excelColumnNames[5] = "Price";
			excelColumnNames[6] = "Quantity";
			excelColumnNames[7] = "ExtPrice";

			//
			//  Create the Header row in our Excel Worksheet
			//
			uint rowIndex = 1;

			var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
			sheetData.Append(headerRow);

			AppendTextCell(excelColumnNames[0] + "1", "Item", headerRow);
			AppendTextCell(excelColumnNames[1] + "1", "Name", headerRow);
			AppendTextCell(excelColumnNames[2] + "1", "Pack/Size", headerRow);
			AppendTextCell(excelColumnNames[3] + "1", "Label", headerRow);
			AppendTextCell(excelColumnNames[4] + "1", "Each", headerRow);
			AppendTextCell(excelColumnNames[5] + "1", "Price", headerRow);
			AppendTextCell(excelColumnNames[6] + "1", "Quantity", headerRow);
			AppendTextCell(excelColumnNames[7] + "1", "Ext. Price", headerRow);

			
			//
			//  Now, step through each row of data in our DataTable...
			foreach (var item in data)
			{
				rowIndex++;
				var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
				sheetData.Append(newExcelRow);
				if (item != null)
				{

					AppendTextCell(excelColumnNames[0] + rowIndex.ToString(), item.ItemId, newExcelRow);
					AppendTextCell(excelColumnNames[1] + rowIndex.ToString(), item.Name, newExcelRow);
					AppendTextCell(excelColumnNames[2] + rowIndex.ToString(), item.PackSize, newExcelRow);
					AppendTextCell(excelColumnNames[3] + rowIndex.ToString(), item.Label, newExcelRow);
					AppendTextCell(excelColumnNames[4] + rowIndex.ToString(), item.Each ? "Y" : "N", newExcelRow);
					AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), item.Price.ToString(), newExcelRow, CellValues.Number);
					AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), item.Quantity.ToString(), newExcelRow, CellValues.Number);
					AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), item.ExtPrice.ToString(), newExcelRow, CellValues.Number);

				}
			}

			rowIndex++;
			var newRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
			sheetData.Append(newRow);

			AppendTextCell(excelColumnNames[0] + rowIndex.ToString(), "", newRow);
			AppendTextCell(excelColumnNames[1] + rowIndex.ToString(), "", newRow);
			AppendTextCell(excelColumnNames[2] + rowIndex.ToString(), "", newRow);
			AppendTextCell(excelColumnNames[3] + rowIndex.ToString(), "", newRow);
			AppendTextCell(excelColumnNames[4] + rowIndex.ToString(), "", newRow);
			AppendTextCell(excelColumnNames[5] + rowIndex.ToString(), "", newRow);
			AppendTextCell(excelColumnNames[6] + rowIndex.ToString(), "Total", newRow);
			AppendTextCell(excelColumnNames[7] + rowIndex.ToString(), data.Sum(s => s.ExtPrice).ToString(), newRow, CellValues.Number);

		}

		private void AppendTextCell(string cellReference, string cellStringValue, Row excelRow, CellValues dataType = CellValues.String)
		{
			//  Add a new Excel Cell to our Row 
			Cell cell = new Cell() { CellReference = cellReference, DataType = dataType };
			CellValue cellValue = new CellValue();
			cellValue.Text = cellStringValue;
			cell.Append(cellValue);
			excelRow.Append(cell);
		}

		#endregion
	}
}
