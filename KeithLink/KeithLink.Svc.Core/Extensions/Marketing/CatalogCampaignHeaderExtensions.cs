using KeithLink.Svc.Core.Models.Marketing;

namespace KeithLink.Svc.Core.Extensions.Marketing
{
    public static class CatalogCampaignHeaderExtensions
    {
        public static CatalogCampaignReturnModel ToModel(this CatalogCampaignHeader from, string presentationUrl, string imageBaseUrl)
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

            if (string.IsNullOrEmpty(from.LinkToUrl)) {
                to.LinkToUrl = string.Format("{0}/#/catalog/campaign/{1}", presentationUrl, from.Uri);
            } else {
                to.LinkToUrl = from.LinkToUrl;
            }

            if (!string.IsNullOrEmpty(imageBaseUrl)) {
                to.ImageDesktop = string.Format("{0}/{1}_desktop.jpg", imageBaseUrl, from.Uri);
                to.ImageMobile = string.Format("{0}/{1}_mobile.jpg", imageBaseUrl, from.Uri);
            }

            return to;
        }
    }
}
