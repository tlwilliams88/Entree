using System;
using System.Collections.Generic;

using Autofac;
using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Email;

using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Impl.Logic.Lists;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class ContractListChangesLogicTests : BaseDITests
    {
        #region Setup
        public class MockDependents
        {
            public Mock<ICatalogLogic> CatalogLogic { get; set; }

            public Mock<ICustomerRepository> CustomerRepository { get; set; }

            public Mock<IEventLogRepository> EventLogRepository { get; set; }

            public Mock<IGenericQueueRepository> GenericQueueRepository { get; set; }

            public Mock<IContractChangesRepository> ContractChangesRepository { get; set; }

            public Mock<IMessageTemplateLogic> MessageTemplateLogic { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
                cb.RegisterInstance(MakeMockCatalogLogic("").Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeMockCustomerRepository("").Object)
                  .As<ICustomerRepository>();
                cb.RegisterInstance(MakeMockEventLogRepository("").Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeMockGenericQueueRepository("").Object)
                  .As<IGenericQueueRepository>();
                cb.RegisterInstance(MakeMockContractChangesRepository("").Object)
                  .As<IContractChangesRepository>();
                cb.RegisterInstance(MakeMockMessageTemplateLogic("").Object)
                  .As<IMessageTemplateLogic>();
            }

            public static Mock<ICatalogLogic> MakeMockCatalogLogic(string testCase)
            {
                var mock = new Mock<ICatalogLogic>();

                return mock;
            }
            public static Mock<ICustomerRepository> MakeMockCustomerRepository(string testCase)
            {
                var mock = new Mock<ICustomerRepository>();

                return mock;
            }

            public static Mock<IEventLogRepository> MakeMockEventLogRepository(string testCase)
            {
                var mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IGenericQueueRepository> MakeMockGenericQueueRepository(string testCase)
            {
                var mock = new Mock<IGenericQueueRepository>();

                return mock;
            }

            public static Mock<IContractChangesRepository> MakeMockContractChangesRepository(string testCase)
            {
                var mock = new Mock<IContractChangesRepository>();

                switch (testCase) {
                    case "case1":
                        mock.Setup(f => f.ReadNextSet())
                            .Returns(new List<ContractChange>()
                            { new ContractChange() {
                                                ItemNumber = "123456"
                                           }
                            });
                        break;
                }

                return mock;
            }

            public static Mock<IMessageTemplateLogic> MakeMockMessageTemplateLogic(string testCase)
            {
                var mock = new Mock<IMessageTemplateLogic>();

                return mock;
            }

        }

        private static IContractListChangesLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents, string testCase = "")
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                var testcontainer = cb.Build();

                return testcontainer.Resolve<IContractListChangesLogic>();
            }
            else
            {
                mockDependents = new MockDependents();
                mockDependents.CatalogLogic = MockDependents.MakeMockCatalogLogic(testCase);
                mockDependents.CustomerRepository = MockDependents.MakeMockCustomerRepository(testCase);
                mockDependents.EventLogRepository = MockDependents.MakeMockEventLogRepository(testCase);
                mockDependents.GenericQueueRepository = MockDependents.MakeMockGenericQueueRepository(testCase);
                mockDependents.ContractChangesRepository = MockDependents.MakeMockContractChangesRepository(testCase);
                mockDependents.MessageTemplateLogic = MockDependents.MakeMockMessageTemplateLogic(testCase);

                var testunit = new ContractListChangesLogicImpl(mockDependents.CatalogLogic.Object, mockDependents.CustomerRepository.Object, mockDependents.EventLogRepository.Object, 
                                                                mockDependents.GenericQueueRepository.Object, mockDependents.ContractChangesRepository.Object, mockDependents.MessageTemplateLogic.Object);
                return testunit;
            }
        }
        #endregion

        #region ProcessContractChanges
        public class ProcessContractChanges
        {
            [Fact]
            public void EveryCall_CallsContractChangesRepositoryReadNextSetAtLeastOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.ContractChangesRepository.Verify(m => m.ReadNextSet(), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void WhenThereIsAContractChange_CallsContractChangesRepositoryReadNextSet()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents, testCase:"case1");

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.ContractChangesRepository.Verify(m => m.ReadNextSet(), Times.Once, "not called");
            }
        }
        #endregion
    }
}
