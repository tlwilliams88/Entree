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
				var prods = catalogLogic.GetProductsByIds(catalogInfo.BranchId, items.Select(i => i.ItemNumber).Distinct().ToList(), user);

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
                returnModel.ErrorMessage = e.Message.ToString();
                KeithLink.Common.Core.Email.ExceptionEmail.Send(e, String.Format("User: {0} for customer {1} in {2} failed importing an order from file: {3}.", user.UserId, catalogInfo.CustomerId, catalogInfo.BranchId, file.FileName));
            }

            if (returnModel.ErrorMessage == null) {
                if (file.Options.IgnoreZeroQuantities)
                    items = items.Where( i => i.Quantity > 0 ).ToList();

                newCart.Items = items;

                returnModel.ListId = shoppingCartLogic.CreateCart( user, catalogInfo, newCart );
                returnModel.Success = true;
            }

            return returnModel;
        }

        private List<ShoppingCartItem> ParseDelimitedFile(OrderImportFileModel file, char Delimiter, UserProfile user, UserSelectedContext catalogInfo) {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>() {};

            var rows = file.Contents.Split( new string[] { Environment.NewLine }, StringSplitOptions.None );
            returnValue = rows
                        .Skip( file.Options.IgnoreFirstLine ? 1 : 0 )
                        .Select( i => i.Split( Delimiter ) )
                        // Not sure why this is being checked. .Where( f => f[0] == "y" )
                        .Select( l => new ShoppingCartItem() {
                            ItemNumber = DetermineItemNumber( l, file.Options, user, catalogInfo ),
                            Quantity = DetermineQuantity( l, file.Options )
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

            rdr.IsFirstRowAsColumnNames = file.Options.IgnoreFirstLine;

            while (rdr.Read()) {
                returnValue.Add(new ShoppingCartItem() {
                    ItemNumber = DetermineItemNumber(new string[] {rdr.GetString(0) }, file.Options, user, catalogInfo),
                    Quantity = DetermineQuantity(new string[] { rdr.GetString(1) }, file.Options) 
                });
            }

            return returnValue;
        }

        private string DetermineItemNumber( string[] itemAndQuantities, OrderImportOptions options, UserProfile user, UserSelectedContext catalogInfo ) {
            switch (options.ItemNumber) {
                case ItemNumberType.ItemNumberOrUPC:
                    if (itemAndQuantities[0].Length > 6) { // It is a UPC - lookup the item number
                        return GetItemNumberFromUPC( itemAndQuantities[0], options, user, catalogInfo ); 
                    }
                    return itemAndQuantities[0];
                case ItemNumberType.UPC:
                    return GetItemNumberFromUPC(itemAndQuantities[0], options, user, catalogInfo);
                default: //ItemNumber
                    //Just return value
                    return itemAndQuantities[0];
            }
        }

        private decimal DetermineQuantity( string[] quantities, OrderImportOptions options ) {
            if (options.Contents == FileContentType.ItemOnly)
                return 0;

            var returnDecimal = quantities[0].ToDecimal();
            return returnDecimal.HasValue ? returnDecimal.Value : 0;
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
    }
}
