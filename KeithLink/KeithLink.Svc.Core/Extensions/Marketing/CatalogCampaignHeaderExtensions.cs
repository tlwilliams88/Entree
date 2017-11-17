using KeithLink.Svc.Core.Models.Marketing;

namespace KeithLink.Svc.Core.Extensions.Marketing
{
    public static class CatalogCampaignHeaderExtensions
    {
        public static CatalogCampaignReturnModel ToModel(this CatalogCampaignHeader from)
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

            return to;
        }
    }
}
