using System;

using KeithLink.Common.Core.Seams;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class ContractDetailExtensions {
        public static ListItemModel ToWebModel(this ContractListDetail value) {
            ListItemModel retVal = new ListItemModel();

            retVal.ListItemId = value.Id;
            retVal.Type = ListType.Contract;
            retVal.Category = value.Category;
            retVal.ItemNumber = value.ItemNumber;
            retVal.FromDate = value.FromDate;
            retVal.ToDate = value.ToDate;
            retVal.Position = value.LineNumber;
            retVal.ModifiedUtc = value.ModifiedUtc;
            retVal.CreatedUtc = value.CreatedUtc;
            retVal.Each = value.Each ?? false;
            retVal.CatalogId = value.CatalogId;

            if(value.CreatedUtc.AddDays(Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > SystemTime.Now) {
                retVal.Delta = Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED + " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE;
            } else {
                if(value.ToDate != null && value.ToDate.Value < SystemTime.Now) {
                    retVal.Delta = Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED;
                } else {
                    retVal.Delta = Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE;
                }
            }
            
            return retVal;
        }
    }
}
