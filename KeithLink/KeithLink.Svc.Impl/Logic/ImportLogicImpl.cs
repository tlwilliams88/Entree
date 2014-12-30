using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
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

namespace KeithLink.Svc.Impl.Logic {
    public class ImportLogicImpl : IImportLogic {
        private IListServiceRepository listServiceRepository;
        private ICatalogLogic catalogLogic;
        private IEventLogRepository eventLogRepository;
        private IShoppingCartLogic shoppingCartLogic;

        private StringBuilder _errors;
        private StringBuilder _warnings;

        private const char CSV_DELIMITER = ',';
        private const char TAB_DELIMITER = '\t';
        private const string BINARY_EXCEL_EXTENSION = ".xls";

        private const int ITEM_NUMBER_INDEX = 0;
        private const int ITEM_QUANTITY_INDEX = 1;

        public ImportLogicImpl( IListServiceRepository listServiceRepository, ICatalogLogic catalogLogic, IEventLogRepository eventLogRepository, IShoppingCartLogic shoppingCartLogic ) {
            this.listServiceRepository = listServiceRepository;
            this.catalogLogic = catalogLogic;
            this.eventLogRepository = eventLogRepository;
            this.shoppingCartLogic = shoppingCartLogic;

            _errors = new StringBuilder();
            _warnings = new StringBuilder();
        }
        
        public ListImportModel ImportList(UserProfile user, UserSelectedContext catalogInfo, string csvFile)
		{
			try
			{
				var importReturn = new ListImportModel();
				var newList = new ListModel() { Name = string.Format("Imported List - {0}", DateTime.Now.ToShortDateString()), BranchId = catalogInfo.BranchId};

				var rows = csvFile.Split('\n');

				var items = rows.Skip(1).Select(i => i.Split(',')).Select(l => new ListItemModel() { ItemNumber = l[0].ToString() }).Where(x => !string.IsNullOrEmpty(x.ItemNumber)).ToList();

				//Verify the user has these products in their catalog
				var prods = catalogLogic.GetProductsByIds(catalogInfo.BranchId, items.Select(i => i.ItemNumber).Distinct().ToList());

				//Item counts don't match, which means there are items in the import that don't belong to this catalog
				if (prods.Products.Select(p => p.ItemNumber).Distinct().Count() != items.Select(i => i.ItemNumber).Distinct().Count())
				{
					importReturn.WarningMessage = "Some items were not imported because they were not found in the current catalog.";

					newList.Items = new List<ListItemModel>();
					foreach (var item in items)
						if (prods.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).Any())
							newList.Items.Add(item);
				}
				else
					newList.Items = items; //All items are in the user's catalog


				importReturn.ListId = 	listServiceRepository.CreateList(user.UserId, catalogInfo, newList, ListType.Custom);

				importReturn.Success = true;

				return importReturn;
			}
			catch (Exception ex)
			{
				eventLogRepository.WriteErrorLog(string.Format("List Import Error for Customer {0}", catalogInfo.CustomerId), ex);
				return new ListImportModel() { Success = false, ErrorMessage = "An error has occurred while processing the import file" };
			}
		}

        public OrderImportModel ImportOrder( UserProfile user, UserSelectedContext catalogInfo, OrderImportFileModel file ) {
            var returnModel = new OrderImportModel();

            var newCart = new ShoppingCart {
                Name = string.Format( "Imported Order - {0}", DateTime.Now.ToString( "g" ) ),
                BranchId = catalogInfo.BranchId
            };

            var items = new List<ShoppingCartItem>();

            try {
                switch (file.Options.FileFormat) {
                    case FileFormat.CSV:
                        items = ParseDelimitedFile( file, CSV_DELIMITER, user, catalogInfo );
                        break;
                    case FileFormat.Tab:
                        items = ParseDelimitedFile( file, TAB_DELIMITER, user, catalogInfo );
                        break;
                    case FileFormat.Excel:
                        items = ParseExcelDocument( file, user, catalogInfo );
                        break;
                }
            } catch (Exception e) {
                returnModel.Success = false;
                Error(String.Format(e.Message.ToString()));
                KeithLink.Common.Core.Email.ExceptionEmail.Send(e, String.Format("User: {0} for customer {1} in {2} failed importing an order from file: {3}.", user.UserId, catalogInfo.CustomerId, catalogInfo.BranchId, file.FileName));
            }

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

        private List<ShoppingCartItem> ParseDelimitedFile(OrderImportFileModel file, char Delimiter, UserProfile user, UserSelectedContext catalogInfo) {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>() {};

            var rows = file.Contents.Split( new string[] { Environment.NewLine }, StringSplitOptions.None );
            returnValue = rows
                        .Skip( file.Options.IgnoreFirstLine ? 1 : 0 )
                        .Where( line => !String.IsNullOrWhiteSpace(line) )
                        .Select( i => i.Split( Delimiter ) )
                        .Select( l => new ShoppingCartItem() {
                            ItemNumber = DetermineItemNumber( l[0], file.Options, user, catalogInfo ),
                            Quantity = DetermineQuantity( l[1], file.Options ),
                            Each = file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase) ? DetermineBrokenCaseItem( l[2], file.Options ):false
                            } )
                        .Where( x => !string.IsNullOrEmpty( x.ItemNumber ) ).ToList();

          return returnValue;
        }

        private List<ShoppingCartItem> ParseExcelDocument(OrderImportFileModel file, UserProfile user, UserSelectedContext catalogInfo) {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>() {};

            IExcelDataReader rdr = null;

            if (System.IO.Path.GetExtension( file.FileName ).Equals(BINARY_EXCEL_EXTENSION,StringComparison.InvariantCultureIgnoreCase)) {
                rdr = ExcelReaderFactory.CreateBinaryReader( file.Stream );
            } else {
                rdr = ExcelReaderFactory.CreateOpenXmlReader( file.Stream );
            }

            if ( file.Options.IgnoreFirstLine.Equals( true ) ) {
                rdr.Read(); // Skip the first line
            }

            while (rdr.Read()) {
                returnValue.Add(new ShoppingCartItem() {
                    ItemNumber = DetermineItemNumber(rdr.GetString(0), file.Options, user, catalogInfo),
                    Quantity = DetermineQuantity(rdr.GetString(1), file.Options),
                    Each = file.Options.Contents.Equals(FileContentType.ItemQtyBrokenCase) ? DetermineBrokenCaseItem( rdr.GetString(2), file.Options ):false
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

        private decimal DetermineQuantity( string quantities, OrderImportOptions options ) {
            decimal? returnValue = null;

            if (options.Contents.Equals(FileContentType.ItemOnly) ) {
                returnValue = 0;
            } else {
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

            ProductsReturn products = catalogLogic.GetProductsBySearch( catalogInfo, upc, new SearchInputModel() { From = 0, Size = 10 }, user);
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
