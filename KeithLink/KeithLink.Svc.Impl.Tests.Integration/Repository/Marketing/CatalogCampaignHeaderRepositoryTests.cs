using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;
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
            return new CatalogCampaignHeader {
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
            public void BadHeaderId_ReturnsNothing() {
                // arrange
                int headerId = 5;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Should()
                      .BeNull();
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedActive() {
                // arrange
                bool expected = true;
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedDescription() {
                // arrange
                string expected = "Description1";
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Description
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedEndDate() {
                // arrange
                DateTime expected = new DateTime(2018, 7, 3, 16, 9, 0, DateTimeKind.Unspecified);
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.EndDate
                      .Should()
                      .Be(expected);
            }

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
            public void GoodHeaderId_ReturnsExpectedName() {
                // arrange
                string expected = "Name1";
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedStartDate() {
                // arrange
                DateTime expected = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified);
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.StartDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedUri() {
                // arrange
                string expected = "uri-1";
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetHeader(headerId);

                // assert
                header.Uri
                      .Should()
                      .Be(expected);
            }
        }

        public class GetHeaderByUri : MigratedDatabaseTest {
            [Fact]
            public void GoodUri_ReturnsExpectedActive() {
                // arrange
                bool expected = true;
                string uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedDescription() {
                // arrange
                string expected = "Description1";
                string uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Description
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedEndDate() {
                // arrange
                DateTime expected = new DateTime(2018, 7, 3, 16, 9, 0, DateTimeKind.Unspecified);
                string uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.EndDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedId() {
                // arrange
                int expected = 1;
                string uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Id
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedName() {
                // arrange
                string expected = "Name1";
                string uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedStartDate() {
                // arrange
                DateTime expected = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified);
                string uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(uri);

                // assert
                header.StartDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodUri_ReturnsExpectedUri() {
                // arrange
                string expected = "uri-1";
                string uri = "uri-1";
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                CatalogCampaignHeader header = repo.GetByUri(expected);

                // assert
                header.Uri
                      .Should()
                      .Be(expected);
            }
        }

        public class GetAll : MigratedDatabaseTest {
            [Fact]
            public void GoodHeader_ReturnsExpectedActive() {
                // arrange
                bool expected = true;
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.First(x => x.Id.Equals(headerId))
                      .Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCount() {
                // arrange
                int expected = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.Count()
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedDescription() {
                // arrange
                string expected = "Description1";
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.First(x => x.Id.Equals(headerId))
                      .Description
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedEndDate() {
                // arrange
                DateTime expected = new DateTime(2018, 7, 3, 16, 9, 0, DateTimeKind.Unspecified);
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.First(x => x.Id.Equals(headerId))
                      .EndDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedId() {
                // arrange
                int expected = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.First()
                      .Id
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                string expected = "Name1";
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.First(x => x.Id.Equals(headerId))
                      .Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedStartDate() {
                // arrange
                DateTime expected = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified);
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.First(x => x.Id.Equals(headerId))
                      .StartDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedUri() {
                // arrange
                string expected = "uri-1";
                int headerId = 1;
                ICatalogCampaignHeaderRepository repo = MakeRepo();

                // act
                List<CatalogCampaignHeader> header = repo.GetAll();

                // assert
                header.First(x => x.Id.Equals(headerId))
                      .Uri
                      .Should()
                      .Be(expected);
            }
        }
    }
}