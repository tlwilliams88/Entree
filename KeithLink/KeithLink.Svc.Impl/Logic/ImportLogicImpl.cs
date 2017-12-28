using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

using Excel;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Impl.Email;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.ImportFiles;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic {
    public class ImportLogicImpl : IImportLogic {
        #region ctor
        public ImportLogicImpl(ICatalogLogic catalogLogic, IEventLogRepository eventLogRepository, IShoppingCartLogic shoppingCartLogic, IPriceLogic priceLogic,
                               ICustomInventoryItemsRepository customInventoryRepo, ISiteCatalogService catalogService) {
            this.catalogLogic = catalogLogic;
            _catalogService = catalogService;
            this.eventLogRepository = eventLogRepository;
            this.shoppingCartLogic = shoppingCartLogic;
            this.priceLogic = priceLogic;
            _customInventoryRepo = customInventoryRepo;

            _errors = new StringBuilder();
            _warnings = new StringBuilder();
        }
        #endregion

        #region attributes
        private readonly ICatalogLogic catalogLogic;
        private readonly ISiteCatalogService _catalogService;
        private readonly ICustomInventoryItemsRepository _customInventoryRepo;
        private readonly IEventLogRepository eventLogRepository;
        private readonly IShoppingCartLogic shoppingCartLogic;
        private readonly IPriceLogic priceLogic;

        private StringBuilder _errors;
        private StringBuilder _warnings;

        private const char CSV_DELIMITER = ',';
        private const char TAB_DELIMITER = '\t';
        private const string BINARY_EXCEL_EXTENSION = ".xls";

        private const int ITEM_NUMBER_INDEX = 0;
        private const int ITEM_QUANTITY_INDEX = 1;

        private int _itemNumberColumn = 0;
        private int _quantityColumn = 1;
        private int _eachColumn = 2;
        #endregion

        #region methods
        public OrderImportModel ImportOrder(UserProfile user, UserSelectedContext catalogInfo, OrderImportFileModel file) {
            OrderImportModel returnModel = new OrderImportModel();

            ShoppingCart newCart = new ShoppingCart {
                Name = string.Format("Imported Order - {0}", DateTime.Now.ToString("g")),
                BranchId = catalogInfo.BranchId
            };

            ListModel parList = null;

            List<ShoppingCartItem> items = new List<ShoppingCartItem>();

            try {
                switch (file.Options.FileFormat) {
                    case FileFormat.CSV:
                        items = ParseDelimitedFile(file, CSV_DELIMITER, user, catalogInfo, parList);
                        break;
                    case FileFormat.Tab:
                        items = ParseDelimitedFile(file, TAB_DELIMITER, user, catalogInfo, parList);
                        break;
                    case FileFormat.Excel:
                        items = ParseExcelDocument(file, user, catalogInfo, parList);
                        break;
                }
            } catch (Exception e) {
                returnModel.Success = false;
                Error(string.Format(e.Message));
                ExceptionEmail.Send(e, string.Format("User: {0} for customer {1} in {2} failed importing an order from file: {3}.", user.UserId, catalogInfo.CustomerId, catalogInfo.BranchId, file.FileName));
            }

            CalculateCartSupTotal(catalogInfo, newCart, ref items);

            if (_errors.Length < 1) {
                if (file.Options.IgnoreZeroQuantities) {
                    items = items.Where(i => i.Quantity > 0)
                                 .ToList();
                }

                newCart.Items = items;

                returnModel.ListId = shoppingCartLogic.CreateCart(user, catalogInfo, newCart);
                returnModel.Success = true;
            }

            returnModel.ErrorMessage = _errors.ToString();

            returnModel.SuccessMessage = "Import Successful.";

            StringBuilder warningMsg = new StringBuilder();
            if (_warnings != null && _warnings.Length>0) {
                warningMsg.Append(_warnings);
            }
            returnModel.WarningMessage = warningMsg.ToString();

            return returnModel;
        }

        private void CalculateCartSupTotal(UserSelectedContext catalogInfo, ShoppingCart newCart, ref List<ShoppingCartItem> items) {
            ProductsReturn products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, items.Select(i => i.ItemNumber)
                                                                                               .ToList());
            items.ForEach(delegate(ShoppingCartItem item) {
                              Product product = products.Products.Where(p => p.ItemNumber == item.ItemNumber)
                                                        .FirstOrDefault();
                              if (product != null) {
                                  item.CatchWeight = product.CatchWeight;
                                  item.AverageWeight = product.AverageWeight;
                              }
                          });

            List<ShoppingCartItem> goodItems = new List<ShoppingCartItem>();

            PriceReturn prices = priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), items.Select(i => new Product {ItemNumber = i.ItemNumber})
                                                                                                                                  .ToList());
            foreach (ShoppingCartItem item in items) {
                Price price = prices.Prices.Where(p => p.ItemNumber == item.ItemNumber)
                                    .FirstOrDefault();
                if (price != null) {
                    newCart.SubTotal += (decimal) item.LineTotal(item.Each ? price.PackagePrice : price.CasePrice);
                    if ((decimal) item.LineTotal(item.Each ? price.PackagePrice : price.CasePrice) > 0) {
                        goodItems.Add(item);
                    } else {
                        _warnings = new StringBuilder();
                        _warnings.AppendLine("Some items failed to import.  Please check the items in your cart.");
                    }
                } else {
                    _warnings = new StringBuilder();
                    _warnings.AppendLine("Some items failed to import.  Please check the items in your cart.");
                }
            }
            items = goodItems;
        }

        private List<ShoppingCartItem> ParseDelimitedFile(OrderImportFileModel file, char Delimiter, UserProfile user, UserSelectedContext catalogInfo, ListModel parList) {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>();

            int itemNumberColumn = 0;
            int quantityColumn = 1;
            int eachColumn = 2;
            //See if we can determine which columns the item number and label exist
            if (file.Options.IgnoreFirstLine) {
                List<string> header = file.Contents.Split(new[] {
                                              Environment.NewLine,
                                              "\n"
                                          }, StringSplitOptions.None)
                                          .Take(1)
                                          .Select(i => i.Split(Delimiter)
                                                        .ToList())
                                          .FirstOrDefault();
                int colCount = 0;
                foreach (string col in header) {
                    if (col.Replace("\"", string.Empty)
                           .Equals("item", StringComparison.CurrentCultureIgnoreCase)) {
                        itemNumberColumn = colCount;
                    } else if (col.Replace("\"", string.Empty)
                                  .Equals("# Ordered", StringComparison.CurrentCultureIgnoreCase)) {
                        quantityColumn = colCount;
                    } else if (col.Replace("\"", string.Empty)
                                  .Equals("each", StringComparison.CurrentCultureIgnoreCase)) {
                        eachColumn = colCount;
                    }
                    colCount++;
                }
            }

            string[] rows = file.Contents.Split(new[] {
                Environment.NewLine,
                "\n"
            }, StringSplitOptions.None);
            //returnValue = rows
            //            .Skip( file.Options.IgnoreFirstLine ? 1 : 0 )
            //            .Where( line => !String.IsNullOrWhiteSpace(line) )
            //            .Select( i => i.Split( Delimiter ) )
            //            .Select( l => new ShoppingCartItem() {
            //                ItemNumber = DetermineItemNumber(l[itemNumberColumn].Replace("\"", string.Empty), file.Options, user, catalogInfo),
            //                Quantity = file.Options.Contents.Equals(FileContentType.ItemQty) ? 
            //                    DetermineQuantity(l[itemNumberColumn].Replace("\"", string.Empty), l[quantityColumn].Replace("\"", string.Empty), file.Options, parList) : 1,
            //                Each = file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase) ? DetermineBrokenCaseItem(l[eachColumn], file.Options) : false,
            //                CatalogId = catalogInfo.BranchId
            //                } )
            //            .Where( x => !string.IsNullOrEmpty( x.ItemNumber ) ).ToList();
            int rownum = 0;
            foreach (string row in rows) {
                if (row.Length > 0) {
                    if (++rownum == 1 &&
                        file.Options.IgnoreFirstLine) // skip the first row
                    {
                        continue;
                    }
                    string[] vals = row.Split(Delimiter);
                    string itmNum = DetermineItemNumber(vals[itemNumberColumn].PadLeft(6, '0'), file.Options, user, catalogInfo);
                    decimal qty = 1;
                    if (file.Options.Contents.Equals(FileContentType.ItemQty) |
                        file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase)) {
                        qty = DetermineQuantity(vals[itemNumberColumn].PadLeft(6, '0'), vals[quantityColumn], file.Options, parList);
                    }
                    bool each = false;
                    if (file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase)) {
                        each = DetermineBrokenCaseItem(vals[eachColumn], file.Options);
                    }
                    returnValue.Add(new ShoppingCartItem {
                        ItemNumber = itmNum,
                        CatalogId = catalogInfo.BranchId,
                        Quantity = qty,
                        Each = each
                    });
                }
            }

            return returnValue;
        }

        private List<ShoppingCartItem> ParseExcelDocument(OrderImportFileModel file, UserProfile user, UserSelectedContext catalogInfo, ListModel parList) {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>();

            IExcelDataReader rdr = null;

            if (Path.GetExtension(file.FileName)
                    .Equals(BINARY_EXCEL_EXTENSION, StringComparison.InvariantCultureIgnoreCase)) {
                rdr = ExcelReaderFactory.CreateBinaryReader(file.Stream);
            } else {
                rdr = ExcelReaderFactory.CreateOpenXmlReader(file.Stream);
            }

            rdr.Read();
            if (file.Options.IgnoreFirstLine.Equals(false)) {
                bool itemNumberHeaderFound = false;
                bool quantityHeaderFound = false;
                bool eachHeaderFound = false;

                if (rdr.FieldCount > 0) {
                    for (int i = 0; i < rdr.FieldCount - 1; i++) {
                        if (rdr.GetString(i)
                               .Equals("item", StringComparison.CurrentCultureIgnoreCase)) {
                            _itemNumberColumn = i;
                            itemNumberHeaderFound = true;
                        } else if (rdr.GetString(i)
                                      .Equals("# Ordered", StringComparison.CurrentCultureIgnoreCase)) {
                            _quantityColumn = i;
                            quantityHeaderFound = true;
                        } else if (rdr.GetString(i)
                                      .Equals("each", StringComparison.CurrentCultureIgnoreCase)) {
                            _eachColumn = i;
                            eachHeaderFound = true;
                        } 
                    }
                }

                if (itemNumberHeaderFound.Equals(false) &&
                    quantityHeaderFound.Equals(false) &&
                    eachHeaderFound.Equals(false)) {
                    ProcessRow(file, catalogInfo, user, parList, rdr, returnValue);
                }
            }

            try {
                while (rdr.Read()) {
                    ProcessRow(file, catalogInfo, user, parList, rdr, returnValue);
                }
            } catch (Exception ex) {
                eventLogRepository.WriteErrorLog("Bad parse of file", ex);
                _warnings = new StringBuilder();
                _warnings.Append("Some items failed to import.  Please check the items in your cart.");
            }

            if (returnValue.Count == 0) {
                throw new ApplicationException("Empty Order; No Products Defined in File");
            }

            return returnValue;
        }

        private void ProcessRow(OrderImportFileModel file, UserSelectedContext catalogInfo, UserProfile user, ListModel parList, IExcelDataReader rdr, List<ShoppingCartItem> returnValue)
        {
            string itmNum = DetermineItemNumber(rdr.GetString(_itemNumberColumn)
                                                   .PadLeft(6, '0'), file.Options, user, catalogInfo);
            decimal qty = 1;
            if (file.Options.Contents.Equals(FileContentType.ItemQty) |
                file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase))
            {
                qty = DetermineQuantity(rdr.GetString(_itemNumberColumn)
                                           .PadLeft(6, '0'), rdr.GetString(_quantityColumn), file.Options, parList);
            }
            bool each = false;
            if (file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase))
            {
                each = DetermineBrokenCaseItem(rdr.GetString(_eachColumn), file.Options);
            }
            returnValue.Add(new ShoppingCartItem {
                ItemNumber = itmNum,
                CatalogId = catalogInfo.BranchId,
                Quantity = qty,
                Each = each
            });
        }

        private string DetermineItemNumber(string itemNumber, OrderImportOptions options, UserProfile user, UserSelectedContext catalogInfo) {
            string returnValue = null;

            itemNumber = itemNumber.Replace("\"", "");

            if (itemNumber.ToInt()
                          .Equals(null) &&
                itemNumber.ToLong()
                          .Equals(null)) {
                Warning(string.Format("There were problems importing the file. Item: {0} is not a valid item or UPC.", itemNumber));
            } else {
                switch (options.ItemNumber) {
                    case ItemNumberType.ItemNumberOrUPC:
                        if (itemNumber.Length > 6) {
                            // It is a UPC - lookup the item number
                            returnValue = GetItemNumberFromUPC(itemNumber, options, user, catalogInfo);
                        } else {
                            returnValue = itemNumber.PadLeft(6, '0');
                        }
                        break;
                    case ItemNumberType.UPC:
                        returnValue = GetItemNumberFromUPC(itemNumber, options, user, catalogInfo);
                        break;
                    default: //ItemNumber
                        //Just return value
                        returnValue = itemNumber.PadLeft(6, '0');
                        ;
                        break;
                }
            }

            return returnValue;
        }

        private decimal DetermineQuantity(string itemNumber, string quantities, OrderImportOptions options, ListModel parList) {
            decimal? returnValue = null;

            quantities = quantities.Replace("\"", "");

            if (options.Contents.Equals(FileContentType.ItemOnly)) {
                returnValue = 0;
            } else {
                if (options.ImportByInventory &&
                    parList != null &&
                    parList.Items != null) {
                    decimal? onHandQuantity = quantities.ToDecimal();

                    ListItemModel parValue = parList.Items.Where(i => i.ItemNumber.Equals(itemNumber))
                                                    .FirstOrDefault();
                    if (parValue != null) {
                        decimal? orderQuantity = parValue.ParLevel - onHandQuantity;
                        returnValue = orderQuantity > 0 ? orderQuantity : 0;
                    } else {
                        returnValue = 0;
                    }
                } else {
                    returnValue = quantities.ToDecimal();
                }
            }

            if (returnValue.Equals(null)) {
                Warning("There was a problem during import. Quantity: {0} does not appear to be a valid number. Some quantities may not have imported properly.");
            }

            return returnValue.HasValue ? returnValue.Value : 0;
        }

        private bool DetermineBrokenCaseItem(string brokenCase, OrderImportOptions options) {
            bool returnValue = false;

            brokenCase = brokenCase.Replace("\"", "");

            if (brokenCase.Equals("y", StringComparison.InvariantCultureIgnoreCase)) {
                returnValue = true;
            }

            return returnValue;
        }

        private string GetItemNumberFromUPC(string upc, OrderImportOptions options, UserProfile user, UserSelectedContext catalogInfo) {
            string returnValue = string.Empty;

            upc = upc.Replace("\"", "");

            ProductsReturn products = _catalogService.GetProductsBySearch(catalogInfo, upc, new SearchInputModel {From = 0, Size = 10, SField = "upc"}, user);
            foreach (Product p in products.Products) {
                if (p.UPC == upc) {
                    returnValue = p.ItemNumber;
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

        public CustomInventoryImportModel ImportCustomInventory
                (UserProfile user, UserSelectedContext catalogInfo, CustomInventoryImportFileModel file) {
            try {
                CustomInventoryImportModel importReturn = new CustomInventoryImportModel();

                List<CustomInventoryItem> items = parseListDelimited(file, CSV_DELIMITER, user, catalogInfo);

                _customInventoryRepo.SaveRange(items);
                importReturn.Success = true;

                return importReturn;
            } catch (Exception ex) {
                eventLogRepository.WriteErrorLog
                        (string.Format("Custom Inventory Import Error for Customer {0}", catalogInfo.CustomerId), ex);
                SendErrorEmail(file, ex);

                return new CustomInventoryImportModel {
                    Success = false,
                    ErrorMessage = "An error has occurred while processing the import file"
                };
            }
        }

        private void SendErrorEmail(CustomInventoryImportFileModel file, Exception ex) {
            try {
                string errorMessage = string.Format
                        ("File Import error.\n\nImport Options:\nSelected Format: {0}\nSkip First Line: {1}\nFile Name:{2}",
                         file.FileFormat,
                         file.IgnoreFirstLine,
                         file.FileName);

                ContentType ct = null;
                Attachment attach = null;

                switch (file.FileFormat) {
                    case FileFormat.Excel:
                        file.Stream.Seek(0, SeekOrigin.Begin);
                        ct = new ContentType("application/msexcel");
                        attach = new Attachment(file.Stream, ct);
                        attach.ContentDisposition.FileName = file.FileName;
                        break;
                    default:
                        ct = new ContentType(MediaTypeNames.Text.Plain);
                        byte[] stringBytes = Encoding.UTF8.GetBytes(file.Contents);
                        MemoryStream memStream = new MemoryStream();
                        memStream.Write(stringBytes, 0, stringBytes.Length);
                        memStream.Seek(0, SeekOrigin.Begin);
                        attach = new Attachment(memStream, ct);
                        attach.ContentDisposition.FileName = file.FileName;
                        break;
                }

                ExceptionEmail.Send(ex, errorMessage, "File Import Error", attach);
            } catch (Exception emailEx) {
                eventLogRepository.WriteErrorLog("Error sending Import failure email", emailEx);
            }
        }

        private List<CustomInventoryItem> parseListDelimited
                (CustomInventoryImportFileModel file, char delimiter, UserProfile user, UserSelectedContext catalogInfo) {
            List<CustomInventoryItem> returnValue = new List<CustomInventoryItem>();

            string[] rows = file.Contents.Split(new[] {
                Environment.NewLine,
                "\n"
            }, StringSplitOptions.None);

            int itemNumberColumn = 0;
            int nameColumn = -1;
            int brandColumn = -1;
            int supplierColumn = -1;
            int packColumn = -1;
            int sizeColumn = -1;
            int eachColumn = -1;
            int casePriceColumn = -1;
            int packagePriceColumn = -1;
            int labelColumn = -1;

            List<string> header = rows.Take(1)
                                      .Select(i => i.Split(delimiter)
                                                    .ToList())
                                      .FirstOrDefault();
            if (header != null) {
                int colCount = 0;
                foreach (string col in header) {
                    string replaced = col.Replace("\"", string.Empty);
                    if (replaced.Equals("itemid", StringComparison.CurrentCultureIgnoreCase)) {
                        itemNumberColumn = colCount;
                    } else if (replaced.Equals("name", StringComparison.CurrentCultureIgnoreCase)) {
                        nameColumn = colCount;
                    } else if (replaced.Equals("brand", StringComparison.CurrentCultureIgnoreCase)) {
                        brandColumn = colCount;
                    } else if (replaced.Equals("supplier", StringComparison.CurrentCultureIgnoreCase)) {
                        supplierColumn = colCount;
                    } else if (replaced.Equals("pack", StringComparison.CurrentCultureIgnoreCase)) {
                        packColumn = colCount;
                    } else if (replaced.Equals("size", StringComparison.CurrentCultureIgnoreCase)) {
                        sizeColumn = colCount;
                    } else if (replaced.Equals("each(t or f)", StringComparison.CurrentCultureIgnoreCase)) {
                        eachColumn = colCount;
                    } else if (replaced.Equals("caseprice", StringComparison.CurrentCultureIgnoreCase)) {
                        casePriceColumn = colCount;
                    } else if (replaced.Equals("packageprice", StringComparison.CurrentCultureIgnoreCase)) {
                        packagePriceColumn = colCount;
                    } else if (replaced.Equals("label", StringComparison.CurrentCultureIgnoreCase)) {
                        labelColumn = colCount;
                    }
                    colCount++;
                }
            } else {
                throw new ApplicationException("Problem with header row. Template should be used.");
            }

            IEnumerable<string> data = rows.Skip(1);
            if (data == null) {
                throw new ApplicationException("Need to include custom inventory items to add.");
            }
            try {
                returnValue = data
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Select(i => i.Split(delimiter))
                        .Select(l => new CustomInventoryItem {
                            CustomerNumber = catalogInfo.CustomerId,
                            BranchId = catalogInfo.BranchId,
                            ItemNumber = l[itemNumberColumn].Replace("\"", string.Empty),
                            Name = l[nameColumn].Replace("\"", string.Empty),
                            Brand = l[brandColumn].Replace("\"", string.Empty),
                            Label = l[labelColumn].Replace("\"", string.Empty),
                            Supplier = l[supplierColumn].Replace("\"", string.Empty),
                            Pack = l[packColumn].Replace("\"", string.Empty),
                            Size = l[sizeColumn].Replace("\"", string.Empty),
                            Each = l[eachColumn].Replace("\"", string.Empty)
                                                .Equals
                                                ("t", StringComparison.CurrentCultureIgnoreCase) ? true : false,
                            CasePrice = l[casePriceColumn].Replace("\"", string.Empty)
                                                          .Length > 0 ?
                                            decimal.Parse(l[casePriceColumn].Replace("\"", string.Empty)) : 0,
                            PackagePrice = l[packagePriceColumn].Replace("\"", string.Empty)
                                                                .Length > 0 ?
                                               decimal.Parse(l[packagePriceColumn].Replace("\"", string.Empty)) : 0
                        })
                        .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .ToList();
            } catch (Exception ex) {
                throw new ApplicationException(ex.InnerException.Message);
            }

            return returnValue;
        }
        #endregion
    }
}