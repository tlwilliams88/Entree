using KeithLink.Svc.Core.Models.ContentManagement;
using EE = KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.ContentManagement {
    public static class ContentItemExtension {
        public static ContentItemViewModel ToContentItemViewModel(this EE.ContentItem value, string branchId) {
            ContentItemViewModel retVal = new ContentItemViewModel();


            retVal.ImageUrl = value.BannerUrl.Length == 0 ? string.Empty : value.BannerUrl;
            retVal.ContentItemId = value.EntryId;
            retVal.BranchId = branchId;
            retVal.TagLine = string.Empty;
            retVal.TargetUrlText = value.Title;
            //retVal.TargetUrl = 
            retVal.CampaignId = value.AdditionalData.Count == 0 ? string.Empty : value.AdditionalData[0].CampaignId;
            retVal.Content = value.Summary;
            retVal.IsContentHtml = true;
            retVal.ProductId = (value.AdditionalData.Count > 0 && value.AdditionalData[0].ProductId.Count > 0) ? value.AdditionalData[0].ProductId[0].ItemNumber : string.Empty;

            DateTime editDate = new DateTime();
            DateTime.TryParse(value.EditDate, out editDate);

            retVal.ActiveDateStart = editDate;
            retVal.ActiveDateEnd = editDate;

            return retVal;
        }
    }
}
