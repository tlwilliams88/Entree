using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Reports;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Reports {
    public class ReportLogicTests : BaseDITests {
        #region attributes
        protected static ItemUsageReportQueryModel MakeQuery() {
            return new ItemUsageReportQueryModel {
                fromDate = DateTime.Parse("1/1/1970"),
                toDate = DateTime.Parse("1/10/1970"),
                sortField = "ItemNumber",
                sortDir = "asc",
                UserSelectedContext = new UserSelectedContext {
                    CustomerId = "123456",
                    BranchId = "FUT"
                }
            };
        }
        #endregion attributes

        #region GetItemUsage
        public class GetItemUsage {
            [Fact]
            public void WhenGoodBranchAndGoodInvoiceYieldGoodOrderWithItemInCatalog_ResultingDetailIsExpected() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IReportLogic testunit = MakeTestsLogic(false, ref mockDependents);

                mockDependents.ICatalogLogic.Setup(m => m.GetProductsByIds("FUT", new List<string> {"111111"}))
                              .Returns(new ProductsReturn {
                                  Products = new List<Product> {
                                      new Product {
                                          ItemNumber = "111111",
                                          Name = "Fake Name",
                                          BrandExtendedDescription = "Fake Brand",
                                          ItemClass = "Fake Class",
                                          Size = "Fake Size",
                                          Pack = "2"
                                      }
                                  }
                              });

                ItemUsageReportQueryModel testQuery = MakeQuery();
                string expected = "Fake Name / 111111 / Fake Brand / Fake Class / 2 / Fake Size";

                // act
                List<ItemUsageReportItemModel> result = testunit.GetItemUsage(testQuery);

                // assert
                result.First()
                      .Detail
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void WhenGoodBranchAndGoodInvoiceYieldGoodOrderWithItemInCatalog_ResultingDetailIsNull() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IReportLogic testunit = MakeTestsLogic(false, ref mockDependents);

                mockDependents.ICatalogLogic.Setup(m => m.GetProductsByIds("FUT", new List<string> {"111111"}))
                              .Returns(new ProductsReturn {
                                  Products = new List<Product>()
                              });

                ItemUsageReportQueryModel testQuery = MakeQuery();

                // act
                List<ItemUsageReportItemModel> result = testunit.GetItemUsage(testQuery);

                // assert
                result.First()
                      .Detail
                      .Should()
                      .BeNull();
            }
        }
        #endregion GetItemUsage

        #region Setup
        public class MockDependents {
            public Mock<ICatalogRepository> ICatalogRepository { get; set; }

            public Mock<IReportRepository> IReportRepository { get; set; }

            public Mock<IEventLogRepository> IEventLogRepository { get; set; }

            public Mock<ICatalogLogic> ICatalogLogic { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeIEventLogRepository()
                                            .Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeICatalogLogic()
                                            .Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeICatalogRepository()
                                            .Object)
                  .As<ICatalogRepository>();
                cb.RegisterInstance(MakeIReportRepository()
                                            .Object)
                  .As<IReportRepository>();
            }

            public static Mock<IReportRepository> MakeIReportRepository() {
                Mock<IReportRepository> mock = new Mock<IReportRepository>();

                mock.Setup(m => m.GetOrderLinesForItemUsageReport("FUT", "123456", It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new List<OrderLine> {
                        new OrderLine {
                            ItemNumber = "111111"
                        }
                    });

                return mock;
            }

            public static Mock<ICatalogRepository> MakeICatalogRepository() {
                Mock<ICatalogRepository> mock = new Mock<ICatalogRepository>();

                return mock;
            }

            public static Mock<ICatalogLogic> MakeICatalogLogic() {
                Mock<ICatalogLogic> mock = new Mock<ICatalogLogic>();

                return mock;
            }

            public static Mock<IEventLogRepository> MakeIEventLogRepository() {
                Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

                return mock;
            }
        }

        private static IReportLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IReportLogic>();
            }
            mockDependents = new MockDependents();
            mockDependents.ICatalogLogic = MockDependents.MakeICatalogLogic();
            mockDependents.ICatalogRepository = MockDependents.MakeICatalogRepository();
            mockDependents.IReportRepository = MockDependents.MakeIReportRepository();
            mockDependents.IEventLogRepository = MockDependents.MakeIEventLogRepository();

            ReportLogic testunit = new ReportLogic(mockDependents.ICatalogRepository.Object, mockDependents.IReportRepository.Object, mockDependents.IEventLogRepository.Object,
                                                   mockDependents.ICatalogLogic.Object);
            return testunit;
        }
        #endregion Setup
    }
}