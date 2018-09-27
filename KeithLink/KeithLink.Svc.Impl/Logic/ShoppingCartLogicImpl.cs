﻿// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

using CommerceServer.Foundation;
using Microsoft.Reporting.WinForms;

using KeithLink.Common.Core.Enumerations;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Models.Logging;

using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Extensions.ShoppingCart;

using KeithLink.Svc.Core.Helpers;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Helpers;


namespace KeithLink.Svc.Impl.Logic
{
	public class ShoppingCartLogicImpl: IShoppingCartLogic {
        #region attributes
        private readonly ICacheRepository _cache;
        private readonly ICustomerRepository customerRepository;
        private readonly IBasketRepository basketRepository;
		private readonly ICatalogLogic catalogLogic;
		private readonly IPriceLogic priceLogic;
		private readonly IPurchaseOrderRepository purchaseOrderRepository;
		private readonly IGenericQueueRepository queueRepository;
		private readonly IBasketLogic basketLogic;
        private readonly INotesListLogic _notesLogic;
        private readonly IOrderQueueLogic orderQueueLogic;
        private readonly IOrderHistoryLogic _historyLogic;
		private readonly IAuditLogRepository auditLogRepository;
        private readonly IEventLogRepository _log;
        private readonly IUserActiveCartLogic _activeCartLogic;
        private readonly IExternalCatalogRepository _externalCatalogRepo;
        private readonly IOrderedFromListRepository _orderedFromListRepository;
        private readonly IOrderedItemsFromListRepository _orderedItemsFromListRepository;
        private readonly IRecommendedItemsOrderedAnalyticsRepository _recommendedItemsOrderedAnalyticsRepository;
        #endregion

        #region ctor
        public ShoppingCartLogicImpl(IBasketRepository basketRepository, ICatalogLogic catalogLogic, IPriceLogic priceLogic,
									 IOrderQueueLogic orderQueueLogic, IPurchaseOrderRepository purchaseOrderRepository, IGenericQueueRepository queueRepository,
									 IBasketLogic basketLogic, IOrderHistoryLogic orderHistoryLogic, IOrderedItemsFromListRepository orderedItemsFromListRepository,
                                     ICustomerRepository customerRepository, IAuditLogRepository auditLogRepository,
                                     INotesListLogic notesLogic, IUserActiveCartLogic userActiveCartLogic, IExternalCatalogRepository externalCatalogRepo,
                                     ICacheRepository cache, IEventLogRepository log, IOrderedFromListRepository orderedFromListRepository, 
                                     IRecommendedItemsOrderedAnalyticsRepository recommendedItemsOrderedAnalyticsRepository)
		{
            _cache = cache;
			this.basketRepository = basketRepository;
			this.catalogLogic = catalogLogic;
			this.priceLogic = priceLogic;
			this.purchaseOrderRepository = purchaseOrderRepository;
			this.queueRepository = queueRepository;
            _notesLogic = notesLogic;
			this.basketLogic = basketLogic;
            this.orderQueueLogic = orderQueueLogic;
			_historyLogic = orderHistoryLogic;
			this.customerRepository = customerRepository;
			this.auditLogRepository = auditLogRepository;
            _log = log;
            _activeCartLogic = userActiveCartLogic;
            _externalCatalogRepo = externalCatalogRepo;
            _orderedFromListRepository = orderedFromListRepository;
		    _orderedItemsFromListRepository = orderedItemsFromListRepository;
		    _recommendedItemsOrderedAnalyticsRepository = recommendedItemsOrderedAnalyticsRepository;
		}
        #endregion

        #region methods
		public Guid? AddItem(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, ShoppingCartItem newItem)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket == null)
				return null;

			//Does item already exist? If so, just update the quantity
			var existingItem = basket.LineItems.Where(l => l.ProductId.Equals(newItem.ItemNumber) && l.Each.Equals(newItem.Each));
			if (existingItem.Any())
			{
				existingItem.First().Quantity += newItem.Quantity;
                basketRepository.UpdateItem(basket.UserId.ToGuid(), cartId, existingItem.First());

				return existingItem.First().Id.ToGuid();
			}

            newItem.Position = basket.LineItems.Count;

		    if (String.IsNullOrEmpty(newItem.OrderedFromSource) == false) {
		        List<string> recommendedItemSources = _recommendedItemsOrderedAnalyticsRepository.GetOrderSources();

		        try {
		            if (recommendedItemSources.Contains(newItem.OrderedFromSource, StringComparer.CurrentCultureIgnoreCase) == false) {
		                _orderedItemsFromListRepository.Write(new OrderedItemFromList() {
		                    ControlNumber = cartId.ToString(),
		                    ItemNumber = newItem.ItemNumber,
		                    SourceList = newItem.OrderedFromSource
		                });
		            } else {
		                _recommendedItemsOrderedAnalyticsRepository.Add(newItem.ItemNumber,
		                                                                newItem.Each == false ? 'C' : 'P',
		                                                                newItem.OrderedFromSource,
		                                                                cartId.ToString(),
		                                                                newItem.TrackingKey);
		            }
		        } catch (Exception ex) {
		            _log.WriteErrorLog("There was an error adding analytics in AddItem routine", ex);
		        }
		    }

