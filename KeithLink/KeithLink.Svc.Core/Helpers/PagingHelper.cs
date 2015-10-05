// KeithLink
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Paging;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Helpers {
    public static class PagingHelper {

        /// <summary>
        /// Build the paging filter from the PrintListModel
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static PrintListModel BuildPagingFilter(PrintListModel options) {
            if (!string.IsNullOrEmpty( options.Paging.Terms )) {
                //Build filter
                options.Paging.Filter = new FilterInfo() {
                    Field = "ItemNumber",
                    FilterType = "contains",
                    Value = options.Paging.Terms,
                    Condition = "||",
                    Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = options.Paging.Terms, FilterType = "contains" }, new FilterInfo() { Condition = "||", Field = "Name", Value = options.Paging.Terms, FilterType = "contains" } }
                };
            }

            options.Paging.Size = int.MaxValue;
            options.Paging.From = 0;

            if (options.Paging.Sort.Count == 1 && options.Paging.Sort[0].Field == null) {
                options.Paging.Sort = new List<SortInfo>();
            }

            return options;
        }

    }
}
