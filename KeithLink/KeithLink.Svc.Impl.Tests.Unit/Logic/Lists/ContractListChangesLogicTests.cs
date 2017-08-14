using System;
using System.Collections.Generic;
using System.Text;

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
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Impl.Logic.Lists;
using KeithLink.Svc.Impl.Seams;

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
                cb.RegisterInstance(MakeMockCatalogLogic().Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeMockCustomerRepository().Object)
                  .As<ICustomerRepository>();
                cb.RegisterInstance(MakeMockEventLogRepository().Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeMockGenericQueueRepository().Object)
                  .As<IGenericQueueRepository>();
                cb.RegisterInstance(MakeMockContractChangesRepository().Object)
                  .As<IContractChangesRepository>();
                cb.RegisterInstance(MakeMockMessageTemplateLogic().Object)
                  .As<IMessageTemplateLogic>();
            }

            public static Mock<ICatalogLogic> MakeMockCatalogLogic()
            {
                var mock = new Mock<ICatalogLogic>();

                mock.Setup(f => f.GetProductsByIds("FUT", It.IsAny<List<string>>()))
                    .Returns(new ProductsReturn()
                    {
                        Products = new List<Product>() {
                                                                                                 new Product() {
                                                                                                                   ItemNumber = "123456",
                                                                                                                   Name = "Test Product"
                                                                                                               }
                                                                                     }
                    });

                return mock;
            }
            public static Mock<ICustomerRepository> MakeMockCustomerRepository()
            {
                var mock = new Mock<ICustomerRepository>();

                mock.Setup(f => f.GetCustomerByCustomerNumber("123456", "FUT"))
                    .Returns(new Customer());

                return mock;
            }

            public static Mock<IEventLogRepository> MakeMockEventLogRepository()
            {
                var mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IGenericQueueRepository> MakeMockGenericQueueRepository()
            {
                var mock = new Mock<IGenericQueueRepository>();

                return mock;
            }

            public static Mock<IContractChangesRepository> MakeMockContractChangesRepository() {
                var mock = new Mock<IContractChangesRepository>();

                return mock;
            }

            public static Mock<IMessageTemplateLogic> MakeMockMessageTemplateLogic()
            {
                var mock = new Mock<IMessageTemplateLogic>();

                mock.Setup(f => f.BuildHeader(It.IsAny<string>(), It.IsAny<Customer>()))
                    .Returns(new StringBuilder("Fake Header"));

                mock.Setup(f => f.ReadForKey("NotifHeader"))
                    .Returns(new MessageTemplateModel()
                    {
                        TemplateKey = "NotifHeader",
                        IsBodyHtml = true,
                        Body = "<table style=\"width: 100 %; \">" +
"<tr>" +
"<td>| LOGO |</td>" +
"<td style = \"text-align:center;\"><h3>{Subject}</h3></td>" +
"<td style = \"text-align:right;\">" +
"<table>" +
"<tr>" +
"<td>{CustomerName}</td>" +
"</tr>" +
"<tr>" +
"<td> Customer: {CustomerNumber}</td>" +
"</tr>" +
"<tr>" +
"<td> Branch: {BranchID}</td>" +
"</tr></table></td></tr></table><hr/>"});

                mock.Setup(f => f.ReadForKey("ContractChangeNotice"))
                    .Returns(new MessageTemplateModel() {
                                                            TemplateKey = "ContractChangeNotice",
                                                            Subject = "Ben E. Keith: Contract Change Notice for {CustomerNumber}-{CustomerName}",
                                                            IsBodyHtml = true,
                                                            Body =  "{NotifHeader}<table style=\"width: 100 %; \">" +
                                                                    "<tr style = \"border-bottom:1px solid gray;\" >" +
                                                                    "<th style = \"text-align:left;\" > Change </ th >" +
                                                                    "<th style = \"text-align:left;\" > Item # </th>" +
                                                                    "<th style = \"text-align:left;\" > Description </th>" +
                                                                    "<th style = \"text-align:left;\" > Brand </th>" +
                                                                    "<th style = \"text-align:left;\" > Pack </th>" +
                                                                    "<th style = \"text-align:left;\" > Size </th>" +
                                                                    "</tr>" +
                                                                    "{ContractChangeItems}" +
                                                                    "</table>"});

                mock.Setup(f => f.ReadForKey("ContractChangeItem"))
                    .Returns(new MessageTemplateModel()
                    {
                        TemplateKey = "ContractChangeItem",
                        IsBodyHtml = true,
                        Body = "<tr>" +
"<td style = \"text-align:left;font-size:small;\" >{Status}</td>" +
"<td style = \"text-align:left;font-size:small;\" >{ProductNumber}</td>" +
"<td style = \"text-align:left;font-size:small;\" >{ProductDescription}</td>" +
"<td style = \"text-align:left;font-size:small;\" >{Brand}</td>" +
"<td style = \"text-align:left;font-size:small;\" >{Pack}</td>" +
"<td style = \"text-align:left;font-size:small;\" >{Size}</td>" +
"</tr>"});

                return mock;
            }

        }

        private static IContractListChangesLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents)
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
                mockDependents.CatalogLogic = MockDependents.MakeMockCatalogLogic();
                mockDependents.CustomerRepository = MockDependents.MakeMockCustomerRepository();
                mockDependents.EventLogRepository = MockDependents.MakeMockEventLogRepository();
                mockDependents.GenericQueueRepository = MockDependents.MakeMockGenericQueueRepository();
                mockDependents.ContractChangesRepository = MockDependents.MakeMockContractChangesRepository();
                mockDependents.MessageTemplateLogic = MockDependents.MakeMockMessageTemplateLogic();

                var testunit = new ContractListChangesLogicImpl(mockDependents.CatalogLogic.Object, mockDependents.CustomerRepository.Object, mockDependents.EventLogRepository.Object, 
                                                                mockDependents.GenericQueueRepository.Object, mockDependents.ContractChangesRepository.Object, mockDependents.MessageTemplateLogic.Object);
                return testunit;
            }
        }
        #endregion

        #region attributes
        private const string MESSAGE_TEMPLATE_CONTRACTCHANGE = "ContractChangeNotice";
        private const string MESSAGE_TEMPLATE_CONTRACTCHANGEITEMS = "ContractChangeItem";
        #endregion

        #region ProcessContractChanges
        public class ProcessContractChanges
        {
            [Fact]
            public void WhenThereAreNoContractChanges_CallsContractChangesRepositoryReadNextSetOnce()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.ContractChangesRepository.Verify(m => m.ReadNextSet(), Times.Once, "not called");
            }

            [Fact]
            public void WhenThereIsJustOneContractChange_CallsContractChangesRepositoryReadNextSetTwice()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ContractChangesRepository.SetupSequence(f => f.ReadNextSet())
                    .Returns(new List<ContractChange>() {
                                                            new ContractChange() {
                                                                                     CustomerNumber = "123456",
                                                                                     BranchId = "FUT",
                                                                                     CatalogId = "FUT",
                                                                                     ItemNumber = "123456",
                                                                                     Status = "Added",
                                                                                     ParentList_Id = 1
                                                                                 }
                                                        })
                    .Returns(null);
                
                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.ContractChangesRepository.Verify(m => m.ReadNextSet(), Times.Exactly(2), "not called");
            }

            [Fact]
            public void WhenThereIsJustOneContractChange1case_CallsToLookupCustomer()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ContractChangesRepository.SetupSequence(f => f.ReadNextSet())
                    .Returns(new List<ContractChange>() {
                                                            new ContractChange() {
                                                                                     CustomerNumber = "123456",
                                                                                     BranchId = "FUT",
                                                                                     CatalogId = "FUT",
                                                                                     ItemNumber = "123456",
                                                                                     Status = "Added",
                                                                                     ParentList_Id = 1
                                                                                 }
                                                        })
                    .Returns(null);

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.CustomerRepository.Verify(m => m.GetCustomerByCustomerNumber(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called");
            }

            [Fact]
            public void WhenThereIsJustOneContractChange_CallsForMessageTemplateLogicBuildHeader()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ContractChangesRepository.SetupSequence(f => f.ReadNextSet())
                    .Returns(new List<ContractChange>() {
                                                            new ContractChange() {
                                                                                     CustomerNumber = "123456",
                                                                                     BranchId = "FUT",
                                                                                     CatalogId = "FUT",
                                                                                     ItemNumber = "123456",
                                                                                     Status = "Added",
                                                                                     ParentList_Id = 1
                                                                                 }
                                                        })
                    .Returns(null);

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.MessageTemplateLogic.Verify(m => m.BuildHeader(It.IsAny<string>(), It.IsAny<Customer>()), Times.Once, "not called");
            }

            [Fact]
            public void WhenThereIsJustOneContractChange_CallsForMessageTemplateLogicReadForKeyContractChangeNotice()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ContractChangesRepository.SetupSequence(f => f.ReadNextSet())
                    .Returns(new List<ContractChange>() {
                                                            new ContractChange() {
                                                                                     CustomerNumber = "123456",
                                                                                     BranchId = "FUT",
                                                                                     CatalogId = "FUT",
                                                                                     ItemNumber = "123456",
                                                                                     Status = "Added",
                                                                                     ParentList_Id = 1
                                                                                 }
                                                        })
                    .Returns(null);

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.MessageTemplateLogic.Verify(m => m.ReadForKey("ContractChangeNotice"), Times.Once, "not called");
            }

            [Fact]
            public void WhenThereIsJustOneContractChange_CallsForCatalogLogicGetProductsByIds()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ContractChangesRepository.SetupSequence(f => f.ReadNextSet())
                    .Returns(new List<ContractChange>() {
                                                            new ContractChange() {
                                                                                     CustomerNumber = "123456",
                                                                                     BranchId = "FUT",
                                                                                     CatalogId = "FUT",
                                                                                     ItemNumber = "123456",
                                                                                     Status = "Added",
                                                                                     ParentList_Id = 1
                                                                                 }
                                                        })
                    .Returns(null);

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.CatalogLogic.Verify(m => m.GetProductsByIds(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Once, "not called");
            }

            [Fact]
            public void WhenThereIsJustOneContractChange_CallsForMessageTemplateLogicReadForKeyContractChangeItem()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ContractChangesRepository.SetupSequence(f => f.ReadNextSet())
                    .Returns(new List<ContractChange>() {
                                                            new ContractChange() {
                                                                                     CustomerNumber = "123456",
                                                                                     BranchId = "FUT",
                                                                                     CatalogId = "FUT",
                                                                                     ItemNumber = "123456",
                                                                                     Status = "Added",
                                                                                     ParentList_Id = 1
                                                                                 }
                                                        })
                    .Returns(null);

                // act
                testunit.ProcessContractChanges();

                // assert
                mockDependents.MessageTemplateLogic.Verify(m => m.ReadForKey("ContractChangeItem"), Times.Once, "not called");
            }

            [Fact]
            public void WhenThereIsJustOneContractChange1case_PublishesMessageWithExpectedItemNumber()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                BEKConfiguration.Add("RabbitMQNotificationServer", "Test");
                BEKConfiguration.Add("RabbitMQNotificationPublisherUserName", "Test");
                BEKConfiguration.Add("RabbitMQNotificationPublisherUserPassword", "Test");
                BEKConfiguration.Add("RabbitMQNotificationVHost", "Test");
                BEKConfiguration.Add("RabbitMQNotificationExchangeV2", "Test");

                var expected = ">123456<";

                mockDependents.ContractChangesRepository.SetupSequence(f => f.ReadNextSet())
                    .Returns(new List<ContractChange>() {
                                                            new ContractChange() {
                                                                                     CustomerNumber = "123456",
                                                                                     BranchId = "FUT",
                                                                                     CatalogId = "FUT",
                                                                                     ItemNumber = "123456",
                                                                                     Status = "Added",
                                                                                     ParentList_Id = 1
                                                                                 }
                                                        })
                    .Returns(null);

                // act
                testunit.ProcessContractChanges();
                BEKConfiguration.Reset();

                // assert
                mockDependents.GenericQueueRepository.Verify(m => m.PublishToDirectedExchange(  It.Is<string>(s => s.IndexOf(expected)>-1), 
                                                                                                It.IsAny<string>(), 
                                                                                                It.IsAny<string>(), 
                                                                                                It.IsAny<string>(), 
                                                                                                It.IsAny<string>(), 
                                                                                                It.IsAny<string>(), 
                                                                                                It.IsAny<string>()), 
                                                                                                Times.Once, 
                                                                                                "not called");
            }
        }
        #endregion
    }
}
