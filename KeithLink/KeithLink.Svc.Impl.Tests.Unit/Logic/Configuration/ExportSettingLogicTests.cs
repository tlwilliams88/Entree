using System;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Impl.Logic.Configurations;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Configuration {
    public class ExportSettingLogicTests : BaseDITests {
        #region ReadCustomExportOptions
        public class ReadCustomExportOptions {
            [Fact]
            public void WhenExportTypeCartDetailIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.CartDetail;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeCartDetailIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.CartDetail;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeContractListIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.List;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeContractListIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.List;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeCustomListIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.List;
                ListType testListType = ListType.Custom;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeCustomListIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.List;
                ListType testListType = ListType.Custom;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeFavoriteListIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.List;
                ListType testListType = ListType.Favorite;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeFavoriteListIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.List;
                ListType testListType = ListType.Favorite;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeInvoiceDetailIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.InvoiceDetail;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeInvoiceDetailIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.InvoiceDetail;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeInvoiceIsCalledFor_ResultingFieldsContainDetailFalse() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.Invoice;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeFalse();
            }

            [Fact]
            public void WhenExportTypeInvoiceIsCalledFor_ResultingFieldsContainEachFalse() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.Invoice;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeFalse();
            }

            [Fact]
            public void WhenExportTypeItemUsageIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.ItemUsage;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeItemUsageIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.ItemUsage;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeMarketingPreferencesIsCalledFor_ResultingFieldsContainDetailFalse() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.MarketingPreferences;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeFalse();
            }

            [Fact]
            public void WhenExportTypeMarketingPreferencesIsCalledFor_ResultingFieldsContainEachFalse() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.MarketingPreferences;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeFalse();
            }

            [Fact]
            public void WhenExportTypeOrderDetailIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.OrderDetail;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeOrderDetailIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.OrderDetail;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypePendingTransactionsIsCalledFor_ResultingFieldsContainDetailFalse() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.PendingTransactions;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeFalse();
            }

            [Fact]
            public void WhenExportTypePendingTransactionsIsCalledFor_ResultingFieldsContainEachFalse() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.PendingTransactions;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeFalse();
            }

            [Fact]
            public void WhenExportTypeProductsIsCalledFor_ResultingFieldsContainDetailTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.Products;
                ListType testListType = ListType.Contract;
                string expected = "Detail";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }

            [Fact]
            public void WhenExportTypeProductsIsCalledFor_ResultingFieldsContainEachTrue() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Guid testGuid = new Guid();
                ExportType testExportType = ExportType.Products;
                ListType testListType = ListType.Contract;
                string expected = "Each";

                // act
                ExportOptionsModel results = testunit.ReadCustomExportOptions(testGuid, testExportType, testListType);

                // assert
                results.Fields
                       .Select(f => f.Field)
                       .ToList()
                       .Contains(expected)
                       .Should()
                       .BeTrue();
            }
        }
        #endregion ReadCustomExportOptions

        #region Setup
        public class MockDependents {
            public Mock<IExportSettingRepository> ExportSettingRepository { get; set; }

            public Mock<IExternalCatalogRepository> ExternalCatalogRepository { get; set; }

            public Mock<IUnitOfWork> UnitOfWork { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeIExportSettingRepository()
                                            .Object)
                  .As<IExportSettingRepository>();
                cb.RegisterInstance(MakeIExternalCatalogRepository()
                                            .Object)
                  .As<IExternalCatalogRepository>();
                cb.RegisterInstance(MakeIUnitOfWork()
                                            .Object)
                  .As<IUnitOfWork>();
            }

            public static Mock<IExportSettingRepository> MakeIExportSettingRepository() {
                Mock<IExportSettingRepository> mock = new Mock<IExportSettingRepository>();

                return mock;
            }

            public static Mock<IExternalCatalogRepository> MakeIExternalCatalogRepository() {
                Mock<IExternalCatalogRepository> mock = new Mock<IExternalCatalogRepository>();

                return mock;
            }

            public static Mock<IUnitOfWork> MakeIUnitOfWork() {
                Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();

                return mock;
            }
        }

        private static IExportSettingLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IExportSettingLogic>();
            }
            mockDependents = new MockDependents();
            mockDependents.ExportSettingRepository = MockDependents.MakeIExportSettingRepository();
            mockDependents.ExternalCatalogRepository = MockDependents.MakeIExternalCatalogRepository();
            mockDependents.UnitOfWork = MockDependents.MakeIUnitOfWork();

            ExportSettingLogicImpl testunit = new ExportSettingLogicImpl(mockDependents.UnitOfWork.Object, mockDependents.ExportSettingRepository.Object, mockDependents.ExternalCatalogRepository.Object);
            return testunit;
        }
        #endregion Setup
    }
}