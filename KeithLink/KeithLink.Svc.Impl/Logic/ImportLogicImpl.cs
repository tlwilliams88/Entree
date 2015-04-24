using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.ImportFiles;
using Excel;
using DocumentFormat;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Enumerations.List;
using System.IO;

namespace KeithLink.Svc.Impl.Logic {
    public class ImportLogicImpl : IImportLogic {
        private IListServiceRepository listServiceRepository;
        private ICatalogLogic catalogLogic;
        private IEventLogRepository eventLogRepository;
        private IShoppingCartLogic shoppingCartLogic;
		private IPriceLogic priceLogic;

        private StringBuilder _errors;
        private StringBuilder _warnings;

        private const char CSV_DELIMITER = ',';
        private const char TAB_DELIMITER = '\t';
        private const string BINARY_EXCEL_EXTENSION = ".xls";

        private const int ITEM_NUMBER_INDEX = 0;
        private const int ITEM_QUANTITY_INDEX = 1;

        public ImportLogicImpl( IListServiceRepository listServiceRepository, ICatalogLogic catalogLogic, IEventLogRepository eventLogRepository, IShoppingCartLogic shoppingCartLogic, IPriceLogic priceLogic ) {
            this.listServiceRepository = listServiceRepository;
            this.catalogLogic = catalogLogic;
            this.eventLogRepository = eventLogRepository;
            this.shoppingCartLogic = shoppingCartLogic;
			this.priceLogic = priceLogic;

            _errors = new StringBuilder();
            _warnings = new StringBuilder();
        }
        
        public ListImportModel ImportList(UserProfile user, UserSelectedContext catalogInfo, ListImportFileModel file)
		{
			try
			{
				var importReturn = new ListImportModel();

                var newList = new ListModel() { Name = string.Format( "Imported List - {0}", DateTime.Now.ToShortDateString() ), BranchId = catalogInfo.BranchId };

                List<ListItemModel> items = new List<ListItemModel>();

                switch (file.FileFormat) {
                    case FileFormat.CSV:
						items = parseListDelimited( file, CSV_DELIMITER, user, catalogInfo );
                        break;
                    case FileFormat.Tab:
                        items = parseListDelimited( file, TAB_DELIMITER, user, catalogInfo );
                        break;
                    case FileFormat.Excel:
						items = parseListExcel( file, user, catalogInfo );
                        break;
                }

                var validProducts = catalogLogic.GetProductsByIds( catalogInfo.BranchId, items.Select(i => i.ItemNumber).Distinct().ToList() );

                List<ListItemModel> mergedItems = (from x in items
                                            join y in validProducts.Products on x.ItemNumber.Trim() equals y.ItemNumber.Trim()
                                            select x).ToList();

                //if (items.Select( p => p.ItemNumber ).Distinct().Count() != validProducts.Products.Select( o => o.ItemNumber ).Distinct().Count()) {
                if (items.Distinct().Count() != mergedItems.Distinct().Count()) {
                    Warning( "Some items were not imported because they were not found in the current catalog." );
                    newList.Items = mergedItems;
                    //foreach (var item in items)
                    //    if (validProducts.Products.Where( p => p.ItemNumber.Equals( item.ItemNumber ) ).Any())
                    //        newList.Items.Add( item );
                } else {
                    newList.Items = items;
                }

                importReturn.Success = true;
                importReturn.ListId = listServiceRepository.CreateList( user.UserId, catalogInfo, newList, ListType.Custom );
                importReturn.WarningMessage = _warnings.ToString();
                importReturn.ErrorMessage = _errors.ToString();

				return importReturn;
			}
			catch (Exception ex)
			{
				eventLogRepository.WriteErrorLog(string.Format("List Import Error for Customer {0}", catalogInfo.CustomerId), ex);
				SendErrorEmail(file, ex);


				return new ListImportModel() { Success = false, ErrorMessage = "An error has occurred while processing the import file" };
			}
		}

