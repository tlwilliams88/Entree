using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Marketing;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Marketing {
    public class CatalogCampaignHeaderTests {
        private static CatalogCampaignHeader MakeHeader() {
            return new CatalogCampaignHeader() {
                Active = true,
                Description = "Fake Description",
                EndDate = new DateTime(2017, 11, 16, 12, 3, 0, DateTimeKind.Local),
                Id = 17,
                Items = new List<CatalogCampaignItem>() {
                    new CatalogCampaignItem() { Id = 5 },
                    new CatalogCampaignItem() { Id = 25 }
                },
                Name = "Fake Name",
                StartDate = new DateTime(2017, 11, 16, 12, 5, 0, DateTimeKind.Local),
                Uri = "Fake Uri",
                HasFilter = true,
                LinkToUrl = "http://shazbot.org",
            };
        }

        public class Active {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = true;

                // act

                // assert
                fakeHeader.Active
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();
                var expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Description {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = "Fake Description";

                // act

                // assert
                fakeHeader.Description
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();

                // act

                // assert
                test.Description
                    .Should()
                    .BeNull();
            }
        }

        public class EndDate {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = new DateTime(2017, 11, 16, 12, 3, 0, DateTimeKind.Local);

                // act

                // assert
                fakeHeader.EndDate
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();
                var expected = new DateTime();

                // act

                // assert
                test.EndDate
                    .Should()
                    .Be(expected);
            }
        }

        public class HasFilter {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var header = MakeHeader();
                var expected = true;

                // act

                // assert
                header.HasFilter
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();
                var expected = false;

                // act

                // assert
                test.HasFilter
                    .Should()
                    .Be(expected);
            }
        }

        public class Id {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = 17;

                // act

                // assert
                fakeHeader.Id
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();
                var expected = 0;

                // act

                // assert
                test.Id
                    .Should()
                    .Be(expected);
            }
        }

        public class Items {
            [Fact]
            public void GoodHeader_HasExpectedCount() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = 2;

                // act

                // assert
                fakeHeader.Items
                          .Count
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodHeaderRecord1_HasExpectedId() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = 5;

                // act

                // assert
                fakeHeader.Items[0]
                          .Id
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodHeaderRecord2_HasExpectedId() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = 25;

                // act

                // assert
                fakeHeader.Items[1]
                          .Id
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();

                // act

                // assert
                test.Items
                    .Should()
                    .BeNull();
            }
        }

        public class LinkToUrl
        {
            [Fact]
            public void GoodHeader_HasExpectedValue()
            {
                // arrange
                var header = MakeHeader();
                var expected = "http://shazbot.org";

                // act

                // assert
                header.LinkToUrl
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue()
            {
                // arrange
                var test = new CatalogCampaignHeader();

                // act

                // assert
                test.LinkToUrl
                    .Should()
                    .BeNull();
            }
        }


        public class Name {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = "Fake Name";

                // act

                // assert
                fakeHeader.Name
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();

                // act

                // assert
                test.Name
                    .Should()
                    .BeNull();
            }
        }

        public class StartDate {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = new DateTime(2017, 11, 16, 12, 5, 0, DateTimeKind.Local);

                // act

                // assert
                fakeHeader.StartDate
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();
                var expected = new DateTime();

                // act

                // assert
                test.StartDate
                    .Should()
                    .Be(expected);
            }
        }

        public class Uri {
            [Fact]
            public void GoodHeader_HasExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = "Fake Uri";

                // act

                // assert
                fakeHeader.Uri
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignHeader();

                // act

                // assert
                test.Uri
                    .Should()
                    .BeNull();
            }
        }

    }
}
