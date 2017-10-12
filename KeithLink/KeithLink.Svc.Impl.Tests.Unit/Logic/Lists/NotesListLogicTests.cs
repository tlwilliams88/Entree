using System.Collections.Generic;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class NotesListLogicTests : BaseDITests {
        private static INotesListLogic MakeTestsObject() {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<INotesHeadersListRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<INotesDetailsListRepository>();

            IContainer testcontainer = cb.Build();

            return testcontainer.Resolve<INotesListLogic>();
        }

        private static INotesHeadersListRepository MakeMockHeaderRepo() {
            Mock<INotesHeadersListRepository> mockHeaderRepo = new Mock<INotesHeadersListRepository>();

            mockHeaderRepo.Setup(h => h.GetHeadersForCustomer(
                                                              It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                              c.CustomerId == "123456")))
                          .Returns(new NotesListHeader {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              Id = 1
                          });

            return mockHeaderRepo.Object;
        }

        private static INotesDetailsListRepository MakeMockDetailsRepo() {
            Mock<INotesDetailsListRepository> mockDetailsRepo = new Mock<INotesDetailsListRepository>();

            mockDetailsRepo.Setup(h => h.GetAll(It.Is<long>(l => l == 1)))
                           .Returns(new List<NotesListDetail> {
                               new NotesListDetail {
                                   CatalogId = "FUT",
                                   ItemNumber = "123456",
                                   Each = false,
                                   LineNumber = 1,
                                   Id = 1,
                                   Note = "test note"
                               }
                           });

            mockDetailsRepo.Setup(h => h.Get(It.Is<long>(l => l == 1), "123456"))
                           .Returns(new NotesListDetail {
                               CatalogId = "FUT",
                               ItemNumber = "123456",
                               Each = false,
                               LineNumber = 1,
                               Id = 1,
                               Note = "test note"
                           });

            return mockDetailsRepo.Object;
        }

        public class GetNote {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                string testItemNumber = "123456";

                // act
                ListItemModel results = testunit.GetNote(testcontext, testItemNumber);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                string testItemNumber = "123456";

                // act
                ListItemModel results = testunit.GetNote(testcontext, testItemNumber);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                string testItemNumber = "123456";
                string expectedNote = "test note";

                // act
                ListItemModel results = testunit.GetNote(testcontext, testItemNumber);

                // assert
                results.Notes
                       .Should()
                       .Be(expectedNote);
            }
        }

        #region GetNotes
        public class GetNotes {
            [Fact]
            public void BadBranchId_ReturnsEmptyList() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserProfile testuser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                List<ListItemModel> results = testunit.GetNotes(testuser, testcontext);

                // assert
                results.Count.Should()
                       .Be(0);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserProfile testuser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                List<ListItemModel> results = testunit.GetNotes(testuser, testcontext);

                // assert
                results.Count.Should()
                       .Be(0);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserProfile testuser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                int expectedCount = 1;

                // act
                List<ListItemModel> results = testunit.GetNotes(testuser, testcontext);

                // assert
                results.Count.Should()
                       .Be(expectedCount);
            }
        }
        #endregion GetNotes

        public class GetList {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                ListModel results = testunit.GetList(testcontext);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };

                // act
                ListModel results = testunit.GetList(testcontext);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                INotesListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                int expectedListId = 1;
                // act
                ListModel results = testunit.GetList(testcontext);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class SaveNote {
// works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCall_CallsGetHeader() {
                // arrange
                Mock<INotesHeadersListRepository> mockHeaderRepo = new Mock<INotesHeadersListRepository>();
                Mock<INotesDetailsListRepository> mockDetailsRepo = new Mock<INotesDetailsListRepository>();
                NotesListLogicImpl testunit = new NotesListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListItemModel testNotes = new ListItemModel {
                    ItemNumber = "123456",
                    Notes = "test note"
                };

                // act
                testunit.SaveNote(testcontext, testNotes);

                // assert - Always returns what is setup provided the mock is called
                mockHeaderRepo.Verify(h => h.GetHeadersForCustomer(testcontext), Times.Once(), "Error updating");
            }

            [Fact]
            public void AnyCall_CallsSaveDetail() {
                // arrange
                Mock<INotesHeadersListRepository> mockHeaderRepo = new Mock<INotesHeadersListRepository>();
                Mock<INotesDetailsListRepository> mockDetailsRepo = new Mock<INotesDetailsListRepository>();
                NotesListLogicImpl testunit = new NotesListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListItemModel testNotes = new ListItemModel {
                    ItemNumber = "123456",
                    Notes = "test note"
                };

                // act
                testunit.SaveNote(testcontext, testNotes);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.IsAny<NotesListDetail>()), Times.Once(), "Error updating");
            }

            [Fact]
            public void CallsWhereCustomerHasNoExistingHeader_CallsSaveHeader() {
                // arrange
                Mock<INotesHeadersListRepository> mockHeaderRepo = new Mock<INotesHeadersListRepository>();
                Mock<INotesDetailsListRepository> mockDetailsRepo = new Mock<INotesDetailsListRepository>();
                NotesListLogicImpl testunit = new NotesListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListItemModel testNotes = new ListItemModel {
                    ItemNumber = "123456",
                    Notes = "test note"
                };

                // act
                testunit.SaveNote(testcontext, testNotes);

                // assert - Always returns what is setup provided the mock is called
                mockHeaderRepo.Verify(h => h.Save(It.IsAny<NotesListHeader>()), Times.Once(), "Error updating");
            }
        }
    }
}