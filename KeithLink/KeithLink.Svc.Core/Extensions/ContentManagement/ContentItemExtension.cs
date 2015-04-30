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
            retVal.TargetUrl = value.UrlTitle;
            retVal.CampaignId = value.AdditionalData.Count == 0 ? string.Empty : value.AdditionalData[0].CampaignId;
            //retVal.Content = value.Summary;
            //retVal.IsContentHtml = true;
            retVal.IsContentHtml = false;
            retVal.ProductId = (value.AdditionalData.Count > 0 && value.AdditionalData[0].ProductId.Count > 0) ? value.AdditionalData[0].ProductId[0].ItemNumber : string.Empty;

            // takes too long to process
            //DateTime editDate = new DateTime();
            //DateTime.TryParse(value.EditDate, out editDate);

            DateTime editDate;

            if (value.EditDate.Length == 14) {
                int year = int.Parse(value.EditDate.Substring(0, 4));
                int month = int.Parse(value.EditDate.Substring(4, 2));
                int day = int.Parse(value.EditDate.Substring(6, 2));
                int hour = int.Parse(value.EditDate.Substring(8, 2));
                int minute = int.Parse(value.EditDate.Substring(10, 2));
                int second = int.Parse(value.EditDate.Substring(12, 2));

                editDate = new DateTime(year, month, day, hour, minute, second);
            } else {
                editDate = DateTime.MinValue;
            }

            retVal.ActiveDateStart = editDate;
            retVal.ActiveDateEnd = editDate;

            return retVal;
        }
    }
}
