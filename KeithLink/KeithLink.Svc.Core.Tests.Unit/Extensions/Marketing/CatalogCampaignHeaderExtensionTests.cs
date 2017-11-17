using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using KeithLink.Svc.Core.Extensions.Marketing;
using KeithLink.Svc.Core.Models.Marketing;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Marketing {
    public class CatalogCampaignHeaderExtensionTests {
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
                HasFilter = true
            };
        }

        public class ToModel {
            public class Active {
                [Fact]
                public void GoodHeader_HasExpectedValue() {
                    // arrange
                    var fakeHeader = MakeHeader();
                    var expected = true;

                    // act
                    var test = fakeHeader.ToModel();

                    // assert
                    test.Active
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();
                    var expected = false;

                    // act
                    var test = fakeHeader.ToModel();

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
                    var test = fakeHeader.ToModel();

                    // assert
                    test.Description
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();

                    // act
                    var test = fakeHeader.ToModel();

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
                    var test = fakeHeader.ToModel();

                    // assert
                    test.EndDate
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();
                    var expected = new DateTime();

                    // act
                    var test = fakeHeader.ToModel();

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
                    var fakeHeader = MakeHeader();
                    var expected = true;

                    // act
                    var test = fakeHeader.ToModel();

                    // assert
                    test.HasFilter
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();
                    var expected = false;

                    // act
                    var test = fakeHeader.ToModel();

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
                    var test = fakeHeader.ToModel();

                    // assert
                    test.Id
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();
                    var expected = 0;

                    // act
                    var test = fakeHeader.ToModel();

                    // assert
                    test.Id
                        .Should()
                        .Be(expected);
                }
            }

            public class Name {
                [Fact]
                public void GoodHeader_HasExpectedValue() {
                    // arrange
                    var fakeHeader = MakeHeader();
                    var expected = "Fake Name";

                    // act
                    var test = fakeHeader.ToModel();

                    // assert
                    test.Name
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();

                    // act
                    var test = fakeHeader.ToModel();

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
                    var test = fakeHeader.ToModel();

                    // assert
                    test.StartDate
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();
                    var expected = new DateTime();

                    // act
                    var test = fakeHeader.ToModel();

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
                    var test = fakeHeader.ToModel();

                    // assert
                    test.Uri
                        .Should()
                        .Be(expected);
                }

                [Fact]
                public void InitializedHeader_HasDefaultValue() {
                    // arrange
                    var fakeHeader = new CatalogCampaignHeader();

                    // act
                    var test = fakeHeader.ToModel();

                    // assert
                    test.Uri
                        .Should()
                        .BeNull();
                }
            }
        }
    }
}
