using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class MandatoryItemsListLogicTests : BaseDITests
    {
        private static IMandatoryItemsListLogic MakeTestsObject()
        {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IMandatoryItemsListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IMandatoryItemsListDetailsRepository>();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IMandatoryItemsListLogic>();
        }

        private static IMandatoryItemsListHeadersRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetListHeaderForCustomer(
                                                                 It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                 c.CustomerId == "123456")))
                          .Returns(new MandatoryItemsListHeader() {
                                                                      BranchId = "FUT",
                                                                      CustomerNumber = "123456",
                                                                      Id = 1,
                                                                  });

            return mockHeaderRepo.Object;
        }

        private static IMandatoryItemsListDetailsRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetAllByHeader(It.Is<long>(l => l == 1)))
                           .Returns(new List<MandatoryItemsListDetail>()
                           {
                               new MandatoryItemsListDetail() {
                                                             CatalogId = "FUT",
                                                             ItemNumber = "123456",
                                                             Each = false,
                                                             LineNumber = 1,
                                                             Id = 1
                                                         }
                           });

            return mockDetailsRepo.Object;
        }

        public class GetMandatoryItemNumbers
        {
            [Fact]
            public void BadBranchId_ReturnsEmptyList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var emptylist = 0;

                // act
                var results = testunit.GetMandatoryItemNumbers(testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(emptylist);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var emptylist = 0;

                // act
                var results = testunit.GetMandatoryItemNumbers(testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(emptylist);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var expectedItem = "123456";

                // act
                var results = testunit.GetMandatoryItemNumbers(testcontext);

                // assert
                results.First()
                       .Should()
                       .Be(expectedItem);
            }
        }

        public class GetListModel
        {
            [Fact]
            public void BadBranchId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var fakeId = 0;

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var fakeUser = new UserProfile();
                var fakeId = 0;

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var fakeId = 0;
                var expectedListId = 1;

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class ReadList
        {
            [Fact]
            public void BadBranchId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                var results = testunit.ReadList(testcontext, false);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };

                // act
                var results = testunit.ReadList(testcontext, false);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var expected = 1;

                // act
                var results = testunit.ReadList(testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }
        }

        public class CreateOrUpdateList
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveHeaderEveryTime()
            {
                // arrange
                var mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                var mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                var testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testDetail = new MandatoryItemsListDetail() {
                                                                    CatalogId = "FUT",
                                                                    ItemNumber = "123456",
                                                                    Each = false,
                                                                    LineNumber = 1,
                                                                    Id = 1
                                                                };

                // act
                testunit.SaveDetail(testcontext, testDetail);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.IsAny<MandatoryItemsListDetail>()), Times.Once(), "Error updating");
            }
        }

        public class DeleteMandatoryItems
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsDeleteDetail()
            {
                // arrange
                var mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                var mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                var testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testId = 1;
                var testDetail = new MandatoryItemsListDetail()
                {
                    CatalogId = "FUT",
                    ItemNumber = "123456",
                    Each = false,
                    LineNumber = 1,
                    Id = testId
                };

                // act
                testunit.DeleteMandatoryItems(testDetail);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Delete(testId), Times.Once(), "Error updating");
            }
        }

        public class CreateList
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_Completes()
            {
                // arrange
                var mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                var mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                var testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                // act
                testunit.CreateList(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockHeaderRepo.Verify(h => h.SaveMandatoryItemsHeader(It.IsAny<MandatoryItemsListHeader>()), Times.Once(), "Error updating");
            }
        }
    }
}
