﻿using System;
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

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Configuration
{
    public class ExportSettingLogicTests : BaseDITests
    {
        #region ReadCustomExportOptions

        public class ReadCustomExportOptions
        {
            [Fact]
            public void WhenExportTypeCartDetailIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.CartDetail;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeCartDetailIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.CartDetail;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypeContractListIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.List;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeContractListIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.List;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypeCustomListIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.List;
                var testListType = ListType.Custom;
                var expected = "Detail";

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
            public void WhenExportTypeCustomListIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.List;
                var testListType = ListType.Custom;
                var expected = "Each";

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
            public void WhenExportTypeFavoriteListIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.List;
                var testListType = ListType.Favorite;
                var expected = "Detail";

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
            public void WhenExportTypeFavoriteListIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.List;
                var testListType = ListType.Favorite;
                var expected = "Each";

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
            public void WhenExportTypeInvoiceDetailIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.InvoiceDetail;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeInvoiceDetailIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.InvoiceDetail;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypeInvoiceIsCalledFor_ResultingFieldsContainDetailFalse()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.Invoice;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeInvoiceIsCalledFor_ResultingFieldsContainEachFalse()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.Invoice;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypeItemUsageIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.ItemUsage;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeItemUsageIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.ItemUsage;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypeMarketingPreferencesIsCalledFor_ResultingFieldsContainDetailFalse()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.MarketingPreferences;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeMarketingPreferencesIsCalledFor_ResultingFieldsContainEachFalse()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.MarketingPreferences;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypeOrderDetailIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.OrderDetail;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeOrderDetailIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.OrderDetail;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypePendingTransactionsIsCalledFor_ResultingFieldsContainDetailFalse()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.PendingTransactions;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypePendingTransactionsIsCalledFor_ResultingFieldsContainEachFalse()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.PendingTransactions;
                var testListType = ListType.Contract;
                var expected = "Each";

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
            public void WhenExportTypeProductsIsCalledFor_ResultingFieldsContainDetailTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.Products;
                var testListType = ListType.Contract;
                var expected = "Detail";

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
            public void WhenExportTypeProductsIsCalledFor_ResultingFieldsContainEachTrue()
            {
                // arrange
                var mockDependents = new MockDependents();
                IExportSettingLogic testunit = MakeTestsLogic(true, ref mockDependents);
                var testGuid = new Guid();
                var testExportType = ExportType.Products;
                var testListType = ListType.Contract;
                var expected = "Each";

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

        public class MockDependents
        {
            public Mock<IExportSettingRepository> ExportSettingRepository { get; set; }

            public Mock<IExternalCatalogRepository> ExternalCatalogRepository { get; set; }

            public Mock<IUnitOfWork> UnitOfWork { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
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

            public static Mock<IExportSettingRepository> MakeIExportSettingRepository()
            {
                var mock = new Mock<IExportSettingRepository>();

                return mock;
            }

            public static Mock<IExternalCatalogRepository> MakeIExternalCatalogRepository()
            {
                var mock = new Mock<IExternalCatalogRepository>();

                return mock;
            }

            public static Mock<IUnitOfWork> MakeIUnitOfWork()
            {
                var mock = new Mock<IUnitOfWork>();

                return mock;
            }
        }

        private static IExportSettingLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
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

            var testunit = new ExportSettingLogicImpl(mockDependents.UnitOfWork.Object,
                mockDependents.ExportSettingRepository.Object, mockDependents.ExternalCatalogRepository.Object);
            return testunit;
        }

        #endregion Setup
    }
}