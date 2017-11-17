using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Marketing;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Marketing {
    public class CatalogCampaignsReturnModelTests {
        private static CatalogCampaignsReturnModel MakeModel() {
            return new CatalogCampaignsReturnModel() {
                campaigns = new List<CatalogCampaignReturnModel>() {
                    new CatalogCampaignReturnModel() {
                        Id = 23
                    },
                    new CatalogCampaignReturnModel() {
                        Id = 46
                    }
                }
            };
        }

        public class Campaigns {
            [Fact]
            public void GoodModel_HasExpectedRowCount() {
                // arrange
                var model = MakeModel();
                var expected = 2;

                // act

                // assert
                model.campaigns
                     .Count
                     .Should()
                     .Be(expected);
            }

            [Fact]
            public void GoodModelRecord1_HasExpectedId() {
                // arrange
                var model = MakeModel();
                var expected = 23;

                // act

                // assert
                model.campaigns[0]
                     .Id
                     .Should()
                     .Be(expected);
            }

            [Fact]
            public void GoodModelRecord2_HasExpectedId() {
                // arrange
                var model = MakeModel();
                var expected = 46;

                // act

                // assert
                model.campaigns[1]
                     .Id
                     .Should()
                     .Be(expected);
            }

            [Fact]
            public void InitializedModel_HasDefaultValue() {
                // arrange
                var model = new CatalogCampaignsReturnModel();

                // act

                // assert
                model.campaigns
                     .Should()
                     .BeEmpty();
            }
        }
    }
}
