using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Service.SiteCatalog;
using UserProfile = KeithLink.Svc.Core.Models.Profile.UserProfile;

namespace KeithLink.Svc.Impl.Tests.Unit.Service.SiteCatalog
{
    public class SiteCatalogServiceTests : BaseDITests
    {
        #region Setup
        public class MockDependents
        {
            public Mock<ICatalogLogic> ICatalogLogic { get; set; }

            public Mock<ICatalogRepository> ICatalogRepository { get; set; }

            public Mock<IPriceLogic> IPriceLogic { get; set; }

            public Mock<IHistoryListLogic> IHistoryListLogic { get; set; }

            public Mock<ICategoryImageRepository> ICategoryImageRepository { get; set; }

            public Mock<IRecommendedItemsRepository> IRecommendedItemsRepository { get; set; }

            public Mock<IGrowthAndRecoveriesRepository> IGrowthAndRecoveriesRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
                cb.RegisterInstance(MakeMockICatalogLogic().Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeMockICatalogRepository().Object)
                  .As<ICatalogRepository>();
                cb.RegisterInstance(MakeMockIPriceLogic().Object)
                  .As<IPriceLogic>();
                cb.RegisterInstance(MakeMockIHistoryListLogic().Object)
                  .As<IHistoryListLogic>();
                cb.RegisterInstance(MakeMockICategoryImageRepository().Object)
                  .As<ICategoryImageRepository>();
                cb.RegisterInstance(MakeMockIRecommendedItemsRepository().Object)
                  .As<IRecommendedItemsRepository>();
                cb.RegisterInstance(MakeMockIGrowthAndRecoveriesRepository().Object)
                  .As<IGrowthAndRecoveriesRepository>();
            }

            public static Mock<ICatalogLogic> MakeMockICatalogLogic()
            {
                Mock<ICatalogLogic> mock = new Mock<ICatalogLogic>();

                return mock;
            }

            public static Mock<ICatalogRepository> MakeMockICatalogRepository()
            {
                Mock<ICatalogRepository> mock = new Mock<ICatalogRepository>();

                return mock;
            }

            public static Mock<IPriceLogic> MakeMockIPriceLogic()
            {
                Mock<IPriceLogic> mock = new Mock<IPriceLogic>();

                return mock;
            }

            public static Mock<IHistoryListLogic> MakeMockIHistoryListLogic()
            {
                Mock<IHistoryListLogic> mock = new Mock<IHistoryListLogic>();

                return mock;
            }

            public static Mock<ICategoryImageRepository> MakeMockICategoryImageRepository()
            {
                Mock<ICategoryImageRepository> mock = new Mock<ICategoryImageRepository>();

                return mock;
            }

            public static Mock<IRecommendedItemsRepository> MakeMockIRecommendedItemsRepository()
            {
                Mock<IRecommendedItemsRepository> mock = new Mock<IRecommendedItemsRepository>();

                return mock;
            }

            public static Mock<IGrowthAndRecoveriesRepository> MakeMockIGrowthAndRecoveriesRepository()
            {
                Mock<IGrowthAndRecoveriesRepository> mock = new Mock<IGrowthAndRecoveriesRepository>();

                mock.Setup(f => f.GetGrowthAdnGetGrowthAndRecoveryOpportunities(It.Is<string>(i => i == "123456"),
                                                                                It.Is<string>(i => i == "FUT")))
                    .Returns(new List<GrowthAndRecoveriesModel>
                    {
                        new GrowthAndRecoveriesModel()
                        {
                            BranchId = "FUT",
                            Amount = 12,
                            GroupingCode = "XXX",
                            GroupingDescription = "XXX",
                            GrowthAndReccoveryTypeKey = GrowthAndRecoveriesModel.GrowthAndRecoveryType.Growth,
                            GrowthAndRecoveryProductGroup = 1
                        },
                        new GrowthAndRecoveriesModel()
                        {
                            BranchId = "FUT",
                            Amount = 12,
                            GroupingCode = "XXX",
                            GroupingDescription = "XXX",
                            GrowthAndReccoveryTypeKey = GrowthAndRecoveriesModel.GrowthAndRecoveryType.Growth,
                            GrowthAndRecoveryProductGroup = 2
                        },
                        new GrowthAndRecoveriesModel()
                        {
                            BranchId = "FUT",
                            Amount = 12,
                            GroupingCode = "XXX",
                            GroupingDescription = "XXX",
                            GrowthAndReccoveryTypeKey = GrowthAndRecoveriesModel.GrowthAndRecoveryType.Growth,
                            GrowthAndRecoveryProductGroup = 3
                        },
                        new GrowthAndRecoveriesModel()
                        {
                            BranchId = "FUT",
                            Amount = 12,
                            GroupingCode = "XXX",
                            GroupingDescription = "XXX",
                            GrowthAndReccoveryTypeKey = GrowthAndRecoveriesModel.GrowthAndRecoveryType.Growth,
                            GrowthAndRecoveryProductGroup = 4
                        },
                        new GrowthAndRecoveriesModel()
                        {
                            BranchId = "FUT",
                            Amount = 12,
                            GroupingCode = "XXX",
                            GroupingDescription = "XXX",
                            GrowthAndReccoveryTypeKey = GrowthAndRecoveriesModel.GrowthAndRecoveryType.Growth,
                            GrowthAndRecoveryProductGroup = 5
                        },
                        new GrowthAndRecoveriesModel()
                        {
                            BranchId = "FUT",
                            Amount = 12,
                            GroupingCode = "XXX",
                            GroupingDescription = "XXX",
                            GrowthAndReccoveryTypeKey = GrowthAndRecoveriesModel.GrowthAndRecoveryType.Growth,
                            GrowthAndRecoveryProductGroup = 6
                        }
                    });

                return mock;
            }
        }

        private static ISiteCatalogService MakeTestsService(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<ISiteCatalogService>();
            }
            mockDependents = new MockDependents();
            mockDependents.ICatalogLogic = MockDependents.MakeMockICatalogLogic();
            mockDependents.ICatalogRepository = MockDependents.MakeMockICatalogRepository();
            mockDependents.IPriceLogic = MockDependents.MakeMockIPriceLogic();
            mockDependents.IHistoryListLogic = MockDependents.MakeMockIHistoryListLogic();
            mockDependents.ICategoryImageRepository = MockDependents.MakeMockICategoryImageRepository();
            mockDependents.IRecommendedItemsRepository = MockDependents.MakeMockIRecommendedItemsRepository();
            mockDependents.IGrowthAndRecoveriesRepository = MockDependents.MakeMockIGrowthAndRecoveriesRepository();

            ISiteCatalogService testunit = new SiteCatalogServiceImpl(mockDependents.ICatalogLogic.Object, mockDependents.ICatalogRepository.Object,
                                                                      mockDependents.IPriceLogic.Object, mockDependents.IHistoryListLogic.Object,
                                                                      mockDependents.ICategoryImageRepository.Object, 
                                                                      mockDependents.IRecommendedItemsRepository.Object,
                                                                      mockDependents.IGrowthAndRecoveriesRepository.Object);
            return testunit;
        }
        #endregion

        public class GetGrowthAndRecoveryItemsForCustomer
        {
            [Fact]
            public void BadBranchId_ReturnsEmptyList()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                ISiteCatalogService testunit = MakeTestsService(false, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeProfile = new UserProfile();
                int testPageSize = 20;
                bool testImages = true;
                // act
                var results = testunit.GetGrowthAndRecoveryItemsForCustomer(testcontext, fakeProfile, testPageSize, testImages);

                // assert
            }

        }

    }
}