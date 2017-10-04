using System.Collections.Generic;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Log;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Impl.Logic.OnlinePayments;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.OnlinePayments {
    public class OnlinePaymentLogicTests : BaseDITests {
        #region AssignContractCategory
        public class AssignContractCategory {
            [Fact]
            public void WhenBadItemWithContractNotMatchingItem_ResultingCategoryIsNull() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOnlinePaymentsLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Dictionary<string, string> testDict = new Dictionary<string, string>();
                testDict.Add("111111", "Category 1");
                InvoiceItemModel testItem = MakeModel();
                testItem.ItemNumber = "222222";

                // act
                testunit.AssignContractCategory(testDict, testItem);

                // assert
                testItem.Category
                        .Should()
                        .BeNull();
            }

            [Fact]
            public void WhenBadItemWithContractNotMatchingItem_ResultingDetailIsNull() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOnlinePaymentsLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Dictionary<string, string> testDict = new Dictionary<string, string>();
                testDict.Add("111111", "Category 1");
                InvoiceItemModel testItem = MakeModel();
                testItem.ItemNumber = "222222";

                // act
                testunit.AssignContractCategory(testDict, testItem);

                // assert
                testItem.Detail
                        .Should()
                        .BeNull();
            }

            [Fact]
            public void WhenBadItemWithNoContract_ResultingCategoryIsNull() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOnlinePaymentsLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Dictionary<string, string> testDict = new Dictionary<string, string>();
                InvoiceItemModel testItem = MakeModel();
                testItem.ItemNumber = "222222";

                // act
                testunit.AssignContractCategory(testDict, testItem);

                // assert
                testItem.Category
                        .Should()
                        .BeNull();
            }

            [Fact]
            public void WhenBadItemWithNoContract_ResultingDetailIsNull() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOnlinePaymentsLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Dictionary<string, string> testDict = new Dictionary<string, string>();
                InvoiceItemModel testItem = MakeModel();
                testItem.ItemNumber = "222222";

                // act
                testunit.AssignContractCategory(testDict, testItem);

                // assert
                testItem.Detail
                        .Should()
                        .BeNull();
            }

            [Fact]
            public void WhenGoodItemWithContractMatchingItem_ResultingCategoryIsExpected() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOnlinePaymentsLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Dictionary<string, string> testDict = new Dictionary<string, string>();
                testDict.Add("111111", "Category 1");
                InvoiceItemModel testItem = MakeModel();
                string expected = "Category 1";

                // act
                testunit.AssignContractCategory(testDict, testItem);

                // assert
                testItem.Category
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void WhenGoodItemWithContractNotMatchingItem_ResultingDetailIsExpected() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOnlinePaymentsLogic testunit = MakeTestsLogic(true, ref mockDependents);
                Dictionary<string, string> testDict = new Dictionary<string, string>();
                testDict.Add("111111", "Category 1");
                InvoiceItemModel testItem = MakeModel();
                string expected = "Fake Name / 111111 / Fake Brand / Category 1 / Fake Pack / Fake Size";

                // act
                testunit.AssignContractCategory(testDict, testItem);

                // assert
                testItem.Detail
                        .Should()
                        .Be(expected);
            }
        }
        #endregion AssignContractCategory

        #region Setup
        private static InvoiceItemModel MakeModel() {
            return new InvoiceItemModel {
                ItemNumber = "111111",
                Name = "Fake Name",
                BrandExtendedDescription = "Fake Brand",
                PackSize = "Fake Pack / Fake Size"
            };
        }

        public class MockDependents {
            public Mock<ICacheRepository> ICacheRepository { get; set; }

            public Mock<IEventLogRepository> IEventLogRepository { get; set; }

            public Mock<IAuditLogRepository> IAuditLogRepository { get; set; }

            public Mock<IInternalUserAccessRepository> IInternalUserAccessRepository { get; set; }

            public Mock<ICustomerBankRepository> ICustomerBankRepository { get; set; }

            public Mock<ICustomerRepository> ICustomerRepository { get; set; }

            public Mock<IKPayInvoiceRepository> IKPayInvoiceRepository { get; set; }

            public Mock<IKPayLogRepository> IKPayLogRepository { get; set; }

            public Mock<IOrderHistoryHeaderRepsitory> IOrderHistoryHeaderRepsitory { get; set; }

            public Mock<IKPayPaymentTransactionRepository> IKPayPaymentTransactionRepository { get; set; }

            public Mock<IGenericQueueRepository> IGenericQueueRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeICacheRepository()
                                            .Object)
                  .As<ICacheRepository>();
                cb.RegisterInstance(MakeIEventLogRepository()
                                            .Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeIAuditLogRepository()
                                            .Object)
                  .As<IAuditLogRepository>();
                cb.RegisterInstance(MakeIInternalUserAccessRepository()
                                            .Object)
                  .As<IInternalUserAccessRepository>();
                cb.RegisterInstance(MakeICustomerBankRepository()
                                            .Object)
                  .As<ICustomerBankRepository>();
                cb.RegisterInstance(MakeICustomerRepository()
                                            .Object)
                  .As<ICustomerRepository>();
                cb.RegisterInstance(MakeIKPayInvoiceRepository()
                                            .Object)
                  .As<IKPayInvoiceRepository>();
                cb.RegisterInstance(MakeIKPayLogRepository()
                                            .Object)
                  .As<IKPayLogRepository>();
                cb.RegisterInstance(MakeIOrderHistoryHeaderRepsitory()
                                            .Object)
                  .As<IOrderHistoryHeaderRepsitory>();
                cb.RegisterInstance(MakeIKPayPaymentTransactionRepository()
                                            .Object)
                  .As<IKPayPaymentTransactionRepository>();
                cb.RegisterInstance(MakeIGenericQueueRepository()
                                            .Object)
                  .As<IGenericQueueRepository>();
            }

            public static Mock<IGenericQueueRepository> MakeIGenericQueueRepository() {
                Mock<IGenericQueueRepository> mock = new Mock<IGenericQueueRepository>();

                return mock;
            }

            public static Mock<IKPayPaymentTransactionRepository> MakeIKPayPaymentTransactionRepository() {
                Mock<IKPayPaymentTransactionRepository> mock = new Mock<IKPayPaymentTransactionRepository>();

                return mock;
            }

            public static Mock<IOrderHistoryHeaderRepsitory> MakeIOrderHistoryHeaderRepsitory() {
                Mock<IOrderHistoryHeaderRepsitory> mock = new Mock<IOrderHistoryHeaderRepsitory>();

                return mock;
            }

            public static Mock<IKPayLogRepository> MakeIKPayLogRepository() {
                Mock<IKPayLogRepository> mock = new Mock<IKPayLogRepository>();

                return mock;
            }

            public static Mock<IKPayInvoiceRepository> MakeIKPayInvoiceRepository() {
                Mock<IKPayInvoiceRepository> mock = new Mock<IKPayInvoiceRepository>();

                return mock;
            }

            public static Mock<ICustomerRepository> MakeICustomerRepository() {
                Mock<ICustomerRepository> mock = new Mock<ICustomerRepository>();

                return mock;
            }

            public static Mock<ICustomerBankRepository> MakeICustomerBankRepository() {
                Mock<ICustomerBankRepository> mock = new Mock<ICustomerBankRepository>();

                return mock;
            }

            public static Mock<IInternalUserAccessRepository> MakeIInternalUserAccessRepository() {
                Mock<IInternalUserAccessRepository> mock = new Mock<IInternalUserAccessRepository>();

                return mock;
            }

            public static Mock<IAuditLogRepository> MakeIAuditLogRepository() {
                Mock<IAuditLogRepository> mock = new Mock<IAuditLogRepository>();

                return mock;
            }

            public static Mock<IEventLogRepository> MakeIEventLogRepository() {
                Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<ICacheRepository> MakeICacheRepository() {
                Mock<ICacheRepository> mock = new Mock<ICacheRepository>();

                return mock;
            }
        }

        private static IOnlinePaymentsLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IOnlinePaymentsLogic>();
            }
            mockDependents = new MockDependents();
            mockDependents.ICacheRepository = MockDependents.MakeICacheRepository();
            mockDependents.IAuditLogRepository = MockDependents.MakeIAuditLogRepository();
            mockDependents.ICustomerBankRepository = MockDependents.MakeICustomerBankRepository();
            mockDependents.ICustomerRepository = MockDependents.MakeICustomerRepository();
            mockDependents.IEventLogRepository = MockDependents.MakeIEventLogRepository();
            mockDependents.IGenericQueueRepository = MockDependents.MakeIGenericQueueRepository();
            mockDependents.IInternalUserAccessRepository = MockDependents.MakeIInternalUserAccessRepository();
            mockDependents.IKPayInvoiceRepository = MockDependents.MakeIKPayInvoiceRepository();
            mockDependents.IKPayLogRepository = MockDependents.MakeIKPayLogRepository();
            mockDependents.IKPayPaymentTransactionRepository = MockDependents.MakeIKPayPaymentTransactionRepository();
            mockDependents.IOrderHistoryHeaderRepsitory = MockDependents.MakeIOrderHistoryHeaderRepsitory();

            OnlinePaymentLogicImpl testunit = new OnlinePaymentLogicImpl(mockDependents.IKPayInvoiceRepository.Object, mockDependents.ICustomerBankRepository.Object, mockDependents.IOrderHistoryHeaderRepsitory.Object,
                                                                         mockDependents.ICustomerRepository.Object, mockDependents.IGenericQueueRepository.Object, mockDependents.IKPayPaymentTransactionRepository.Object,
                                                                         mockDependents.IKPayLogRepository.Object, mockDependents.IAuditLogRepository.Object, mockDependents.IEventLogRepository.Object,
                                                                         mockDependents.IInternalUserAccessRepository.Object, mockDependents.ICacheRepository.Object);
            return testunit;
        }
        #endregion Setup
    }
}