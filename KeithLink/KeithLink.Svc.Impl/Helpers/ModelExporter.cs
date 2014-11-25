using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
	public class ModelExporter<TModel> where TModel : class, IExportableModel
	{
		private IList<TModel> Model { get; set; }

		//private List<ModelExportEnumMap> _enumMaps = null;
		//public IReadOnlyList<ModelExportEnumMap> EnumMaps { get { return this._enumMaps.AsReadOnly(); } }
		private List<ExportModelConfiguration> exportConfig = null;
						
		public ModelExporter(IList<TModel> model, List<ExportModelConfiguration> exportConfig)
		{
			this.Model = model;
			this.exportConfig = exportConfig;
		}

		public ModelExporter(IList<TModel> model)
		{
			this.Model = model;
			this.exportConfig = model.First().DefaultExportConfiguration();
		}

		//public void AddEnumMap<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, Type enumerationType)
		//{
		//	var property = this.GetMemberInfo(propertyExpression);
		//	if (property is PropertyInfo)
		//		this._enumMaps.Add(new ModelExportEnumMap(property.Name, enumerationType));
		//	else
		//		throw new ArgumentException("Expression is not a property access", "propertyExpression");
		//}

		private MemberInfo GetMemberInfo<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
		{
			var member = propertyExpression.Body as MemberExpression;
			if (member != null)
				return member.Member;

			throw new ArgumentException("Expression is not a member access", "expression");
		}

		public MemoryStream Export(string exportType)
		{
			StringBuilder sb = new StringBuilder();

			if(exportType.Equals("excel", StringComparison.CurrentCultureIgnoreCase))
				return this.GenerateExcelExport();

			if(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) || exportType.Equals("tab", StringComparison.CurrentCultureIgnoreCase))
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
			string worksheetName ="Export";

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

		private void WriteDataTableToExcelWorksheet( WorksheetPart worksheetPart)
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

			var properties = typeof(TModel).GetProperties();
			int columnIndex = 0;
			foreach (var config in exportConfig.OrderBy(e => e.Order))
			{
				var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
				if (property != null)
				{
					var description = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
					string value = string.Empty;
					if (description != null)
						value = description.Description;
					else
						value = property.Name;

					AppendTextCell(excelColumnNames[columnIndex] + "1", value.Trim(), headerRow);
					columnIndex++;
				}
				
			}
			
			//
			//  Now, step through each row of data in our DataTable...
					
			foreach (var item in this.Model)
			{
				rowIndex++;
				var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
				
				if (item != null)
				{
					columnIndex = 0;
					foreach (var config in exportConfig.OrderBy(e => e.Order))
					{
						var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
						if (property != null)
						{
							AppendTextCell(excelColumnNames[columnIndex] + rowIndex.ToString(), this.GetFieldValue(item, property).Trim(), newExcelRow);
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

		private void WriteItemRecord(StringBuilder sb, TModel item, string exportType)
		{
			List<string> itemRecord = new List<string>();

			var properties = item.GetType().GetProperties();

			foreach (var config in exportConfig.OrderBy(e => e.Order))
			{
				var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
				if (property != null)
				{
					itemRecord.Add(string.Format("\"{0}\"", this.GetFieldValue(item, property).Trim()));
				}
			}
			
			if (itemRecord.Count > 0)
				sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase ) ? "," : "\t", itemRecord));
		}

		private string GetFieldValue(object item, PropertyInfo property)
		{
			var value = property.GetValue(item);
			if (value == null)
				return string.Empty;

			if (value.GetType().IsEnum)
				return this.GetAttributeFieldValue(value.GetType(), value.ToString());

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

			var properties = typeof(TModel).GetProperties();

			foreach (var config in exportConfig.OrderBy(e => e.Order))
			{
				var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
				if (property != null)
				{
					var description = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
					string value = string.Empty;
					if (description != null)
						value = description.Description;
					else
						value = property.Name;

					headerRecord.Add(string.Format("\"{0}\"", value.Trim()));
				}
			}

			sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) ? "," : "\t", headerRecord));
		}

	}
}
