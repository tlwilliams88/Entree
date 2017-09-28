using System;
using System.Collections.Generic;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Service.Invoices;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Service.Invoices
{
    public class ExportInvoicesServiceTests : BaseDITests
    {
        #region Setup
        public class MockDependents
        {
            public Mock<ICacheRepository> ICacheRepository { get; set; }

            public Mock<IExportSettingLogic> IExportSettingLogic { get; set; }

            public Mock<ICustomerRepository> ICustomerRepository { get; set; }

            public Mock<IOrderLogic> IOrderLogic { get; set; }

            public Mock<IOnlinePaymentsLogic> IOnlinePaymentsLogic { get; set; }

            public Mock<IKPayInvoiceRepository> IKPayInvoiceRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
                cb.RegisterInstance(MakeICacheRepository()
                                            .Object)
                  .As<ICacheRepository>();
                cb.RegisterInstance(MakeIExportSettingLogic()
                                            .Object)
                  .As<IExportSettingLogic>();
                cb.RegisterInstance(MakeICustomerRepository()
                                            .Object)
                  .As<ICustomerRepository>();
                cb.RegisterInstance(MakeIOrderLogic()
                                            .Object)
                  .As<IOrderLogic>();
                cb.RegisterInstance(MakeIOnlinePaymentsLogic()
                                            .Object)
                  .As<IOnlinePaymentsLogic>();
                cb.RegisterInstance(MakeIKPayInvoiceRepository()
                                            .Object)
                  .As<IKPayInvoiceRepository>();
            }

            public static Mock<ICacheRepository> MakeICacheRepository()
            {
                var mock = new Mock<ICacheRepository>();

                return mock;
            }

            public static Mock<IExportSettingLogic> MakeIExportSettingLogic()
            {
                var mock = new Mock<IExportSettingLogic>();

                return mock;
            }

            public static Mock<ICustomerRepository> MakeICustomerRepository()
            {
                var mock = new Mock<ICustomerRepository>();

                return mock;
            }

            public static Mock<IOrderLogic> MakeIOrderLogic()
            {
                var mock = new Mock<IOrderLogic>();

                mock.Setup(f => f.GetOrder(It.Is<string>(i => i == "XXX"),
                                           It.IsAny<string>()))
                    .Returns(new Order() {
                                             CatalogId = "XXX",
                                             Items = new List<OrderLine>() {
                                                                               new OrderLine() {
                                                                                                   ItemNumber = "123456"
                                                                                               }
                                                                           }
                                         });

                return mock;
            }

            public static Mock<IOnlinePaymentsLogic> MakeIOnlinePaymentsLogic()
            {
                var mock = new Mock<IOnlinePaymentsLogic>();

                return mock;
            }

            public static Mock<IKPayInvoiceRepository> MakeIKPayInvoiceRepository()
            {
                var mock = new Mock<IKPayInvoiceRepository>();

                return mock;
            }

        }

        private static IExportInvoicesService MakeTestsService(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                var testcontainer = cb.Build();

                return testcontainer.Resolve<IExportInvoicesService>();
            }
            else
            {
                mockDependents = new MockDependents();
                mockDependents.IExportSettingLogic = MockDependents.MakeIExportSettingLogic();
                mockDependents.ICustomerRepository = MockDependents.MakeICustomerRepository();
                mockDependents.IOrderLogic = MockDependents.MakeIOrderLogic();
                mockDependents.IOnlinePaymentsLogic = MockDependents.MakeIOnlinePaymentsLogic();
                mockDependents.IKPayInvoiceRepository = MockDependents.MakeIKPayInvoiceRepository();

                var testunit = new ExportInvoicesServiceImpl(mockDependents.IExportSettingLogic.Object, mockDependents.IOrderLogic.Object,
                                                             mockDependents.IOnlinePaymentsLogic.Object, mockDependents.IKPayInvoiceRepository.Object,
                                                             mockDependents.ICustomerRepository.Object);
                return testunit;
            }
        }
        #endregion

        #region GetExportableInvoiceItems
        public class GetExportableInvoiceItems
        {
            [Fact]
            public void EveryCall_CallsIExportSettingLogicSaveUserExportSettingsOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testrequest = new ExportRequestModel() {
                                                               Fields = new List<ExportModelConfiguration>()
                                                           };
                var testinvoice = "";
                var testdictionary = new Dictionary<string, string>();
                var emptyList = 0;

                // act
                var results = testunit.GetExportableInvoiceItems(testuser, testcontext, testrequest, testinvoice, testdictionary);

                // assert
                mockDependents.IExportSettingLogic
                              .Verify
                                  (m => m.SaveUserExportSettings(It.IsAny<Guid>(), 
                                                                 It.IsAny<ExportType>(), 
                                                                 It.IsAny<ListType>(),
                                                                 It.IsAny<List<ExportModelConfiguration>>(),
                                                                 It.IsAny<string>()), 
                                                                 Times.Once, 
                                                                 "not called");
            }

            [Fact]
            public void EveryCall_CallsIOrderLogicGetOrderOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testrequest = new ExportRequestModel()
                {
                    Fields = new List<ExportModelConfiguration>()
                };
                var testinvoice = "";
                var testdictionary = new Dictionary<string, string>();
                var emptyList = 0;

                // act
                var results = testunit.GetExportableInvoiceItems(testuser, testcontext, testrequest, testinvoice, testdictionary);

                // assert
                mockDependents.IOrderLogic
                              .Verify
                                  (m => m.GetOrder(It.IsAny<string>(),
                                                   It.IsAny<string>()),
                                                   Times.Once,
                                                   "not called");
            }

            [Fact]
            public void EveryCallWhereTheOrderHasOneItem_CallsIOnlinePaymentsLogicAssignContractCategoryOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testrequest = new ExportRequestModel()
                {
                    Fields = new List<ExportModelConfiguration>()
                };
                var testinvoice = "";
                var testdictionary = new Dictionary<string, string>();
                var emptyList = 0;

                // act
                var results = testunit.GetExportableInvoiceItems(testuser, testcontext, testrequest, testinvoice, testdictionary);

                // assert
                mockDependents.IOnlinePaymentsLogic
                              .Verify
                                  (m => m.AssignContractCategory(It.IsAny<Dictionary<string, string>>(),
                                                                 It.IsAny<InvoiceItemModel>()),
                                                                 Times.Once,
                                                                 "not called");
            }
        }
        #endregion

        #region GetExportableInvoice
        public class GetExportableInvoice
        {
            [Fact]
            public void EveryCall_CallsIExportSettingLogicSaveUserExportSettingsOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testrequest = new ExportRequestModel()
                {
                    Fields = new List<ExportModelConfiguration>()
                };
                var testinvoice = "";
                var emptyList = 0;

                // act
                var results = testunit.GetExportableInvoice(testuser, testcontext, testrequest, testinvoice);

                // assert
                mockDependents.IExportSettingLogic
                              .Verify
                                  (m => m.SaveUserExportSettings(It.IsAny<Guid>(),
                                                                 It.IsAny<ExportType>(),
                                                                 It.IsAny<ListType>(),
                                                                 It.IsAny<List<ExportModelConfiguration>>(),
                                                                 It.IsAny<string>()),
                                                                 Times.Once,
                                                                 "not called");
            }

            [Fact]
            public void EveryCall_CallsIKPayInvoiceRepositoryGetInvoiceHeaderOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testrequest = new ExportRequestModel()
                {
                    Fields = new List<ExportModelConfiguration>()
                };
                var testinvoice = "";
                var emptyList = 0;

                // act
                var results = testunit.GetExportableInvoice(testuser, testcontext, testrequest, testinvoice);

                // assert
                mockDependents.IKPayInvoiceRepository
                              .Verify
                                  (m => m.GetInvoiceHeader(It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>()),
                                                           Times.Once,
                                                           "not called");
            }

            [Fact]
            public void EveryCall_CallsICustomerRepositoryGetCustomerByCustomerNumberOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testrequest = new ExportRequestModel()
                {
                    Fields = new List<ExportModelConfiguration>()
                };
                var testinvoice = "";

                // act
                var results = testunit.GetExportableInvoice(testuser, testcontext, testrequest, testinvoice);

                // assert
                mockDependents.ICustomerRepository
                              .Verify
                                  (m => m.GetCustomerByCustomerNumber(It.IsAny<string>(),
                                                                      It.IsAny<string>()),
                                                                      Times.Once,
                                                                      "not called");
            }
        }
        #endregion

        #region GetExportableInvoiceModels
        public class GetExportableInvoiceModels
        {
            [Fact]
            public void EveryCall_CallsIExportSettingLogicSaveUserExportSettingsOnce() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                                                                BranchId = "XXX",
                                                                CustomerId = "123456"
                                                            };
                var testrequest = new InvoiceExportRequestModel() {
                                                                      export = new ExportRequestModel() {
                                                                                                            Fields = new List<ExportModelConfiguration>()
                                                                                                        }
                                                                  };
                var testAllCustomers = false;

                // act
                var results = testunit.GetExportableInvoiceModels(testuser, testcontext, testrequest, testAllCustomers);

                // assert
                mockDependents.IExportSettingLogic
                              .Verify
                              (m => m.SaveUserExportSettings(It.IsAny<Guid>(),
                                                             It.IsAny<ExportType>(),
                                                             It.IsAny<ListType>(),
                                                             It.IsAny<List<ExportModelConfiguration>>(),
                                                             It.IsAny<string>()),
                               Times.Once,
                               "not called");
            }

            [Fact]
            public void EveryCall_CallsIOnlinePaymentsLogicGetInvoiceHeadersOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testrequest = new InvoiceExportRequestModel()
                {
                    export = new ExportRequestModel()
                    {
                        Fields = new List<ExportModelConfiguration>()
                    }
                };
                var testAllCustomers = false;

                // act
                var results = testunit.GetExportableInvoiceModels(testuser, testcontext, testrequest, testAllCustomers);

                // assert
                mockDependents.IOnlinePaymentsLogic
                              .Verify
                              (m => m.GetInvoiceHeaders(It.IsAny<UserProfile>(),
                                                        It.IsAny<UserSelectedContext>(),
                                                        It.IsAny<PagingModel>(),
                                                        It.IsAny<bool>()),
                               Times.Once,
                               "not called");
            }
        }
        #endregion
    }
}
