using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
    public class ListSortHelper
    {
        public static string GetSort(string setting)
        {
            StringBuilder sort = new StringBuilder();
            int cursor = 0;
            while (cursor < setting.Length)
            {
                switch (setting.Substring(cursor, 1))
                {
                    case "0":
                        sort.Append("Position");
                        break;
                    case "1":
                        sort.Append("ItemNumber");
                        break;
                    case "2":
                        sort.Append("Name");
                        break;
                    case "3":
                        sort.Append("BrandExtendedDescription");
                        break;
                    case "4":
                        sort.Append("ItemClass");
                        break;
                    case "5":
                        sort.Append("Notes");
                        break;
                    case "6":
                        sort.Append("Label");
                        break;
                    case "7":
                        sort.Append("ParLevel");
                        break;
                }
                cursor++;
                switch (setting.Substring(cursor, 1))
                {
                    case "y":
                        sort.Append(",desc");
                        break;
                    case "n":
                        sort.Append(",asc");
                        break;
                }
                cursor++;
                if (cursor < setting.Length)
                {
                    sort.Append(";");
                }
            }
            return sort.ToString();
        }

        ///// <summary>
        ///// Sort list items given an unparsed string with sort information
        ///// </summary>
        ///// <param name="sortinfo">A string with unparsed sort info for the list items (of the form "fld1,ord1[;fld2,ord2]")</param>
        ///// <param name="items">A list of shopping cart items</param>
        ///// <returns>A list of shopping cart items in the order described by sortinfo with position calibrated to that sort</returns>
        public static List<ListItemModel> SortOrderItems(string sortinfo, List<ListItemModel> items)
        {
            IQueryable<ListItemModel> stmt = items.AsQueryable();
            string[] sortpairs = sortinfo.Split(';');
            int ind = 0;
            foreach (string sortpair in sortpairs)
            {
                string fld = sortpair.Substring(0, sortpair.IndexOf(','));
                string ord = sortpair.Substring(sortpair.IndexOf(',') + 1);
                stmt = stmt.OrderingHelper(fld, ord.Equals("desc", StringComparison.CurrentCultureIgnoreCase), ind > 0);
                ind++;
            }
            return stmt.ToList();
        }
    }
}
