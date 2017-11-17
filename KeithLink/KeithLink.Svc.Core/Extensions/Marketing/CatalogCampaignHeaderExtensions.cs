using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Marketing;

namespace KeithLink.Svc.Core.Extensions.Marketing
{
    public static class CatalogCampaignHeaderExtensions
    {
        public static CatalogCampaignReturnModel ToModel(this CatalogCampaignHeader from, string imageBaseUrl)
        {
            CatalogCampaignReturnModel to = new CatalogCampaignReturnModel();
            to.Id          = from.Id;
            to.Uri         = from.Uri;
            to.Description = from.Description;
            to.Active      = from.Active;
            to.StartDate   = from.StartDate;
            to.EndDate     = from.EndDate;
            to.Name        = from.Name;
            to.HasFilter   = from.HasFilter;
            to.LinkToUrl   = from.LinkToUrl;

            if (!string.IsNullOrEmpty(imageBaseUrl)) {
                to.ImageDesktop = string.Format("{0}/{1}_desktop.jpg", imageBaseUrl, from.Uri);
                to.ImageMobile = string.Format("{0}/{1}_mobile.jpg", imageBaseUrl, from.Uri);
            }

            return to;
        }

        public static List<CatalogCampaignReturnModel> ToWebModel(this IEnumerable<CatalogCampaignHeader> from, string imageBaseUrl) {
            List<CatalogCampaignReturnModel> to = new List<CatalogCampaignReturnModel>();

            foreach (CatalogCampaignHeader campaign in from) {
                to.Add(campaign.ToModel(imageBaseUrl));
            }

            return to;
        }
    }
}
