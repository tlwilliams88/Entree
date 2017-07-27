using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Helpers
{
    /* For now this helper is in the Impl assembly (for testing), but should only be called from a controller in WebApi */
    public class FavoritesAndNotesHelper
    {
        public static List<Product> GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, List<Product> prods, IListService listService)
        {
            List<Product> products = new List<Product>();
            foreach (Product prod in prods) {
                products.Add(new Product() { ItemNumber = prod.ItemNumber });
            }

            products = listService.MarkFavoritesAndAddNotes(user, products, catalogInfo);

            if (products != null && products.Count > 0) {
                foreach (Product prod in prods) {
                    SetFavoriteNotesAndInHistory(prod, products);
                }
            }

            return prods;
        }

        public static Product GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, Product prod, IListService listService) {

            List<Product> products = new List<Product>();
            products.Add(new Product() {ItemNumber = prod.ItemNumber});

            products = listService.MarkFavoritesAndAddNotes(user, products, catalogInfo);

            SetFavoriteNotesAndInHistory(prod, products);

            return prod;
        }

        private static void SetFavoriteNotesAndInHistory(Product prod, List<Product> products) {
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
                        prod.InHistory = prd.InHistory;
                    }
                }
            }
        }
    }
}