		    return basketRepository.AddItem(cartId, newItem.ToLineItem(), basket);
		}

		private string CartName(string name, UserSelectedContext catalogInfo)
		{
			return string.Format("s{0}_{1}_{2}", catalogInfo.BranchId.ToLower(), catalogInfo.CustomerId, Regex.Replace(name, @"\s+", ""));
		}
        
        public Guid CreateCart(UserProfile user, UserSelectedContext catalogInfo, ShoppingCart cart, string catalogId = null)
		{
			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            var newBasket = new CS.Basket();
            newBasket.BranchId = catalogInfo.BranchId.ToLower();
			newBasket.DisplayName = cart.Name;
			newBasket.ListType = (int)BasketType.Cart;
			newBasket.Name = CartName(cart.Name, catalogInfo);
			newBasket.CustomerId = catalogInfo.CustomerId;
			newBasket.Shared = true;
			newBasket.TempSubTotal = cart.SubTotal;
            newBasket.PONumber = cart.PONumber;
            newBasket.RequestedShipDate = cart.RequestedShipDate.ToFormattedDateString();
            var cartBranchId = catalogInfo.BranchId;
            if (catalogId != null)
                cartBranchId = catalogId;

            int startpos = 1;
            foreach (var item in cart.Items)
            {
                item.Position = startpos++;
            }

            Guid newCartId = basketRepository.CreateOrUpdateBasket(customer.CustomerId, 
                                                         cartBranchId.ToLower(), 
                                                         newBasket, 
                                                         cart.Items.Select(l => l.ToLineItem()).ToList());

		    if (cart.ListId != null) {
		        try {
		            _orderedFromListRepository.Write(new OrderedFromList() {
		                ControlNumber = newCartId.ToString(),
		                ListId = cart.ListId,
		                ListType = cart.ListType
		            });
		        } catch (Exception ex) {
                    _log.WriteErrorLog("Error saving ordered from list information on cart creation", ex);
		        }
            }

            // Get the recommended item sources to check for a valid source
	        List<string> recommendedItemSources = _recommendedItemsOrderedAnalyticsRepository.GetOrderSources();
	        
            foreach (var item in cart.Items)
            {
                if (String.IsNullOrEmpty(item.OrderedFromSource) == false) {
                    try
                    {
                        // If it isn't a recommended item process normally
                        if (recommendedItemSources.Contains(item.OrderedFromSource, StringComparer.CurrentCultureIgnoreCase) == false) {
                            _orderedItemsFromListRepository.Write(new OrderedItemFromList() {
                                ControlNumber = newCartId.ToString(),
                                ItemNumber = item.ItemNumber,
                                SourceList = item.OrderedFromSource
                            });
                        } else {
                            _recommendedItemsOrderedAnalyticsRepository.Add(item.ItemNumber,
                                item.Each == false ? 'C' : 'P',
                                item.OrderedFromSource,
                                newCartId.ToString(),
                                item.TrackingKey);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.WriteErrorLog("Error saving ordered item analytics on cart creation", ex);
                    }
                }
            }

            return newCartId;
		}

        public QuickAddReturnModel CreateQuickAddCart(UserProfile user, UserSelectedContext catalogInfo, List<QuickAddItemModel> items)
        {
            //Create a shoppingcart model and pass to the existing createcart method
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items = new List<ShoppingCartItem>();
            shoppingCart.BranchId = catalogInfo.BranchId;
            shoppingCart.Name = string.Format("Quick Add - {0} - {1} {2}", catalogInfo.CustomerId, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());

            var itemErrorMessage = new StringBuilder();

            foreach (var item in items)
            {
                if (item == null || string.IsNullOrEmpty(item.ItemNumber))
                    continue;

                //Verify they have access to item, if the item is invalid, log then return error
                var prod = catalogLogic.GetProductById(catalogInfo, item.ItemNumber, user, catalogInfo.BranchId);

                if (prod == null)
                    itemErrorMessage.AppendFormat("Item {0} is not a valid item #", item.ItemNumber);
                else
                {
                    shoppingCart.Items.Add(new ShoppingCartItem()
                    {
                        ItemNumber = item.ItemNumber,
                        Each = item.Each,
                        Quantity = item.Quantity,
                        CatalogId = catalogInfo.BranchId,
                        CasePriceNumeric = prod.CasePriceNumeric,
                        PackagePriceNumeric = prod.PackagePriceNumeric,
                        CatchWeight = prod.CatchWeight,
                        AverageWeight = prod.AverageWeight,
                        Pack = prod.Pack
                    }
//cart.SubTotal += (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, pack);
                    );

                    shoppingCart.SubTotal += (decimal)PricingHelper.GetPrice((int)item.Quantity, prod.CasePriceNumeric, prod.PackagePriceNumeric, item.Each, prod.CatchWeight, prod.AverageWeight, prod.Pack.ToInt(1));
                    
                }

            }

            if (itemErrorMessage.Length > 0)
                return new QuickAddReturnModel() { Success = false, ErrorMessage = itemErrorMessage.ToString() };

            return new QuickAddReturnModel() { Success = true, CartId = CreateCart(user, catalogInfo, shoppingCart) };

        }

        public void DeleteCart(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket == null)
				return;

			basketRepository.DeleteBasket(basket.UserId.ToGuid(), cartId);
		}
        
        public void DeleteCarts(UserProfile user, UserSelectedContext catalogInfo, List<Guid> cartIds)
		{
			foreach (var cartId in cartIds)
			{
				var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);
				if (basket != null)
					basketRepository.DeleteBasket(basket.UserId.ToGuid(), cartId);
			}
		}
		
        public void DeleteItem(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, Guid itemId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket == null)
				return;

			basketRepository.DeleteItem(basket.UserId.ToGuid(), cartId, itemId);
		}
        
		public void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, ShoppingCart cart, List<KeithLink.Svc.Core.Models.Lists.ListItemModel> notes = null)
		{
			if (cart.Items == null)
				return;

            var catalogList = cart.Items.GroupBy(p => p.CatalogId.ToLower());
            var products = new ProductsReturn() { Products = new List<Product>() };
            var pricing = new PriceReturn() { Prices = new List<Price>() };

            foreach (var catalogItems in catalogList) {
                var tempProducts = catalogLogic.GetProductsByIds(catalogItems.Key, 
                                                                 cart.Items.Where(i => i.CatalogId.Equals(catalogItems.Key, StringComparison.CurrentCultureIgnoreCase))
                                                                           .Select(i => i.ItemNumber)
                                                                           .Distinct()
                                                                           .ToList()
                                                                );
                products.AddRange(tempProducts);
                //if (catalogLogic.IsSpecialtyCatalog(null, catalogId)) {
                //    var source = catalogLogic.GetCatalogTypeFromCatalogId(catalogId);
                //    pricing.AddRange(priceLogic.GetNonBekItemPrices(catalogInfo.BranchId, catalogInfo.CustomerId, source, DateTime.Now.AddDays(1), tempProducts.Products));
                //} else {
                //    pricing.AddRange(priceLogic.GetPrices(catalogId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), tempProducts.Products)); //BEK
                //}
            }

            pricing.AddRange(priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), products.Products));

			var productHash = products.Products.ToDictionary(p => p.ItemNumber);
			var priceHash = pricing.Prices.ToDictionary(p => p.ItemNumber);
			var notesHash = new Dictionary<string, KeithLink.Svc.Core.Models.Lists.ListItemModel>();
			if (notes != null)
				notesHash = notes.ToDictionary(n => n.ItemNumber);
			

			Parallel.ForEach(cart.Items, item =>
			{
				var prod = productHash.ContainsKey(item.ItemNumber) ? productHash[item.ItemNumber] : null;
				var price = priceHash.ContainsKey(item.ItemNumber) ? priceHash[item.ItemNumber] : null;
				if (prod != null)
				{
                    item.IsValid = true;
					item.Name = prod.Name;
                    item.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                                    prod.Name,
                                                    prod.ItemNumber,
                                                    prod.BrandExtendedDescription,
                                                    prod.ItemClass,
                                                    prod.PackSize);
                    item.Pack = prod.Pack;
					item.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
					item.StorageTemp = prod.Nutritional == null ? "" : prod.Nutritional.StorageTemp;
					item.Brand = prod.Brand;
                    item.CategoryCode = prod.CategoryCode;
                    item.SubCategoryCode = prod.SubCategoryCode;
                    item.CategoryName = prod.CategoryName;
					item.ReplacedItem = prod.ReplacedItem;
					item.ReplacementItem = prod.ReplacementItem;
					item.NonStock = prod.NonStock;
					item.ChildNutrition = prod.ChildNutrition;
					item.CatchWeight = prod.CatchWeight;
                    item.TempZone = prod.TempZone;
                    item.AverageWeight = prod.AverageWeight;
					item.ItemClass = prod.ItemClass;
                    item.IsSpecialtyCatalog = prod.IsSpecialtyCatalog;
				}
				if (price != null)
				{
                    item.SpecialtyItemCost = prod.SpecialtyItemCost;
					item.PackagePrice = price.PackagePrice.ToString();
					item.CasePrice = price.CasePrice.ToString();

				}
                
				item.Notes = notesHash.ContainsKey(item.ItemNumber) ? notesHash[item.ItemNumber].Notes : null;
			});			
			
		}
		
        private void MarkCurrentActiveCartAsInactive(UserProfile user, UserSelectedContext catalogInfo, string branchId)
		{
			//var currentlyActiveCart = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, BasketType.Cart).Where(b => b.BranchId.Equals(branchId) && b.Active.Equals(true)).FirstOrDefault();

			//if (currentlyActiveCart != null)
			//{
			//	currentlyActiveCart.Active = false;
			//	basketRepository.CreateOrUpdateBasket(currentlyActiveCart.UserId.ToGuid(), currentlyActiveCart.BranchId, currentlyActiveCart, currentlyActiveCart.LineItems);
			//}
		}

		public List<ShoppingCart> ReadAllCarts(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly)
		{
            //System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(); //Temp: remove
            if (String.IsNullOrEmpty(catalogInfo.CustomerId))
				return new List<ShoppingCart>();
            
			var lists = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, BasketType.Cart);
            //EntreeStopWatchHelper.ReadStopwatch(stopWatch, _log, "ReadAllCarts - RetrieveAllSharedCustomerBaskets");

            var listForBranch = lists.Where(b => b.BranchId.Equals(catalogInfo.BranchId.ToLower()) &&
				!string.IsNullOrEmpty(b.CustomerId) &&
				b.CustomerId.Equals(catalogInfo.CustomerId));
            //EntreeStopWatchHelper.ReadStopwatch(stopWatch, _log, "ReadAllCarts - listForBranch");

            var userActiveCart = _activeCartLogic.GetUserActiveCart(catalogInfo, user.UserId);
            //EntreeStopWatchHelper.ReadStopwatch(stopWatch, _log, "ReadAllCarts - GetUserActiveCart");

            if (headerInfoOnly)
            {
                var returnCart = listForBranch.Select(b => ToShoppingCart(b, userActiveCart)).ToList();
                //EntreeStopWatchHelper.ReadStopwatch(stopWatch, _log, "ReadAllCarts - return");
                //return listForBranch.Select(l => new ShoppingCart()
                //{
                //    CartId = l.Id.ToGuid(),
                //    Name = l.DisplayName,
                //    Active = userActiveCart != null && userActiveCart.CartId == l.Id.ToGuid(),
                //    PONumber = l.PONumber,
                //    SubTotal = l.TempSubTotal.HasValue ? l.TempSubTotal.Value : 0,
                //    ItemCount = l.LineItems != null ? l.LineItems.Count() : 0,
                //    PieceCount = l.LineItems != null ? (int)l.LineItems.Sum(i => i.Quantity) : 0,
                //    RequestedShipDate = l.RequestedShipDate,
                //    CreatedDate = l.Properties["DateCreated"].ToString().ToDateTime().Value
                //}).ToList();

                returnCart.ForEach(delegate (ShoppingCart list)
                {
                    var o2l = _orderedFromListRepository.Read(list.CartId.ToString());
                    if (o2l != null)
                    {
                        list.ListId = o2l.ListId;
                        list.ListType = o2l.ListType;
                    }
                });

                return returnCart;
            }
            else
			{
				var returnCart = listForBranch.Select(b => ToShoppingCart(b, userActiveCart)).ToList();
				var notes = _notesLogic.GetNotes(user, catalogInfo);

				returnCart.ForEach(delegate(ShoppingCart list)
				{
					LookupProductDetails(user, catalogInfo, list, notes);

                    list.ItemCount = list.Items.Count;
                    list.PieceCount = (int)list.Items.Sum(i => i.Quantity);

                    var o2l = _orderedFromListRepository.Read(list.CartId.ToString());
                    if (o2l != null)
                    {
                        list.ListId = o2l.ListId;
                        list.ListType = o2l.ListType;
                    }

                    foreach (var item in list.Items) {
                        int qty = (int)item.Quantity;
                        list.SubTotal += (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, item.Pack.ToInt(1));
                    }
				});
                //EntreeStopWatchHelper.ReadStopwatch(stopWatch, _log, "ReadAllCarts - return");
                return returnCart;
			}
		}

        public bool IsSubmitted(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
        {
            var context = new TransactionContext
            {
                TransactionType = "customer shopping cart",
                TransactionId = "basket:" + cartId,
                ClassName = GetType().Name,
                MethodName = MethodBase.GetCurrentMethod().Name,
            };

            bool isSubmitted = OrderSubmissionHelper.CheckOrderBlock(user, catalogInfo, cartId, null, purchaseOrderRepository, null, _cache);
            if (isSubmitted)
            {
                string logMessage = string.Format("An order was already submitted from basket {0} for customer {1}.", cartId, catalogInfo.CustomerId);
                _log.WriteWarningLog(logMessage, context);
            }
            return isSubmitted;
        }

        public ShoppingCart ReadCart(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);
			if (basket == null)
				return null;
			var userActiveCart = _activeCartLogic.GetUserActiveCart(catalogInfo, user.UserId);
			var cart = ToShoppingCart(basket, userActiveCart);
            cart.Items = cart.Items.OrderBy(i => i.Position).ToList();
			var notes = _notesLogic.GetNotes(user, catalogInfo);

			LookupProductDetails(user, catalogInfo, cart, notes);

            cart.ItemCount = cart.Items.Count;
            cart.PieceCount = (int)cart.Items.Sum(i => i.Quantity);

		    decimal calcSubTotal = 0;

            calcSubTotal = (decimal)PricingHelper.CalculateCartSubtotal(cart.Items);

            foreach (var item in cart.Items) {
                int qty = (int)item.Quantity;
                if (item.PackSize != null && item.PackSize.IndexOf("/") > -1)
                { // added to aid exporting separate pack and size on cart export
                    item.Size = item.PackSize.Substring(item.PackSize.IndexOf("/") + 1);
                }

                var sourceList = _orderedItemsFromListRepository.Read(cart.CartId.ToString(), item.ItemNumber);
                if (sourceList != null) {
                    item.OrderedFromSource = sourceList.SourceList;
                }
            }

		    if (cart.SubTotal != calcSubTotal)
		    { // the calculated subtotal is changed from the saved total, so use it
		        cart.SubTotal = calcSubTotal;
		    }

            cart.ContainsSpecialItems = cart.Items.Any(i => i.IsSpecialtyCatalog);

	        var o2l = _orderedFromListRepository.Read(cartId.ToString());
		    if (o2l != null) {
                cart.ListId = o2l.ListId;
                cart.ListType = o2l.ListType;
            }

            return cart;
		}

        /// <summary>
        /// Combines a list and cart for report pdf printing
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <param name="cartId"></param>
        /// <param name="listId"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        private ShoppingCartReportModel PrintCartWithList( UserProfile user, UserSelectedContext context, Guid cartId, PagedListModel list) {
            ShoppingCart cart = ReadCart(user, context, cartId);

            if (cart == null || list == null)
                return null;
            
            List<ShoppingCartItemReportModel> cartReportItems = cart.Items.ToReportModel();
            List<ShoppingCartItemReportModel> clonedCartReportItems = cartReportItems.ConvertAll(i => i);
            List<ShoppingCartItemReportModel> listReportItems = list.ToShoppingCartItemReportList();
             
            clonedCartReportItems.ForEach(x => {
                ShoppingCartItemReportModel item = listReportItems.Where( i => i.ItemNumber.Equals( x.ItemNumber ) ).FirstOrDefault();
                if (item != null) {
                    //item.Category = x.Category;
                    //item.Label = x.Label;
                    item.Quantity = x.Quantity;
                    item.Each = x.Each;
                    item.ExtPrice = x.ExtPrice;

                    cartReportItems.Remove( cartReportItems.Where(p => p.ItemNumber.Equals(x.ItemNumber)).First() );
                }
            });

            return new ShoppingCartReportModel() { CartName = cart.Name, ListName = list.Name, CartItems = cartReportItems, ListItems = listReportItems };
        }

        public MemoryStream CartReport(UserProfile user, UserSelectedContext context, Guid cartId, PagedListModel list, PrintListModel options)
        {
            ReportViewer rv = new ReportViewer();

            Assembly assembly = Assembly.Load("KeithLink.Svc.Impl");

            Stream rdlcStream = null;
            var deviceInfo = string.Empty;
            if (options.Landscape)
            {
                deviceInfo = "<DeviceInfo><PageHeight>8.5in</PageHeight><PageWidth>11in</PageWidth></DeviceInfo>";
                rdlcStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Reports.CartReport_Landscape.rdlc");
            }
            else
            {
                deviceInfo = "<DeviceInfo><PageHeight>11in</PageHeight><PageWidth>8.5in</PageWidth></DeviceInfo>";
                rdlcStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Reports.CartReport.rdlc");
            }

            ShoppingCartReportModel reportModel = PrintCartWithList(user, 
                                                                    context, 
                                                                    cartId, 
                                                                    list);

            rv.LocalReport.LoadReportDefinition(rdlcStream);
            ReportParameter[] parameters = new ReportParameter[3];
            if (reportModel != null) {
                parameters[0] = new ReportParameter("ListName", reportModel.ListName);
                parameters[1] = new ReportParameter("CartName", reportModel.CartName);
                parameters[2] = new ReportParameter("ShowParValues", options.ShowParValues ? "true" : "false");

                rv.LocalReport.SetParameters(parameters);

                rv.LocalReport.DataSources.Add(new ReportDataSource("CartItems", reportModel.CartItems));
                rv.LocalReport.DataSources.Add(new ReportDataSource("ListItems", reportModel.ListItems));

                var bytes = rv.LocalReport.Render("PDF", deviceInfo);
                return new MemoryStream(bytes);
            }
            return null;
        }

        public SaveOrderReturn SaveAsOrder(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
		{
            var context = new TransactionContext
            {
                TransactionType = "customer shopping cart",
                TransactionId = "basket:" + cartId,
                ClassName = GetType().Name,
                MethodName = MethodBase.GetCurrentMethod().Name,
            };

            var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

			//Check that RequestedShipDate
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket.RequestedShipDate == null)
				throw new ApplicationException("Requested Ship Date is required before submitting an order");
            if (basket.LineItems == null || basket.LineItems.Count == 0)
                throw new ApplicationException("Cannot submit order with 0 line items");

            //use Commerce Server
            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();

            //split into multiple orders when multiple catalogs
            var catalogList = basket.LineItems.Select(i => i.CatalogName).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            var returnOrders = new SaveOrderReturn();
            returnOrders.NumberOfOrders = catalogList.Count;
            returnOrders.OrdersReturned = new List<NewOrderReturn>();

            string logMessage = null;
            if (catalogList.Count > 1)
            {
                logMessage = string.Format($"Splitting basket into {catalogList.Count} carts for customer {customer.CustomerNumber}.");
                _log.WriteInformationLog(logMessage, context);
            }

            OrderedFromList o2l = _orderedFromListRepository.Read(cartId.ToString());

            //make list of baskets
            foreach (var catalogId in catalogList)
            {
                var shoppingCart = new ShoppingCart()
                {
                    Name = "Split " + catalogId.ToUpper(),
                    RequestedShipDate = basket.RequestedShipDate,
                    Active = false,
                    PONumber = basket.PONumber,
                    CreatedDate = new DateTime(),
                    Items = basket.LineItems
                                  .Where(l => string.Equals(l.CatalogName, catalogId, StringComparison.CurrentCultureIgnoreCase))
                                  .OrderBy(l => int.Parse(l.Properties["LinePosition"].ToString()))
                                  .Select(l => new ShoppingCartItem() {
                                        ItemNumber = l.ProductId,
                                        Position = int.Parse(l.Properties["LinePosition"].ToString()),
                                        Notes = l.Notes,
                                        Quantity = l.Quantity.HasValue ? l.Quantity.Value : 0,
                                        Each = l.Each.HasValue ? l.Each.Value : false,
                                        CreatedDate = new DateTime(),
                                        CatalogId = l.CatalogName.ToUpper() })
                                  .ToList()
                };
                LookupProductDetails(user, catalogInfo, shoppingCart);

                logMessage = string.Format($"Calling CreateCart with {shoppingCart.Items.Count} items for catalog {catalogId} and customer {customer.CustomerNumber}.");
                _log.WriteInformationLog(logMessage, context);

                var newCartId = CreateCart(user, catalogInfo, shoppingCart, catalogId.ToUpper());
                string orderNumber = null;
                try
                {
                    orderNumber = client.SaveCartAsOrder(basket.UserId.ToGuid(), newCartId);
                    //if (catalogId == "unfi_7")
                    //    throw new Exception();// for testing
                    OrderSubmissionHelper.StartOrderBlock(cartId, orderNumber, _cache);
                    OrderSubmissionHelper.StartOrderBlock(newCartId, orderNumber, _cache);

                    if(o2l != null && o2l.ListId != null)
                    {
                        _orderedFromListRepository.Write(new OrderedFromList()
                        {
                            ControlNumber = orderNumber,
                            ListId = o2l.ListId.Value,
                            ListType = o2l.ListType
                        });
                    }
                }
                catch (Exception exception)
                {
                    logMessage = string.Format($"Exception while processing basket for customer {customer.CustomerNumber}.");
                    _log.WriteErrorLog(logMessage, exception, context);

                    // continuing may allow submission of remaining parts of a split order
                    // but allow the retention of a partial basket
                    continue;   
                }



                CS.PurchaseOrder purchaseOrder = purchaseOrderRepository.ReadPurchaseOrder(customer.CustomerId, orderNumber);

                var lineItems = ((CommerceRelationshipList)purchaseOrder.Properties["LineItems"]);
                foreach (var lineItem in lineItems)
                {
                    var item = (CS.LineItem)lineItem.Target;
                    var products = catalogLogic.GetProductsByIds(catalogId, new List<string>() {item.ProductId});
                    var filteredProducts = products.Products.Where(x => x.ItemNumber == item.ProductId).ToList();

                    if (filteredProducts.Count > 0) {
                        item.Notes = filteredProducts[0].Brand;

                        if(item.Each ?? false){
                            item.ListPrice = (decimal)filteredProducts[0].PackagePriceNumeric;
                        } else {
                            item.ListPrice = (decimal)filteredProducts[0].CasePriceNumeric;
                        }
                    }
                }

                // Log the control number to the recommended items for this cart
                _recommendedItemsOrderedAnalyticsRepository.UpdateAnalyticsForCardIdWithControlNumber(cartId.ToString(), orderNumber);

                string catalogType = catalogLogic.GetCatalogTypeFromCatalogId(catalogId).ToUpper().Substring(0, 3);

                bool isSpecialOrder = catalogLogic.IsSpecialtyCatalog(null, catalogId);

                // save to order history
                OrderHistoryFile orderHistoryFile = purchaseOrder.ToOrderHistoryFile(catalogInfo);
                _historyLogic.SaveOrder(orderHistoryFile, isSpecialOrder);

                logMessage = string.Format($"Calling WriteFileToQueue with control number {orderNumber} for customer {customer.CustomerNumber}.");
                _log.WriteInformationLog(logMessage, context);

                // post order to queue
                if (isSpecialOrder)
                {
                    client.UpdatePurchaseOrderStatus(customer.CustomerId, purchaseOrder.Id.ToGuid(), "Requested");

                    orderQueueLogic.WriteFileToQueue(user.EmailAddress, orderNumber, purchaseOrder, OrderType.SpecialOrder, catalogType, customer.DsrNumber, customer.Address.StreetAddress, customer.Address.City, customer.Address.RegionCode, customer.Address.PostalCode);
                }
                else
                {
                    orderQueueLogic.WriteFileToQueue(user.EmailAddress, orderNumber, purchaseOrder, OrderType.NormalOrder, catalogType); // send to queue - mainframe only for BEK
                }

                auditLogRepository.WriteToAuditLog(AuditType.OrderSubmited, user.EmailAddress, String.Format("Order: {0}, Customer: {1}", orderNumber, customer.CustomerNumber));

                returnOrders.OrdersReturned.Add(new NewOrderReturn() { OrderNumber = orderNumber, CatalogType = catalogType, IsSpecialOrder = isSpecialOrder });

                var itemsToDelete = basket.LineItems.Where(l => l.CatalogName.Equals(catalogId)).Select(l => l.Id).ToList();
                logMessage = string.Format($"Calling DeleteItem for {itemsToDelete.Count} items from basket for customer {customer.CustomerNumber}.");
                _log.WriteInformationLog(logMessage, context);
                foreach (var toDelete in itemsToDelete)
                {
                    DeleteItem(user, catalogInfo, cartId, toDelete.ToGuid());
                }

                if (isSpecialOrder)
                {
                    PublishSpecialOrderNotification(purchaseOrder);
                }
            }

            if (returnOrders.OrdersReturned.Count > 1) {
                string parentOrderId = returnOrders.OrdersReturned.Where(o => o.IsSpecialOrder == false).Select(ro => ro.OrderNumber).FirstOrDefault();

                List<string> childOrderIds = returnOrders.OrdersReturned.Where(o => o.IsSpecialOrder).Select(ro => ro.OrderNumber).ToList();

                foreach (string orderId in childOrderIds) {
                    _historyLogic.UpdateRelatedOrderNumber(orderId, parentOrderId);
                }
            }

            logMessage = string.Format($"{returnOrders.OrdersReturned.Count} orders submitted for customer {customer.CustomerNumber}.");
            _log.WriteInformationLog(logMessage, context);

            // delete original cart if all orders succeed
            if (returnOrders.NumberOfOrders == returnOrders.OrdersReturned.Count)
            {
                logMessage = string.Format($"Calling DeleteCart for customer {customer.CustomerNumber}.");
                _log.WriteInformationLog(logMessage, context);

                DeleteCart(user, catalogInfo, cartId);
            }

            return returnOrders; //Return actual order number
		}

        private void PublishSpecialOrderNotification(CS.PurchaseOrder po)
        {
            Order order = po.ToOrder();
            OrderChange orderChange = new OrderChange();
            orderChange.OrderName = (string)po.Properties["DisplayName"];
            orderChange.OriginalStatus = "Requested";
            orderChange.CurrentStatus = "Requested";
            orderChange.ItemChanges = new List<OrderLineChange>();
            orderChange.Items = new List<OrderLineChange>();
            orderChange.SpecialInstructions = "";
            orderChange.ShipDate = DateTime.MinValue.ToShortDateString();
            foreach (var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"]))
            {
                var item = (CS.LineItem)lineItem.Target;
                orderChange.Items.Add(new OrderLineChange()
                {
                    ItemNumber = item.ProductId,
                    ItemCatalog = item.CatalogName,
                    OriginalStatus = "Requested",
                    QuantityOrdered = (int)item.Quantity,
                    QuantityShipped = 0,
                    ItemPrice = item.PlacedPrice.Value
                });
            } 
            OrderConfirmationNotification orderConfNotification = new OrderConfirmationNotification();
            orderConfNotification.OrderChange = orderChange;
            orderConfNotification.OrderNumber = (string)po.Properties["OrderNumber"];
            orderConfNotification.CustomerNumber = (string)po.Properties["CustomerId"];
            orderConfNotification.BranchId = (string)po.Properties["BranchId"];
            orderConfNotification.InvoiceNumber = (string)po.Properties["InvoiceNumber"];

            queueRepository.PublishToDirectedExchange(orderConfNotification.ToJson(), Configuration.RabbitMQNotificationServer,
                Configuration.RabbitMQNotificationUserNamePublisher, Configuration.RabbitMQNotificationUserPasswordPublisher,
                Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotification, Constants.RABBITMQ_NOTIFICATION_ORDERCONFIRMATION_ROUTEKEY);
        }

        public void SetActive(UserProfile user, UserSelectedContext catalogInfo, Guid cartId) {
            //var cart = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

            //MarkCurrentActiveCartAsInactive(user, catalogInfo, catalogInfo.BranchId.ToLower());
            //cart.Active = true;
            //basketRepository.CreateOrUpdateBasket(cart.UserId.ToGuid(), cart.BranchId, cart, cart.LineItems);
        }

        private ShoppingCart ToShoppingCart(CS.Basket basket, UserActiveCartModel activeCart)
        {
            ShoppingCart sc = new ShoppingCart()
            {
                CartId = basket.Id.ToGuid(),
                Name = basket.DisplayName,
                BranchId = basket.BranchId,
                RequestedShipDate = basket.RequestedShipDate,
                Active = activeCart != null && activeCart.CartId == basket.Id.ToGuid(),
                PONumber = basket.PONumber,
                SubTotal = basket.TempSubTotal.HasValue ? basket.TempSubTotal.Value : 0,
                ItemCount = basket.LineItems != null ? basket.LineItems.Count() : 0,
                PieceCount = basket.LineItems != null ? (int)basket.LineItems.Sum(i => i.Quantity) : 0,
                CreatedDate = basket.Properties != null && basket.Properties["DateCreated"] != null ? basket.Properties["DateCreated"].ToString().ToDateTime().Value : DateTime.MinValue,
                Items = basket.LineItems != null ? basket.LineItems
                                                    .Select(l => new ShoppingCartItem()
                                                    {
                                                        ItemNumber = l.ProductId,
                                                        CartItemId = l.Id.ToGuid(),
                                                        strPosition = l.LinePosition,
                                                        Notes = l.Notes,
                                                        Quantity = l.Quantity.HasValue ? l.Quantity.Value : 0,
                                                        Each = l.Each.HasValue ? l.Each.Value : false,
                                                        Label = l.Label,
                                                        CreatedDate = l.Properties["DateCreated"].ToString().ToDateTime().Value,
                                                        CatalogId = l.CatalogName.ToUpper()
                                                    }).ToList() : new List<ShoppingCartItem>()
            };
            foreach (ShoppingCartItem item in sc.Items)
            {
                int pos;
                int.TryParse(item.strPosition, out pos);
                if (pos > 0) item.Position = pos;
            }
            return sc;
        }


        public void UpdateCart(UserSelectedContext catalogInfo, UserProfile user, ShoppingCart cart, bool deleteOmmitedItems)
		{
			var updateCart = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cart.CartId);
			
			if (updateCart == null)
				return;

			updateCart.DisplayName = cart.Name;
			updateCart.Name = CartName(cart.Name, catalogInfo);
			updateCart.TempSubTotal = cart.SubTotal;
            updateCart.RequestedShipDate = cart.RequestedShipDate;
			updateCart.PONumber = cart.PONumber;

		    try {
		        OrderedFromList o2l = _orderedFromListRepository.Read(cart.CartId.ToString());
		        if (o2l == null &&
		            cart.ListId != null) {
		            _orderedFromListRepository.Write(new OrderedFromList() {
		                ControlNumber = cart.CartId.ToString(),
		                ListId = cart.ListId.Value,
		                ListType = cart.ListType
		            });
		        } else if (o2l != null &&
		                   o2l.ListId != cart.ListId) {
		            _orderedFromListRepository.Delete(cart.CartId.ToString());
		            if (cart.ListId != null) {
		                _orderedFromListRepository.Write(new OrderedFromList() {
		                    ControlNumber = cart.CartId.ToString(),
		                    ListId = cart.ListId.Value,
		                    ListType = cart.ListType
		                });
		            }
		        }
		    } catch { }

		    var itemsToRemove = new List<Guid>();
		    var itemNumbersToRemoveAnalyticsFor = new List<string>();
			var lineItems = new List<CS.LineItem>();

			if (cart.Items != null)
			{
				itemsToRemove = updateCart.LineItems.Where(b => !cart.Items.Any(c => c.CartItemId.ToCommerceServerFormat().Equals(b.Id))).Select(l => l.Id.ToGuid()).ToList();

                // This is to handle SalesIQ and CartIQ Deletions from the analytics. It needs to be refactored into a real solution.
				itemNumbersToRemoveAnalyticsFor = updateCart.LineItems.Where(b => !cart.Items.Any(c => c.CartItemId.ToCommerceServerFormat().Equals(b.Id))).Select(l => l.ProductId).ToList();

			    List<string> recommendedAnalyticsSources = _recommendedItemsOrderedAnalyticsRepository.GetOrderSources();

                foreach (var item in cart.Items)
				{
                    if(item.CartItemId==new Guid() && item.Position == 1)
                    {
                        item.Position = cart.ItemCount;
                    }

					var existingItem = updateCart.LineItems.Where(l => l.ProductId.Equals(item.ItemNumber));
                    // Commenting on this mystery, I believe it is for quick add items
				    if (existingItem.Any() &&
				        item.CartItemId == Guid.Empty) {
				        existingItem.First().Quantity += item.Quantity;
				        lineItems.Add(existingItem.First());
				    } else {
				        lineItems.Add(item.ToLineItem());
				    }

				    if (String.IsNullOrEmpty(item.OrderedFromSource) == false)
                    {
                        try {
                            if (recommendedAnalyticsSources.Contains(item.OrderedFromSource, StringComparer.CurrentCultureIgnoreCase)) {
                                _recommendedItemsOrderedAnalyticsRepository.Add(item.ItemNumber,
                                                                                item.Each == false ? 'C' : 'P',
                                                                                item.OrderedFromSource,
                                                                                cart.CartId.ToString(),
                                                                                null);
                            }
                        } catch (Exception ex) {
                            _log.WriteErrorLog("Error adding recommended analytics for items in UpdateCart", ex);
                        }
                    }
                }
            }

			var duplicates = lineItems.Cast<CS.LineItem>().GroupBy(l => new { l.ProductId, l.Each }).Select(i => new { Ech = i.Select(p => p.Each).First(), Key = i.Key, Cnt = i.Count() }).Where(w => w.Cnt > 1).ToList();

			foreach (var duplicate in duplicates)
			{
				var keepGuid = lineItems.Where(l => l.ProductId.Equals(duplicate.Key.ProductId) && l.Each.Equals((bool)duplicate.Key.Each)).First();

				lineItems.Where(i => i.Id.Equals(keepGuid.Id)).First().Quantity = lineItems.Where(l => l.ProductId.Equals(duplicate.Key.ProductId) && l.Each.Equals(duplicate.Key.Each)).Sum(s => s.Quantity);

				itemsToRemove.AddRange(lineItems.Where(l => l.ProductId.Equals(duplicate.Key.ProductId) && l.Each.Equals(duplicate.Key.Each) && !l.Id.Equals(keepGuid.Id)).Select(p => p.Id.ToGuid()).ToList());

				itemNumbersToRemoveAnalyticsFor.AddRange(lineItems.Where(l => l.ProductId.Equals(duplicate.Key.ProductId) && l.Each.Equals(duplicate.Key.Each) && !l.Id.Equals(keepGuid.Id)).Select(p => p.ProductId).ToList());
			}

			basketRepository.CreateOrUpdateBasket(updateCart.UserId.ToGuid(), updateCart.BranchId, updateCart, lineItems);

		    if (deleteOmmitedItems) {
		        foreach (var toDelete in itemsToRemove) {
		            basketRepository.DeleteItem(updateCart.UserId.ToGuid(), cart.CartId, toDelete);
		        }

		        foreach (string itemNumber in itemNumbersToRemoveAnalyticsFor) {
		            _recommendedItemsOrderedAnalyticsRepository.DeleteByCartIdAndItemNumber(cart.CartId.ToString(), itemNumber);
		        }
		    }
		}
        
        public void UpdateItem(UserProfile user,  UserSelectedContext catalogInfo, Guid cartId, ShoppingCartItem updatedItem)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);
			if (basket == null)
				return;

			basketRepository.UpdateItem(basket.UserId.ToGuid(), cartId, updatedItem.ToLineItem());
		}

		public List<ItemValidationResultModel> ValidateItems(UserSelectedContext catalogInfo, UserProfile user, List<QuickAddItemModel> productsToValidate)
		{
			int totalProcessed = 0;
			ProductsReturn products = new ProductsReturn() { Products = new List<Product>() };

			while (totalProcessed < productsToValidate.Count)
			{  
				var batch = productsToValidate.Skip(totalProcessed).Take(50).Select(i => i.ItemNumber).ToList();

				var tempProducts = catalogLogic.GetProductsByIdsWithPricing(catalogInfo, batch);

				products.Products.AddRange(tempProducts.Products);
				totalProcessed += 50;
			}
      
            if(products.Products.Count() < productsToValidate.Count())
            {
                if (ProfileHelper.CheckCanViewUNFI(user, catalogInfo.CustomerId, catalogInfo.BranchId, customerRepository, _log))
                {
                    LookupUnfoundProductsInExternalCatalogs(catalogInfo, productsToValidate, products);
                }
            }

            var productHash = products.Products.GroupBy(p => p.ItemNumber).Select(i => i.First()).ToDictionary(p => p.ItemNumber);

			var results = new List<ItemValidationResultModel>();

			foreach (var item in productsToValidate)
			{
                try
                {
                    var product = productHash.ContainsKey(item.ItemNumber) ? productHash[item.ItemNumber] : null;

                    if (product == null)
                        results.Add(new ItemValidationResultModel() { Product = product, Item = item, Valid = false, Reason = InvalidReason.InvalidItemNumber });
                    else
                    {
                        if (item.Each)
                        {
                            if (product.CaseOnly)
                            {
                                results.Add(new ItemValidationResultModel() { Product = product, Item = item, Valid = false, Reason = InvalidReason.EachNotAllowed });
                            }
                            else if (product.PackagePriceNumeric > 0)
                            {
                                results.Add(new ItemValidationResultModel() { Product = product, Item = item, Valid = true });
                            }
                            else
                            {
                                results.Add(new ItemValidationResultModel() { Product = product, Item = item, Valid = false, Reason = InvalidReason.EachNotAllowed });
                            }
                        }
                        else
                        {
                            if (product.CasePriceNumeric > 0)
                            {
                                results.Add(new ItemValidationResultModel() { Product = product, Item = item, Valid = true });
                            }
                            else
                            {
                                results.Add(new ItemValidationResultModel() { Product = product, Item = item, Valid = false, Reason = InvalidReason.InvalidItemNumber });
                            }
                        }
                    }
                }
                catch
                {
                    results.Add(new ItemValidationResultModel() { Product = null, Item = item, Valid = false, Reason = InvalidReason.InvalidItemNumber });
                }
            }

			return results;
		}

        private void LookupUnfoundProductsInExternalCatalogs(UserSelectedContext catalogInfo, List<QuickAddItemModel> productsToValidate, ProductsReturn products)
        {
            List<string> found = products.Products.Select(i => i.ItemNumber).ToList();
            List<string> batch = productsToValidate.Where(i => found.Contains(i.ItemNumber) == false).Select(i => i.ItemNumber).ToList();
            Dictionary<string, string> externalCatalogDict =
                _externalCatalogRepo.ReadAll().ToDictionary(e => e.BekBranchId.ToLower(), e => e.ExternalBranchId);

            var tempProducts = catalogLogic.GetProductsByIds(externalCatalogDict[catalogInfo.BranchId.ToLower()], batch);
            catalogLogic.AddPricingInfo(tempProducts, catalogInfo, new SearchInputModel());

            if (tempProducts.Products.Count() > 0)
            {
                products.Products.AddRange(tempProducts.Products);
            }
        }
        #endregion
    }
}
