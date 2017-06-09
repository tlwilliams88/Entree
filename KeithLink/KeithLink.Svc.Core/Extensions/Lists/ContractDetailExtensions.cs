using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class ContractDetailExtensions {
        public static ListItemModel ToWebModel(this ContractListDetail value) {
            return new ListItemModel() {
                ListItemId = value.Id,
                Type = ListType.Contract,
                Category = value.Category,
                ItemNumber = value.ItemNumber,
                Delta = (value.CreatedUtc.AddDays(Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? 
                            Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED + " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                            (value.ToDate != null && value.ToDate.Value < DateTime.Now) ? 
                                Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                                Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                FromDate = value.FromDate,
                ToDate = value.ToDate,
                Position = value.LineNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId
            };
        }
    }
}
