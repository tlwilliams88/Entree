using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Export
{
	public class ModelExportLogicImpl<T> : IModelExportLogic<T> where T: class, IExportableModel
	{
		private IList<T> Model  { get; set; }
		private List<ExportModelConfiguration> exportConfig = null;

		public System.IO.MemoryStream Export(IList<T> model, List<ExportModelConfiguration> exportConfig, string exportType)
		{
			this.Model = model;
			this.exportConfig = exportConfig;

			return Export(exportType);
		}

		public System.IO.MemoryStream Export(IList<T> model, string exportType)
		{
			this.Model = model;
			this.exportConfig = model.First().DefaultExportConfiguration();
			return Export(exportType);
		}

		private MemoryStream Export(string exportType)
		{
			StringBuilder sb = new StringBuilder();

			if (exportType.Equals("excel", StringComparison.CurrentCultureIgnoreCase))
				return this.GenerateExcelExport();

			if (exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) || exportType.Equals("tab", StringComparison.CurrentCultureIgnoreCase))
				this.WriteHeaderRecord(sb, exportType);

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
			MemoryStream stream = new MemoryStream();
			using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, true))
			{
				WriteExcelFile(document);
			}

			stream.Flush();
			stream.Position = 0;
			return stream;
		}

		private void WriteExcelFile(SpreadsheetDocument spreadsheet)
		{
			//Bulk of code from http://www.codeproject.com/Articles/692121/Csharp-Export-data-to-Excel-using-OpenXML-librarie with modification
			//to work with our classes and structure

			//  Create the Excel file contents.  This function is used when creating an Excel file either writing 
			//  to a file, or writing to a MemoryStream.
			spreadsheet.AddWorkbookPart();
			spreadsheet.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

			//  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
			spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

			//  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
			WorkbookStylesPart workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
			Stylesheet stylesheet = new Stylesheet();
			workbookStylesPart.Stylesheet = stylesheet;

			//  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
			uint worksheetNumber = 1;

			//  For each worksheet you want to create
			string workSheetID = "rId" + worksheetNumber.ToString();
			string worksheetName = "Export";

			WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
			newWorksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

			// create sheet data
			newWorksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

			// save worksheet
			WriteDataTableToExcelWorksheet(newWorksheetPart);
			newWorksheetPart.Worksheet.Save();

			// create the worksheet to workbook relation
			if (worksheetNumber == 1)
				spreadsheet.WorkbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

			spreadsheet.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
			{
				Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
				SheetId = (uint)worksheetNumber,
				Name = worksheetName
			});



			spreadsheet.WorkbookPart.Workbook.Save();
		}

		private void WriteDataTableToExcelWorksheet(WorksheetPart worksheetPart)
		{
			var worksheet = worksheetPart.Worksheet;
			var sheetData = worksheet.GetFirstChild<SheetData>();

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

			//
			//  Create the Header row in our Excel Worksheet
			//
			uint rowIndex = 1;

			var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
			sheetData.Append(headerRow);

			var properties = typeof(T).GetProperties();
			int columnIndex = 0;
			foreach (var config in exportConfig.OrderBy(e => e.Order))
			{

				var propertyName = config.Field.Split('.');

				if (propertyName.Length == 1)
				{

					var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
					if (property != null)
					{
						AppendTextCell(excelColumnNames[columnIndex] + "1", GetPropertyDescription(property).Trim(), headerRow);
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
						AppendTextCell(excelColumnNames[columnIndex] + "1", GetPropertyDescription(subProperty).Trim(), headerRow);
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
							if (property != null)
							{
								AppendTextCell(excelColumnNames[columnIndex] + rowIndex.ToString(), this.GetFieldValue(item, property).Trim(), newExcelRow);								
							}
						}
						else
						{
							var childProp = properties.Where(p => p.Name.Equals(propertyName[0])).FirstOrDefault();
							var childProperties = childProp.PropertyType.GetProperties();
							var subProperty = childProperties.Where(p => p.Name.Equals(propertyName[1])).FirstOrDefault();
							if (subProperty != null)
							{
								AppendTextCell(excelColumnNames[columnIndex] + rowIndex.ToString(), this.GetFieldValue(childProp.GetValue(item), subProperty).Trim(), newExcelRow);
							}

						}
						
						columnIndex++;
					}
				}
			}
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

		private void AppendTextCell(string cellReference, string cellStringValue, Row excelRow)
		{
			//  Add a new Excel Cell to our Row 
			Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
			CellValue cellValue = new CellValue();
			cellValue.Text = cellStringValue;
			cell.Append(cellValue);
			excelRow.Append(cell);
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
