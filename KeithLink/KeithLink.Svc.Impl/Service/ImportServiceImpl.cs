using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Excel;

using KeithLink.Svc.Core.Interface.Import;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.ImportFiles;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Service {
    public class ImportServiceImpl : IImportService {
        #region ctor
        public ImportServiceImpl(ICatalogLogic catalogLogic) {
            this.catalogLogic = catalogLogic;

            _errors = new StringBuilder();
            _warnings = new StringBuilder();
        }
        #endregion

        public ListModel BuildList(UserProfile user, UserSelectedContext catalogInfo, ListImportFileModel file) {
            ListModel newList = new ListModel {Name = string.Format("Imported List - {0}", DateTime.Now.ToShortDateString()), BranchId = catalogInfo.BranchId};

            List<ListItemModel> items = new List<ListItemModel>();

            switch (file.FileFormat) {
                case FileFormat.CSV:
                    items = parseListDelimited(file, CSV_DELIMITER, user, catalogInfo);
                    break;
                case FileFormat.Tab:
                    items = parseListDelimited(file, TAB_DELIMITER, user, catalogInfo);
                    break;
                case FileFormat.Excel:
                    items = parseListExcel(file, user, catalogInfo);
                    break;
            }

            ProductsReturn validProducts = catalogLogic.GetProductsByIds(catalogInfo.BranchId, items.Select(i => i.ItemNumber)
                                                                                                    .Distinct()
                                                                                                    .ToList());

            List<ListItemModel> mergedItems = (from x in items
                                               join y in validProducts.Products on x.ItemNumber.Trim() equals y.ItemNumber.Trim()
                                               select x).ToList();

            //if (items.Select( p => p.ItemNumber ).Distinct().Count() != validProducts.Products.Select( o => o.ItemNumber ).Distinct().Count()) {
            if (items.Distinct()
                     .Count() != mergedItems.Distinct()
                                            .Count()) {
                Warning("Some items were not imported because they were not found in the current catalog.");
                newList.Items = mergedItems;
                //foreach (var item in items)
                //    if (validProducts.Products.Where( p => p.ItemNumber.Equals( item.ItemNumber ) ).Any())
                //        newList.Items.Add( item );
            } else {
                newList.Items = items;
            }

            return newList;
        }

        private List<ListItemModel> parseListDelimited(ListImportFileModel file, char delimiter, UserProfile user, UserSelectedContext catalogInfo) {
            List<ListItemModel> returnValue = new List<ListItemModel>();

            int itemNumberColumn = 0;
            int labelColumn = -1;
            //See if we can determine which columns the item number and label exist
            if (file.IgnoreFirstLine) {
                List<string> header = file.Contents.Split(new[] {
                                              Environment.NewLine,
                                              "\n"
                                          }, StringSplitOptions.None)
                                          .Take(1)
                                          .Select(i => i.Split(delimiter)
                                                        .ToList())
                                          .FirstOrDefault();
                int colCount = 0;
                foreach (string col in header) {
                    if (col.Replace("\"", string.Empty)
                           .Equals("item", StringComparison.CurrentCultureIgnoreCase)) {
                        itemNumberColumn = colCount;
                    } else if (col.Replace("\"", string.Empty)
                                  .Equals("label", StringComparison.CurrentCultureIgnoreCase)) {
                        labelColumn = colCount;
                    }
                    colCount++;
                }
            }

            IEnumerable<string> rows = file.Contents.Split(new[] {
                                               Environment.NewLine,
                                               "\n"
                                           }, StringSplitOptions.None)
                                           .Skip(file.IgnoreFirstLine ? 1 : 0);
            returnValue = rows
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(i => i.Split(delimiter))
                    .Select(l => new ListItemModel {
                        ItemNumber = l[itemNumberColumn].Replace("\"", string.Empty),
                        Label = labelColumn == -1 ? string.Empty : l[labelColumn].Replace("\"", string.Empty),
                        CatalogId = catalogInfo.BranchId
                    })
                    .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                    .ToList();

            int ind = 0;
            foreach (var item in returnValue) {
                item.Position = ++ind;
            }

            return returnValue;
        }

        private List<ListItemModel> parseListExcel(ListImportFileModel file, UserProfile user, UserSelectedContext catalogInfo) {
            List<ListItemModel> returnValue = new List<ListItemModel>();

            IExcelDataReader rdr = null;

            if (Path.GetExtension(file.FileName)
                    .Equals(BINARY_EXCEL_EXTENSION, StringComparison.InvariantCultureIgnoreCase)) {
                rdr = ExcelReaderFactory.CreateBinaryReader(file.Stream);
            } else {
                rdr = ExcelReaderFactory.CreateOpenXmlReader(file.Stream);
            }
            int itemNumberColumn = 0;
            int labelColumn = -1;

            if (file.IgnoreFirstLine) {
                rdr.Read(); // Skip the first line
                for (int i = 0; i < rdr.FieldCount - 1; i++) {
                    if (rdr.GetString(i)
                           .Equals("item", StringComparison.CurrentCultureIgnoreCase)) {
                        itemNumberColumn = i;
                    } else if (rdr.GetString(i)
                                  .Equals("label", StringComparison.CurrentCultureIgnoreCase)) {
                        labelColumn = i;
                    }
                }
            }

            int ind = 0;
            while (rdr.Read()) {
                if (rdr.GetString(itemNumberColumn) != null) {
                    returnValue.Add(new ListItemModel {
                        ItemNumber = rdr.GetString(itemNumberColumn)
                                        .PadLeft(6, '0'),
                        Label = labelColumn == -1 ? string.Empty : rdr.GetString(labelColumn),
                        CatalogId = catalogInfo.BranchId,
                        Position = ++ind
                    });
                }
            }

            return returnValue;
        }

        private void Error(string error) {
            _errors.AppendLine(error);
        }

        private void Warning(string warning) {
            _warnings.AppendLine(warning);
        }

        #region attributes
        private readonly ICatalogLogic catalogLogic;

        private readonly StringBuilder _errors;
        private readonly StringBuilder _warnings;

        public string Errors {
            get { return _errors.ToString(); }
        }

        public string Warnings {
            get { return _warnings.ToString(); }
        }

        private const char CSV_DELIMITER = ',';
        private const char TAB_DELIMITER = '\t';
        private const string BINARY_EXCEL_EXTENSION = ".xls";
        #endregion
    }
}