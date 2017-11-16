using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Impl.Repository.Marketing;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Marketing {
    public class CatalogCampaignHeaderRepositoryTests {
        private static ICatalogCampaignHeaderRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<ICatalogCampaignHeaderRepository>();
        }

        private static CatalogCampaignHeader MakeSampleHeader() {
            return new CatalogCampaignHeader() {
                Name = "Name2",
                Description = "Description2",
                Uri = "uri-2",
                StartDate = new DateTime(2017, 07, 03, 00, 00, 00, DateTimeKind.Unspecified),
                EndDate = new DateTime(2018, 07, 03, 00, 00, 00, DateTimeKind.Unspecified),
                Active = true
            };
        }

        public class GetHeader : MigratedDatabaseTest {
            [Fact]
            public void GoodHeaderId_ReturnsExpectedId() {
                // arrange
                int expected = 1;
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Id
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedName()
            {
                // arrange
                var expected = "Name1";
                var headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedDescription()
            {
                // arrange
                var expected = "Description1";
                var headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Description
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedUri()
            {
                // arrange
                var expected = "uri-1";
                var headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Uri
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedStartDate()
            {
                // arrange
                var expected = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified);
                var headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.StartDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedEndDate()
            {
                // arrange
                var expected = new DateTime(2018, 7, 3, 16, 9, 0, DateTimeKind.Unspecified);
                var headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.EndDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedActive()
            {
                // arrange
                var expected = true;
                var headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Active
                      .Should()
                      .Be(expected);
        }

            [Fact]
            public void BadHeaderId_ReturnsNothing() {
                // arrange
                var headerId = 5;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Should()
                      .BeNull();
            }
        }

        public class GetHeaderByUri : MigratedDatabaseTest
        {
            [Fact]
            public void GoodUri_ReturnsExpectedId()
            {
                // arrange
                var expected = 1;
                var uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Id
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedName()
            {
                // arrange
                var expected = "Name1";
                var uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedDescription()
            {
                // arrange
                var expected = "Description1";
                var uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Description
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedUri()
            {
                // arrange
                var expected = "uri-1";
                var uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(expected);

                // assert
                header.Uri
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedStartDate()
            {
                // arrange
                var expected = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified);
                var uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.StartDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedEndDate()
            {
                // arrange
                var expected = new DateTime(2018, 7, 3, 16, 9, 0, DateTimeKind.Unspecified);
                var uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.EndDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedActive()
            {
                // arrange
                var expected = true;
                var uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Active
                      .Should()
                      .Be(expected);
            }
        }

    }
}
