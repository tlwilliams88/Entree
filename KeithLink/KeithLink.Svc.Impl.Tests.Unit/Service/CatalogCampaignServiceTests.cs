using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Service;

namespace KeithLink.Svc.Impl.Tests.Unit.Service {
    public class CatalogCampaignServiceTests {
        private static Mock<ICatalogLogic> MockCatalogLogic() { 

        }

        private static Mock<ICatalogCampaignLogic> MockCampaignLogic() { }

        private static CatalogCampaignServiceImpl MakeService(ICatalogLogic catalogLogic = null, ICatalogCampaignLogic campaignLogic = null) { 
            if(catalogLogic == null) { catalogLogic = MockCatalogLogic().Object; }
            if(campaignLogic == null) { campaignLogic = MockCampaignLogic().Object; }

            return new CatalogCampaignServiceImpl(catalogLogic, campaignLogic);
        }

        public class GetCatalogCampaignProducts { }
    }
}
