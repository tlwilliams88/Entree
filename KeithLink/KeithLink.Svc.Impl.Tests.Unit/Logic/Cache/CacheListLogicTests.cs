using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Cache;

using Moq;
using Xunit;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Cache
{
    public class CacheListLogicTests : BaseDITests
    {
        #region Setup
        public class MockDependents
        {
            public Mock<ICacheRepository> CacheRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
                cb.RegisterInstance(MakeICacheRepository().Object)
                  .As<ICacheRepository>();
            }

            public static Mock<ICacheRepository> MakeICacheRepository()
            {
                var mock = new Mock<ICacheRepository>();

                return mock;
            }

        }

        private static ICacheListLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                var testcontainer = cb.Build();

                return testcontainer.Resolve<ICacheListLogic>();
            }
            else
            {
                mockDependents = new MockDependents();
                mockDependents.CacheRepository = MockDependents.MakeICacheRepository();

                var testunit = new CacheListLogicImpl(mockDependents.CacheRepository.Object);
                return testunit;
            }
        }
        #endregion

        #region attributes
        #endregion

        #region GetCachedContractInformation
        public class GetCachedContractInformation
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryGetItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                var results = testunit.GetCachedContractInformation(testContext);

                // assert
                mockDependents.CacheRepository.Verify(m => m.GetItem<Dictionary<string, string>>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called");
            }

        }
        #endregion

        #region GetCachedLabels
        public class GetCachedLabels
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryGetItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                var results = testunit.GetCachedLabels(testContext);

                // assert
                mockDependents.CacheRepository.Verify(m => m.GetItem<List<string>>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called");
            }

        }
        #endregion

        #region AddCachedLabels
        public class AddCachedLabels
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryAddItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var lists = new List<string>();

                // act
                testunit.AddCachedLabels(testContext, lists);

                // assert
                mockDependents.CacheRepository.Verify(m => m.AddItem<List<string>>(It.IsAny<string>(),
                                                                                      It.IsAny<string>(),
                                                                                      It.IsAny<string>(),
                                                                                      It.IsAny<string>(),
                                                                                      It.IsAny<TimeSpan>(),
                                                                                      It.IsAny<List<string>>()), Times.Once, "not called");
            }

        }
        #endregion

        #region GetCachedTypedLists
        public class GetCachedTypedLists
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryGetItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testtype = ListType.Contract;

                // act
                var results = testunit.GetCachedTypedLists(testContext, testtype);

                // assert
                mockDependents.CacheRepository.Verify(m => m.GetItem<List<ListModel>>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called");
            }

        }
        #endregion

        #region AddCachedTypedLists
        public class AddCachedTypedLists
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryAddItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testtype = ListType.Contract;
                var lists = new List<ListModel>();

                // act
                testunit.AddCachedTypedLists(testContext, testtype, lists);

                // assert
                mockDependents.CacheRepository.Verify(m => m.AddItem<List<ListModel>>(It.IsAny<string>(), 
                                                                                      It.IsAny<string>(), 
                                                                                      It.IsAny<string>(), 
                                                                                      It.IsAny<string>(),
                                                                                      It.IsAny<TimeSpan>(), 
                                                                                      It.IsAny<List<ListModel>>()), Times.Once, "not called");
            }

        }
        #endregion

        #region GetCachedCustomerLists
        public class GetCachedCustomerLists
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryGetItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                var results = testunit.GetCachedCustomerLists(testContext);

                // assert
                mockDependents.CacheRepository.Verify(m => m.GetItem<List<ListModel>>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called");
            }

        }
        #endregion

        #region AddCachedCustomerLists
        public class AddCachedCustomerLists
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryAddItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var lists = new List<ListModel>();

                // act
                testunit.AddCachedCustomerLists(testContext, lists);

                // assert
                mockDependents.CacheRepository.Verify(m => m.AddItem<List<ListModel>>(It.IsAny<string>(),
                                                                                      It.IsAny<string>(),
                                                                                      It.IsAny<string>(),
                                                                                      It.IsAny<string>(),
                                                                                      It.IsAny<TimeSpan>(),
                                                                                      It.IsAny<List<ListModel>>()), Times.Once, "not called");
            }

        }
        #endregion

        #region GetCachedSpecificList
        public class GetCachedSpecificList
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryGetItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testtype = ListType.Contract;

                // act
                var results = testunit.GetCachedSpecificList(testContext, testtype, 4);

                // assert
                mockDependents.CacheRepository.Verify(m => m.GetItem<ListModel>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called");
            }

        }
        #endregion

        #region AddCachedSpecificList
        public class AddCachedSpecificList
        {
            [Fact]
            public void AnyCall_CallsCacheRepositoryAddItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testtype = ListType.Contract;
                var list = new ListModel();

                // act
                testunit.AddCachedSpecificList(testContext, testtype, 5, list);

                // assert
                mockDependents.CacheRepository.Verify(m => m.AddItem<ListModel>(It.IsAny<string>(),
                                                                                It.IsAny<string>(),
                                                                                It.IsAny<string>(),
                                                                                It.IsAny<string>(),
                                                                                It.IsAny<TimeSpan>(),
                                                                                It.IsAny<ListModel>()), Times.Once, "not called");
            }

        }
        #endregion

        #region ClearCustomersListCaches
        public class ClearCustomersListCaches
        {
            [Fact]
            public void CallWith2ListsInCollection_CallsCacheRepositoryRemoveItem6Times()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var fakeUser = new UserProfile();
                var testLists = new List<ListModel> {
                    new ListModel() {
                                        BranchId="FUT",
                                        CustomerNumber = "123456",
                                        Type = ListType.Contract,
                                        ListId = 5
                                    },
                    new ListModel() {
                                        BranchId="FUT",
                                        CustomerNumber = "123456",
                                        Type = ListType.Favorite,
                                        ListId = 5
                                    }
                    };

                // act
                testunit.ClearCustomersListCaches(fakeUser, testContext, testLists);

                // assert
                mockDependents.CacheRepository.Verify(m => m.RemoveItem(It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<string>()), Times.Exactly(6), "not called");
            }

        }
        #endregion

    }
}
