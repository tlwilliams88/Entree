using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class CustomListLogicTests {
        private static ICustomListLogic MakeMockLogic() {
            ContainerBuilder cb = DependencyMapFactory.GetTestsContainer();

            cb.RegisterInstance(MakeDetailRepo())
              .AsImplementedInterfaces();
            cb.RegisterInstance(MakeHeaderRepo())
              .AsImplementedInterfaces();
            cb.RegisterInstance(MakeShareRepo())
              .AsImplementedInterfaces();

            IContainer diMap = cb.Build();

            return diMap.Resolve<ICustomListLogic>();
        }

        private static ICustomListDetailsRepository MakeDetailRepo() {
            var repo = new Mock<ICustomListDetailsRepository>();

            repo.Setup(d => d.GetCustomListDetails(It.Is<long>(i => i == 1)))
                .Returns(
                    new List<CustomListDetail>() { 
                        new CustomListDetail() {
                            Active = true,
                            CatalogId = "FUT",
                            CustomInventoryItemId = 100,
                            Each = false,
                            HeaderId = 1,
                            Id = 1,
                            ItemNumber = "123456",
                            Label = "Fake Label",
                            LineNumber = 1,
                            Par = 12,
                            CreatedUtc = new DateTime(2017, 7, 6, 16, 50, 0, DateTimeKind.Utc),
                            ModifiedUtc = new DateTime(2017, 7, 6, 16, 51, 0, DateTimeKind.Utc)
                        },
                        new CustomListDetail() {
                            Active = true,
                            CatalogId = "FUT",
                            CustomInventoryItemId = 101,
                            Each = false,
                            HeaderId = 1,
                            Id = 2,
                            ItemNumber = "234567",
                            Label = "Fake Label",
                            LineNumber = 2,
                            Par = 10,
                            CreatedUtc = new DateTime(2017, 7, 6, 16, 50, 0, DateTimeKind.Utc),
                            ModifiedUtc = new DateTime(2017, 7, 6, 16, 51, 0, DateTimeKind.Utc)
                        }
                    }
                );

            return repo.Object;
        }

        private static ICustomListHeadersRepository MakeHeaderRepo() {
            var repo = new Mock<ICustomListHeadersRepository>();

            repo.Setup(h => h.GetCustomListHeader(It.Is<long>(i => i == 1)))
                .Returns(new CustomListHeader() {
                    Active = true,
                    BranchId = "FUT",
                    CustomerNumber = "123456",
                    CreatedUtc = new DateTime(2017, 7, 7, 9, 41, 0, DateTimeKind.Utc),
                    Id = 1,
                    ModifiedUtc = new DateTime(2017, 7, 7, 9, 42, 0, DateTimeKind.Utc),
                    Name = "Fake List Name",
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                });

            return repo.Object;
        }

        private static ICustomListSharesRepository MakeShareRepo() {
            var repo = new Mock<ICustomListSharesRepository>();

            repo.Setup(s => s.GetCustomListSharesByHeaderId(It.Is<long>(i => i == 1)))
                .Returns(
                    new List<CustomListShare>(){
                        new CustomListShare() {
                            Active = true,
                            BranchId = "FUT",
                            CreatedUtc = new DateTime(2017, 7, 7, 16, 25, 0, DateTimeKind.Utc),
                            CustomerNumber = "234567",
                            HeaderId = 1,
                            Id = 1,
                            ModifiedUtc = new DateTime(2017, 7, 7, 16, 26, 0, DateTimeKind.Utc)
                        },
                        new CustomListShare() {
                            Active = true,
                            BranchId = "FUT",
                            CreatedUtc = new DateTime(2017, 7, 7, 16, 25, 0, DateTimeKind.Utc),
                            CustomerNumber = "345678",
                            HeaderId = 1,
                            Id = 2,
                            ModifiedUtc = new DateTime(2017, 7, 7, 16, 26, 0, DateTimeKind.Utc)
                        }
                    }
                );

            return repo.Object;
        }

        public class CreateOrUpdateList { }
        public class DeleteList { }
        public class GetListModel { }
        public class ReadList {
            [Fact]
            public void BadHeaderIdWithoutItems_ReturnsNull() {
                // arrange
                var headerId = 0;
                var logic = MakeMockLogic();

                // act
                var results = logic.ReadList(headerId, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedListModel() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, true);
            
                // assert
                result.ListId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedItemCount() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, true);

                // assert
                result.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedListModel() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, false);

                // assert
                result.ListId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedItemCount() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, false);

                // assert
                result.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedShareCount() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, true);

                // assert
                result.SharedWith
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedShareCount() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, false);

                // assert
                result.SharedWith
                      .Count
                      .Should()
                      .Be(expected);
            }
        }

        public class ReadLists { 

            // test to get all lists by customer 

            // test to make sur ehtat we can get lists for customers that were created by different users
        }
        public class SaveItem { }
        public class SaveList { }
    }
}