		private void SendErrorEmail(ListImportFileModel file, Exception ex)
		{
			try
			{
				var errorMessage = string.Format("File Import error.\n\nImport Options:\nSelected Format: {0}\nSkip First Line: {1}\nFile Name:{2}", file.FileFormat, file.IgnoreFirstLine, file.FileName);

				System.Net.Mime.ContentType ct = null;
				System.Net.Mail.Attachment attach = null;

				switch (file.FileFormat)
				{
					case FileFormat.Excel:
						file.Stream.Seek(0, System.IO.SeekOrigin.Begin);
						ct = new System.Net.Mime.ContentType("application/msexcel");
						attach = new System.Net.Mail.Attachment(file.Stream, ct);
						attach.ContentDisposition.FileName = file.FileName;
						break;
					default:
						ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Plain);
						var stringBytes = System.Text.Encoding.UTF8.GetBytes(file.Contents);
						var memStream = new MemoryStream();
						memStream.Write(stringBytes, 0, stringBytes.Length);
						memStream.Seek(0, SeekOrigin.Begin);
						attach = new System.Net.Mail.Attachment(memStream, ct);
						attach.ContentDisposition.FileName = file.FileName;
						break;
				}

				KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, errorMessage, "File Import Error", attach);
			}
			catch (Exception emailEx)
			{
				eventLogRepository.WriteErrorLog("Error sending Import failure email", emailEx);
			}
		}

        private List<ListItemModel> parseListDelimited( ListImportFileModel file, char delimiter, UserProfile user, UserSelectedContext catalogInfo ) {
            List<ListItemModel> returnValue = new List<ListItemModel>();

			var itemNumberColumn = 0;
			var labelColumn = -1;
			//See if we can determine which columns the item number and label exist
			if (file.IgnoreFirstLine)
			{
				var header = file.Contents.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None).Take(1).Select(i => i.Split(delimiter).ToList()).FirstOrDefault();
				int colCount = 0;
				foreach (var col in header)
				{
					if (col.Replace("\"", string.Empty).Equals("item", StringComparison.CurrentCultureIgnoreCase))
						itemNumberColumn = colCount;
					else if (col.Replace("\"", string.Empty).Equals("label", StringComparison.CurrentCultureIgnoreCase))
						labelColumn = colCount;
					colCount++;
				}
			}


            var rows = file.Contents.Split( new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None ).Skip( file.IgnoreFirstLine ? 1 : 0 );
            returnValue = rows
                        .Where( line => !String.IsNullOrWhiteSpace( line ) )
                        .Select( i => i.Split( delimiter ) )
                        .Select( l => new ListItemModel() {
							ItemNumber = l[itemNumberColumn].Replace("\"", string.Empty),
							Label = labelColumn == -1 ? string.Empty : l[labelColumn].Replace("\"", string.Empty)
                        } )
                        .Where( x => !String.IsNullOrEmpty( x.ItemNumber ) ).ToList();

            return returnValue;
        }

        private List<ListItemModel> parseListExcel( ListImportFileModel file, UserProfile user, UserSelectedContext catalogInfo ) {
            List<ListItemModel> returnValue = new List<ListItemModel>() { };

            IExcelDataReader rdr = null;

            if (System.IO.Path.GetExtension( file.FileName ).Equals( BINARY_EXCEL_EXTENSION, StringComparison.InvariantCultureIgnoreCase )) {
                rdr = ExcelReaderFactory.CreateBinaryReader( file.Stream );
            } else {
                rdr = ExcelReaderFactory.CreateOpenXmlReader( file.Stream );
            }
			var itemNumberColumn = 0;
			var labelColumn = -1;

            if (file.IgnoreFirstLine) {
                rdr.Read(); // Skip the first line
				for (int i = 0; i < rdr.FieldCount-1; i++)
				{
					if (rdr.GetString(i).Equals("item", StringComparison.CurrentCultureIgnoreCase))
						itemNumberColumn = i;
					else if (rdr.GetString(i).Equals("label", StringComparison.CurrentCultureIgnoreCase))
						labelColumn = i;

				}
            }

			

            while (rdr.Read()) {
                returnValue.Add( new ListItemModel() {
                    ItemNumber = rdr.GetString( itemNumberColumn ).PadLeft(6, '0'),
					Label = labelColumn == -1 ? string.Empty : rdr.GetString(labelColumn)
                } );
            }

            return returnValue;
        }

        public OrderImportModel ImportOrder( UserProfile user, UserSelectedContext catalogInfo, OrderImportFileModel file ) {
            var returnModel = new OrderImportModel();

            var newCart = new ShoppingCart {
                Name = string.Format( "Imported Order - {0}", DateTime.Now.ToString( "g" ) ),
                BranchId = catalogInfo.BranchId
            };

			ListModel parList = null;

			if (file.Options.ImportByInventory && file.Options.ListId.HasValue)
			{
				parList = listServiceRepository.ReadList(user, catalogInfo, file.Options.ListId.Value);
			}


            var items = new List<ShoppingCartItem>();

            try {
                switch (file.Options.FileFormat) {
                    case FileFormat.CSV:
                        items = ParseDelimitedFile( file, CSV_DELIMITER, user, catalogInfo, parList );
                        break;
                    case FileFormat.Tab:
						items = ParseDelimitedFile(file, TAB_DELIMITER, user, catalogInfo, parList);
                        break;
                    case FileFormat.Excel:
                        items = ParseExcelDocument( file, user, catalogInfo, parList);
                        break;
                }
            } catch (Exception e) {
                returnModel.Success = false;
                Error(String.Format(e.Message.ToString()));
                KeithLink.Common.Core.Email.ExceptionEmail.Send(e, String.Format("User: {0} for customer {1} in {2} failed importing an order from file: {3}.", user.UserId, catalogInfo.CustomerId, catalogInfo.BranchId, file.FileName));
            }

			CalculateCartSupTotal(catalogInfo, newCart, items);

            if (_errors.Length < 1) {
                if (file.Options.IgnoreZeroQuantities)
                    items = items.Where( i => i.Quantity > 0 ).ToList();

                newCart.Items = items;

                returnModel.ListId = shoppingCartLogic.CreateCart( user, catalogInfo, newCart );
                returnModel.Success = true;
            }

            returnModel.ErrorMessage = _errors.ToString();
            returnModel.WarningMessage = _warnings.ToString();
            return returnModel;
        }

		private void CalculateCartSupTotal(UserSelectedContext catalogInfo, ShoppingCart newCart, List<ShoppingCartItem> items)
		{
			var products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, items.Select(i => i.ItemNumber).ToList());
			items.ForEach(delegate(ShoppingCartItem item)
			{
				var product = products.Products.Where(p => p.ItemNumber == item.ItemNumber).FirstOrDefault();
				if (product != null)
				{
					item.CatchWeight = product.CatchWeight;
					item.AverageWeight = product.AverageWeight;
				}
			});

			var prices = priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), items.Select(i => new Product() { ItemNumber = i.ItemNumber }).ToList());
			foreach (var item in items)
			{
				var price = prices.Prices.Where(p => p.ItemNumber == item.ItemNumber).FirstOrDefault();
				if (price != null)
					newCart.SubTotal += (decimal)item.LineTotal(item.Each ? price.PackagePrice : price.CasePrice);
			}
		}

        private List<ShoppingCartItem> ParseDelimitedFile(OrderImportFileModel file, char Delimiter, UserProfile user, UserSelectedContext catalogInfo, ListModel parList) {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>() {};

			var itemNumberColumn = 0;
			var quantityColumn = 1;
			var eachColumn = 2;
			//See if we can determine which columns the item number and label exist
			if (file.Options.IgnoreFirstLine)
			{
				var header = file.Contents.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None).Take(1).Select(i => i.Split(Delimiter).ToList()).FirstOrDefault();
				int colCount = 0;
				foreach (var col in header)
				{
					if (col.Replace("\"", string.Empty).Equals("item", StringComparison.CurrentCultureIgnoreCase))
						itemNumberColumn = colCount;
					else if (col.Replace("\"", string.Empty).Equals("# Ordered", StringComparison.CurrentCultureIgnoreCase))
						quantityColumn = colCount;
					else if (col.Replace("\"", string.Empty).Equals("each", StringComparison.CurrentCultureIgnoreCase))
						eachColumn = colCount;
					colCount++;
				}
			}


            var rows = file.Contents.Split( new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None );
            returnValue = rows
                        .Skip( file.Options.IgnoreFirstLine ? 1 : 0 )
                        .Where( line => !String.IsNullOrWhiteSpace(line) )
                        .Select( i => i.Split( Delimiter ) )
                        .Select( l => new ShoppingCartItem() {
							ItemNumber = DetermineItemNumber(l[itemNumberColumn].Replace("\"", string.Empty), file.Options, user, catalogInfo),
							Quantity = DetermineQuantity(l[itemNumberColumn].Replace("\"", string.Empty), l[quantityColumn].Replace("\"", string.Empty), file.Options, parList),
							Each = file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase) ? DetermineBrokenCaseItem(l[eachColumn], file.Options) : false
                            } )
                        .Where( x => !string.IsNullOrEmpty( x.ItemNumber ) ).ToList();

          return returnValue;
        }

        private List<ShoppingCartItem> ParseExcelDocument(OrderImportFileModel file, UserProfile user, UserSelectedContext catalogInfo, ListModel parList) {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>() {};

            IExcelDataReader rdr = null;

            if (System.IO.Path.GetExtension( file.FileName ).Equals(BINARY_EXCEL_EXTENSION,StringComparison.InvariantCultureIgnoreCase)) {
                rdr = ExcelReaderFactory.CreateBinaryReader( file.Stream );
            } else {
                rdr = ExcelReaderFactory.CreateOpenXmlReader( file.Stream );
            }

			var itemNumberColumn = 0;
			var quantityColumn = 1;
			var eachColumn = 2;

            if ( file.Options.IgnoreFirstLine.Equals( true ) ) {
                rdr.Read(); // Skip the first line
				for (int i = 0; i < rdr.FieldCount - 1; i++)
				{
					if (rdr.GetString(i).Equals("item", StringComparison.CurrentCultureIgnoreCase))
						itemNumberColumn = i;
					else if (rdr.GetString(i).Equals("# Ordered", StringComparison.CurrentCultureIgnoreCase))
						quantityColumn = i;
					else if (rdr.GetString(i).Equals("each", StringComparison.CurrentCultureIgnoreCase))
						quantityColumn = i;

				}
            }

            while (rdr.Read()) {
                returnValue.Add(new ShoppingCartItem() {
                    ItemNumber = DetermineItemNumber(rdr.GetString(itemNumberColumn).PadLeft(6, '0'), file.Options, user, catalogInfo),
                    Quantity = DetermineQuantity(rdr.GetString(itemNumberColumn).PadLeft(6, '0'), rdr.GetString(quantityColumn), file.Options, parList),
                    Each = file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase) ? DetermineBrokenCaseItem( rdr.GetString(eachColumn), file.Options ):false
                });
            }

            return returnValue;
        }

        private string DetermineItemNumber( string itemNumber, OrderImportOptions options, UserProfile user, UserSelectedContext catalogInfo ) {
            string returnValue = null;

            if (itemNumber.ToInt().Equals( null ) && itemNumber.ToLong().Equals( null )) {
                Warning( String.Format("There were problems importing the file. Item: {0} is not a valid item or UPC.", itemNumber ));
            } else {
                switch (options.ItemNumber) {
                    case ItemNumberType.ItemNumberOrUPC:
                        if (itemNumber.Length > 6) { // It is a UPC - lookup the item number
                            returnValue = GetItemNumberFromUPC( itemNumber, options, user, catalogInfo );
                        } else {
                            returnValue = itemNumber;
                        }
                        break;
                    case ItemNumberType.UPC:
                        returnValue = GetItemNumberFromUPC(itemNumber, options, user, catalogInfo);
                        break;
                    default: //ItemNumber
                        //Just return value
                        returnValue = itemNumber;
                        break;
                }
            }

            return returnValue;
        }

        private decimal DetermineQuantity(string itemNumber, string quantities, OrderImportOptions options, ListModel parList ) {
            decimal? returnValue = null;

            if (options.Contents.Equals(FileContentType.ItemOnly) ) {
                returnValue = 0;
            } else {
				if (options.ImportByInventory && parList != null && parList.Items != null)
				{
					var onHandQuantity = quantities.ToDecimal();

					var parValue = parList.Items.Where(i => i.ItemNumber.Equals(itemNumber)).FirstOrDefault();
					if (parValue != null)
					{
						var orderQuantity = parValue.ParLevel - onHandQuantity;
						returnValue = orderQuantity > 0 ? orderQuantity : 0;
					}
					else
						returnValue = 0;

				}
				else
					returnValue = quantities.ToDecimal();
            }

            if (returnValue.Equals( null )) {
                Warning( "There was a problem during import. Quantity: {0} does not appear to be a valid number. Some quantities may not have imported properly." );
            }

            return returnValue.HasValue ? returnValue.Value : 0;
        }

        private bool DetermineBrokenCaseItem( string brokenCase, OrderImportOptions options ) {
            bool returnValue = false;

            if (brokenCase.Equals( "y", StringComparison.InvariantCultureIgnoreCase )) {
                returnValue = true;
            }

            return returnValue;
        }
        
        private string GetItemNumberFromUPC( string upc, OrderImportOptions options, UserProfile user, UserSelectedContext catalogInfo ) {
            string returnValue = String.Empty;

            ProductsReturn products = catalogLogic.GetProductsBySearch( catalogInfo, upc, new SearchInputModel() { From = 0, Size = 10, SField= "upc"  }, user);
            foreach (Product p in products.Products) {
                if (p.UPC == upc) {
                    returnValue = p.ItemNumber;
                }
            }

            return returnValue;
        }

        private void Error( string error ) {
            _errors.AppendLine( error );
        }

        private void Warning( string warning ) {
            _warnings.AppendLine( warning );
        }
    }
}
