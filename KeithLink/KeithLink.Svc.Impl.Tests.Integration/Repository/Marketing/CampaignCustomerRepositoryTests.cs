using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Marketing {
    public class CampaignCustomerRepositoryTests {
        private static ICampaignCustomerRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();

            return diMap.Resolve<ICampaignCustomerRepository>();
        }

        public class GetAllCustomersByCampaign : MigratedDatabaseTest {
            [Fact]
            public void BadCampaignId_ReturnsEmptyList() {
                // arrange
                var repo = MakeRepo();
                var id = 99;
                var expected = 0;

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test.Count
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCampaignId_ReturensExpectedRowCount() {
                // arrange
                var repo = MakeRepo();
                var id = 1;
                var expected = 2;

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test.Count
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCampaignIdRecord1_HasExpectedCampaignId() {
                // arrange
                var repo = MakeRepo();
                var id = 1;
                var expected = 1;

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test[0]
                    .CampaignId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCampaignIdRecord1_HasExpectedBranchId() {
                // arrange
                var repo = MakeRepo();
                var id = 1;
                var expected = "FDF";

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test[0]
                    .BranchId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCampaignIdRecord1_HasExpectedCustomerNumber() {
                // arrange
                var repo = MakeRepo();
                var id = 1;
                var expected = "123456";

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test[0]
                    .CustomerNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCampaignIdRecord2_HasExpectedCampaignId() {
                // arrange
                var repo = MakeRepo();
                var id = 1;
                var expected = 1;

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test[1]
                    .CampaignId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCampaignIdRecord2_HasExpectedBranchId() {
                // arrange
                var repo = MakeRepo();
                var id = 1;
                var expected = "FDF";

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test[1]
                    .BranchId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCampaignIdRecord2_HasExpectedCustomerNumber() {
                // arrange
                var repo = MakeRepo();
                var id = 1;
                var expected = "123457";

                // act
                var test = repo.GetAllCustomersByCampaign(id);

                // assert
                test[1]
                    .CustomerNumber
                    .Should()
                    .Be(expected);
            }
        }
    }
}
