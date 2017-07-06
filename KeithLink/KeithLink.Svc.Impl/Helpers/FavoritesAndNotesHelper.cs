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
    public class FavoritesAndNotesHelper
    {
        public static List<Product> GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, List<Product> prods, IListService listService)
        {
            ListModel listModel = new ListModel();
            listModel.Items = new List<ListItemModel>();
            foreach (Product prod in prods) {
                listModel.Items.Add(new ListItemModel() { ItemNumber = prod.ItemNumber });
            }

            listModel = listService.MarkFavoritesAndAddNotes(user, listModel, catalogInfo);

            Parallel.ForEach(prods, prod =>
            {
                ListItemModel listItem = listModel.Items
                                                  .Where(li => li.ItemNumber == prod.ItemNumber)
                                                  .First();

                prod.Favorite = listItem.Favorite;
                prod.Notes = listItem.Notes;
            });

            return prods;
        }

        public static Product GetFavoritesAndNotesFromLists(UserProfile user, UserSelectedContext catalogInfo, Product prod, IListService listService)
        {
            ListModel listModel = new ListModel();
            listModel.Items = new List<ListItemModel>();
            listModel.Items.Add(new ListItemModel() { ItemNumber = prod.ItemNumber });

            listModel = listService.MarkFavoritesAndAddNotes(user, listModel, catalogInfo);

            try {
                ListItemModel listItem = listModel.Items
                                                  .Where(li => li.ItemNumber == prod.ItemNumber)
                                                  .First();

                prod.Favorite = listItem.Favorite;
                prod.Notes = listItem.Notes;
            }
            catch { }

            return prod;
        }

    }
}
