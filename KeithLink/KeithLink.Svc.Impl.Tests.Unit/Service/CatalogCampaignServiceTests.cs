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
            Mock<ICatalogLogic> logic = new Mock<ICatalogLogic>();

            return logic;
        }

        private static Mock<ICatalogCampaignLogic> MockCampaignLogic() {
            Mock<ICatalogCampaignLogic> logic = new Mock<ICatalogCampaignLogic>();

            return logic;
        }


        private static CatalogCampaignServiceImpl MakeService(ICatalogLogic catalogLogic = null, ICatalogCampaignLogic campaignLogic = null, 
                                                              ICampaignCustomerRepository campaignCustomerRepository = null) { 
            if(catalogLogic == null) { catalogLogic = MockCatalogLogic().Object; }
            if(campaignLogic == null) { campaignLogic = MockCampaignLogic().Object; }

            return new CatalogCampaignServiceImpl(catalogLogic, campaignLogic);
        }

        public class GetCatalogCampaignProducts { }
    }
}
