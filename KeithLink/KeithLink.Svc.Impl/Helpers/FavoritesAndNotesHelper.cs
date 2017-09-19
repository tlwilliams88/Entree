using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Helpers
{
    /* For now this helper is in the Impl assembly (for testing), but should only be called from a controller in WebApi */
    public class FavoritesAndNotesHelper
    {
        public static void GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, List<ShoppingCartItem> prods, IListService listService)
        {
            PullInformationFromDataAndDistribute(user, catalogInfo, prods, listService);
        }

        public static void GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, List<OrderLine> prods, IListService listService)
        {
            PullInformationFromDataAndDistribute(user, catalogInfo, prods, listService);
        }

        public static List<Product> GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, List<Product> prods, IListService listService) {
            PullInformationFromDataAndDistribute(user, catalogInfo, prods, listService);

            return prods;
        }

        public static Product GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, Product prod, IListService listService) {

            List<Product> products = new List<Product>();

            products.Add(new Product() {ItemNumber = prod.ItemNumber});

            products = listService.MarkFavoritesAndAddNotes(user, products, catalogInfo);

            SetFavoriteNotesAndInHistory(prod, products);

            return prod;
        }

        private static void PullInformationFromDataAndDistribute(UserProfile user, UserSelectedContext catalogInfo, dynamic prods, IListService listService)
        {
            List<Product> products = new List<Product>();

            LoadProductsToGetInformation(prods, products);

            GetInformationForItems(user, catalogInfo, listService, products);

            if (products != null &&
                products.Count > 0)
            {
                DistributeInformation(prods, products);
            }
        }

        private static void GetInformationForItems(UserProfile user, UserSelectedContext catalogInfo, IListService listService, dynamic products)
        {
            products = listService.MarkFavoritesAndAddNotes(user, products, catalogInfo);
        }

        private static void DistributeInformation(dynamic prods, List<Product> products)
        {
            foreach (dynamic prod in prods)
            {
                SetFavoriteNotesAndInHistory(prod, products);
            }
        }

        private static void LoadProductsToGetInformation(dynamic prods, List<Product> products)
        {
            foreach (BaseProductInfo prod in prods)
            {
                products.Add(new Product() { ItemNumber = prod.ItemNumber });
            }
        }

        private static void SetFavoriteNotesAndInHistory(ShoppingCartItem prod, List<Product> products)
        {
            if (products != null)
            {
                var prdList = products.Where(p => p.ItemNumber == prod.ItemNumber)
                                      .ToList();

                if (prdList != null &&
                    prdList.Count > 0)
                {
                    Product prd = products.Where(p => p.ItemNumber == prod.ItemNumber)
                                          .First();

                    if (prd != null)
                    {
                        prod.Favorite = prd.Favorite;
                        prod.Notes = prd.Notes;
                    }
                }
            }
        }

        private static void SetFavoriteNotesAndInHistory(Product prod, List<Product> products)
        {
            if (products != null)
            {
                var prdList = products.Where(p => p.ItemNumber == prod.ItemNumber)
                                      .ToList();

                if (prdList != null &&
                    prdList.Count > 0)
                {
                    Product prd = products.Where(p => p.ItemNumber == prod.ItemNumber)
                                          .First();

                    if (prd != null)
                    {
                        prod.Favorite = prd.Favorite;
                        prod.Notes = prd.Notes;
                        prod.InHistory = prd.InHistory;
                    }
                }
            }
        }

        private static void SetFavoriteNotesAndInHistory(OrderLine prod, List<Product> products) {
            if (products != null) {
                var prdList = products.Where(p => p.ItemNumber == prod.ItemNumber)
                                      .ToList();

                if (prdList != null &&
                    prdList.Count > 0) {
                    Product prd = products.Where(p => p.ItemNumber == prod.ItemNumber)
                                          .First();

                    if (prd != null) {
                        prod.Favorite = prd.Favorite;
                        prod.Notes = prd.Notes;
                    }
                }
            }
        }
    }
}
