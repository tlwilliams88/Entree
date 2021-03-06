﻿using System;
using System.Collections.Generic;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class ContractListLogicTests {
        private static IContractListLogic MakeMockLogic() {
            ContainerBuilder cb = DependencyMapFactory.GetTestsContainer();

            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IContractListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailRepo())
              .As<IContractListDetailsRepository>();

            IContainer diMap = cb.Build();

            return diMap.Resolve<IContractListLogic>();
        }

        private static IContractListDetailsRepository MakeMockDetailRepo() {
            Mock<IContractListDetailsRepository> mockRepo = new Mock<IContractListDetailsRepository>();

            mockRepo.Setup(d => d.GetContractListDetails(It.Is<long>(i => i == 1)))
                    .Returns(
                             new List<ContractListDetail> {
                                 new ContractListDetail {
                                     CatalogId = "FUT",
                                     Category = "Fake Category",
                                     CreatedUtc = new DateTime(2017, 7, 5, 15, 19, 0, DateTimeKind.Utc),
                                     Each = false,
                                     FromDate = new DateTime(2017, 7, 1),
                                     HeaderId = 1,
                                     Id = 1,
                                     ItemNumber = "123456",
                                     LineNumber = 1,
                                     ModifiedUtc = new DateTime(2017, 7, 5, 15, 20, 0, DateTimeKind.Utc),
                                     ToDate = new DateTime(2017, 7, 30)
                                 },
                                 new ContractListDetail {
                                     CatalogId = "FUT",
                                     Category = "Fake Category",
                                     CreatedUtc = new DateTime(2017, 7, 5, 15, 19, 0, DateTimeKind.Utc),
                                     Each = false,
                                     FromDate = new DateTime(2017, 7, 1),
                                     HeaderId = 1,
                                     Id = 2,
                                     ItemNumber = "234567",
                                     LineNumber = 2,
                                     ModifiedUtc = new DateTime(2017, 7, 5, 15, 20, 0, DateTimeKind.Utc),
                                     ToDate = new DateTime(2017, 7, 30)
                                 }
                             });

            return mockRepo.Object;
        }

        private static IContractListHeadersRepository MakeMockHeaderRepo() {
            Mock<IContractListHeadersRepository> mockHeaderRepo = new Mock<IContractListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetListHeaderForCustomer(It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                 c.CustomerId == "123456")))
                          .Returns(new ContractListHeader {
                              BranchId = "FIT",
                              CustomerNumber = "123456",
                              ContractId = "ABC12345",
                              CreatedUtc = new DateTime(2017, 7, 5, 15, 13, 0, DateTimeKind.Utc),
                              Id = 1,
                              ModifiedUtc = new DateTime(2017, 7, 5, 15, 14, 0, DateTimeKind.Utc)
                          });

            mockHeaderRepo.Setup(h => h.GetListHeaderForCustomer(It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                 c.CustomerId == "234567")))
                          .Returns(new ContractListHeader {
                              BranchId = "FIT",
                              CustomerNumber = "123456",
                              ContractId = "ABC12345",
                              CreatedUtc = new DateTime(2017, 7, 5, 15, 13, 0, DateTimeKind.Utc),
                              Id = 2,
                              ModifiedUtc = new DateTime(2017, 7, 5, 15, 14, 0, DateTimeKind.Utc)
                          });

            return mockHeaderRepo.Object;
        }

        public class GetListModel {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int fakeId = 1;

                // act
                ListModel results = logic.GetListModel(fakeUser, test, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile fakeUser = new UserProfile();
                int fakeId = 1;

                // act
                ListModel results = logic.GetListModel(fakeUser, test, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void ContractWithNoItems_ReturnsZeroLengthItemList() {
                // arrange
                int expected = 0;
                UserProfile fakeUser = new UserProfile();
                int fakeId = 1;
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                ListModel results = logic.GetListModel(fakeUser, test, fakeId);

                // assert
                results
                        .Items
                        .Count
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedHeaderId() {
                // arrange
                int expected = 1;
                UserProfile fakeUser = new UserProfile();
                int fakeId = 1;
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                ListModel results = logic.GetListModel(fakeUser, test, fakeId);

                // assert
                results
                        .ListId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItemCount() {
                // arrange
                int expected = 2;
                UserProfile fakeUser = new UserProfile();
                int fakeId = 1;
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                ListModel results = logic.GetListModel(fakeUser, test, fakeId);

                // assert
                results
                        .Items
                        .Count
                        .Should()
                        .Be(expected);
            }
        }

        public class ReadList {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                bool headerOnly = false;

                // act
                ListModel results = logic.ReadList(fakeUser, test, headerOnly);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile fakeUser = new UserProfile();
                bool headerOnly = false;

                // act
                ListModel results = logic.ReadList(fakeUser, test, headerOnly);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void ContractWithNoItems_ReturnsZeroLengthItemList() {
                // arrange
                int expected = 0;
                UserProfile fakeUser = new UserProfile();
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                bool headerOnly = false;

                // act
                ListModel results = logic.ReadList(fakeUser, test, headerOnly);

                // assert
                results
                        .Items
                        .Count
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedHeaderId() {
                // arrange
                int expected = 1;
                UserProfile fakeUser = new UserProfile();
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                bool headerOnly = false;

                // act
                ListModel results = logic.ReadList(fakeUser, test, headerOnly);

                // assert
                results
                        .ListId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItemCount() {
                // arrange
                int expected = 2;
                UserProfile fakeUser = new UserProfile();
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                bool headerOnly = false;

                // act
                ListModel results = logic.ReadList(fakeUser, test, headerOnly);

                // assert
                results
                        .Items
                        .Count
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodCustomerHeaderOnlyFalse_ReturnsExpectedItemCount() {
                // arrange
                int expected = 2;
                UserProfile fakeUser = new UserProfile();
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                bool headerOnly = false;

                // act
                ListModel results = logic.ReadList(fakeUser, test, headerOnly);

                // assert
                results
                        .Items
                        .Count
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodCustomerHeaderOnlyTrue_ReturnsExpectedItemCount() {
                // arrange
                int expected = 0;
                UserProfile fakeUser = new UserProfile();
                IContractListLogic logic = MakeMockLogic();
                UserSelectedContext test = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                bool headerOnly = true;

                // act
                ListModel results = logic.ReadList(fakeUser, test, headerOnly);

                // assert
                results
                        .Items
                        .Count
                        .Should()
                        .Be(expected);
            }
        }
    }
}