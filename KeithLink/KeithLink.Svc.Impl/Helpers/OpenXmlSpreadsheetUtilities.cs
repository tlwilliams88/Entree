using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeithLink.Svc.Impl.Helpers
{
    public class OpenXmlSpreadsheetUtilities
    {
        public const UInt32 DEFAULT_CELL = 0;
        public const UInt32 RIGHT_ALIGNED_CELL = 1;
        public const UInt32 TEXT_WRAP_CELL = 2;
        public const UInt32 RIGHT_ALIGNED_TEXT_WRAP_CELL = 3;
        public const UInt32 TEXT_WRAP_BOLD_CELL = 4;
        public const UInt32 RIGHT_ALIGNED_TEXT_WRAP_BOLD_CELL = 5;
        public const UInt32 BOLD_CELL = 6;
        public const UInt32 ITALIC_CELL = 7;
        public const UInt32 NUMBER_F2_CELL = 12;
        public const UInt32 SHORTDATE_CELL = 14;

        private ICustomerRepository _customerRepo;
        public OpenXmlSpreadsheetUtilities(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }
        public static MemoryStream MakeSpreadSheet(Worksheet workSheet, SheetData sheetData, string modelName)
        {
            MemoryStream stream = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, true))
            {
                document.AddWorkbookPart();
                document.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                document.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

                WorkbookStylesPart workbookStylesPart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
                Stylesheet stylesheet = OpenXmlSpreadsheetUtilities.GenerateStyleSheet();

                workbookStylesPart.Stylesheet = stylesheet;

                uint worksheetNumber = 1;

                string workSheetID = "rId" + worksheetNumber.ToString();
                string worksheetName = "Export";

                WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();

                newWorksheetPart.Worksheet = workSheet;
                newWorksheetPart.Worksheet.AppendChild(sheetData);

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

        public static Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(                                                               // Index 0 â€“ The default font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "00000000" } },
                        new FontName() { Val = "Arial" }),
                    new Font(                                                               // Index 1 â€“ The bold font.
                        new Bold(),
                        new FontSize() { Val = 12 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "00000000" } },
                        new FontName() { Val = "Arial" }),
                    new Font(                                                               // Index 2 â€“ The Italic font.
                        new Italic(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "00000000" } },
                        new FontName() { Val = "Arial" }),
                    new Font(                                                               // Index 2 â€“ The Times Roman font. with 16 size
                        new FontSize() { Val = 16 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "00000000" } },
                        new FontName() { Val = "Times New Roman" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 â€“ The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(                                                           // Index 1 â€“ The default fill of gray 125 (required)
                        new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill(                                                           // Index 2 â€“ The yellow fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFFFFF00" } }
                        )
                        { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(                                                         // Index 0 â€“ The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border(                                                         // Index 1 â€“ Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },                          // Index 0 The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Right }) { FontId = 0, FillId = 0, BorderId = 0 },       // Index 1 Right Aligned 
                    new CellFormat(new Alignment() { WrapText = true }) { FontId = 0, FillId = 0, BorderId = 0 },       // Index 2 Text Wrap
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Right, WrapText = true }) { FontId = 0, FillId = 0, BorderId = 0 },       // Index 3 Right,Text Wrap
                    new CellFormat(new Alignment() { WrapText = true }) { FontId = 1, FillId = 0, BorderId = 0 },       // Index 4 Text Wrap Bold
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Right, WrapText = true }) { FontId = 1, FillId = 0, BorderId = 0 },       // Index 5 Right,Text Wrap,Bold
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 6 Bold
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 7 Italic
                    new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 8 Times Roman
                    new CellFormat() { FontId = 0, FillId = 2, BorderId = 0, ApplyFill = true },       // Index 9 Yellow Fill
                    new CellFormat(                                                                   // Index 10 Alignment
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                    )
                    { FontId = 0, FillId = 0, BorderId = 0, ApplyAlignment = true },
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true },      // Index 11 Border
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0, NumberFormatId = 4, ApplyNumberFormat = true },      // Index 12 F2 Number
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0, NumberFormatId = 9, ApplyNumberFormat = true },      // Index 13 Percent
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0, NumberFormatId = 14, ApplyNumberFormat = true }      // Index 14 Short Date
                )
            ); // return
        }

        public static Worksheet SetColumnWidth(Worksheet worksheet, uint Index, DoubleValue dwidth)
        {
            DocumentFormat.OpenXml.Spreadsheet.Columns cs = worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Columns>();
            if (cs != null)
            {
                IEnumerable<DocumentFormat.OpenXml.Spreadsheet.Column> ic = cs.Elements<DocumentFormat.OpenXml.Spreadsheet.Column>().Where(r => r.Min == Index).Where(r => r.Max == Index);
                if (ic.Count() > 0)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Column c = ic.First();
                    c.Width = dwidth;
                }
                else
                {
                    DocumentFormat.OpenXml.Spreadsheet.Column c = new DocumentFormat.OpenXml.Spreadsheet.Column() { Min = Index, Max = Index, Width = dwidth, CustomWidth = true };
                    cs.Append(c);
                }
            }
            else
            {
                cs = new DocumentFormat.OpenXml.Spreadsheet.Columns();
                DocumentFormat.OpenXml.Spreadsheet.Column c = new DocumentFormat.OpenXml.Spreadsheet.Column() { Min = Index, Max = Index, Width = dwidth, CustomWidth = true };
                cs.Append(c);
                worksheet.InsertAfter(cs, worksheet.GetFirstChild<SheetFormatProperties>());
            }
            return worksheet;
        }

        public static uint AddTitleRow(uint rowIndex, string modelName, string[] excelColumnNames, string reportTitle, SheetData sheetData)
        {
            var titleRow = new Row { RowIndex = rowIndex };  // add a row at the to name the fields of spreadsheet
            OpenXmlSpreadsheetUtilities.AppendTextCell
                (excelColumnNames[0] + rowIndex.ToString(), reportTitle, titleRow, CellValues.String, OpenXmlSpreadsheetUtilities.BOLD_CELL);
            sheetData.Append(titleRow);
            rowIndex++;
            return rowIndex;
        }

        public static uint AddCustomerRow(uint rowIndex, string modelName, string[] excelColumnNames, Customer customer, SheetData sheetData)
        {
            var customerRow = new Row { RowIndex = rowIndex };  // add a row at the to name the fields of spreadsheet
            AppendTextCell
                (excelColumnNames[0] + rowIndex.ToString(), customer.CustomerBranch, customerRow, CellValues.String, OpenXmlSpreadsheetUtilities.ITALIC_CELL);
            AppendTextCell
                (excelColumnNames[1] + rowIndex.ToString(), customer.CustomerNumber, customerRow, CellValues.String, OpenXmlSpreadsheetUtilities.ITALIC_CELL);
            AppendTextCell
                (excelColumnNames[2] + rowIndex.ToString(), customer.CustomerName, customerRow, CellValues.String, OpenXmlSpreadsheetUtilities.ITALIC_CELL);
            sheetData.Append(customerRow);
            rowIndex++;
            return rowIndex;
        }

        public static void AppendTextCell(string cellReference, string cellStringValue, Row excelRow, CellValues dataType = CellValues.String, UInt32 styleIndex = OpenXmlSpreadsheetUtilities.DEFAULT_CELL)
        {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference, DataType = dataType, StyleIndex = styleIndex };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }
    }
}