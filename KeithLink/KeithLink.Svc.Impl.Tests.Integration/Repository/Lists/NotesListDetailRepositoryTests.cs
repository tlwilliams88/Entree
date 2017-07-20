using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class NotesListDetailRepositoryTests {
        private const string GOOD_ITEM_NUMBER = "123456";
        private const int GOOD_HEADER_ID = 1;

        private const string BAD_ITEM_NUMBER = "999999";
        private const int BAD_HEADER_ID = 4;

        private const string NEW_ITEM_NUMBER = "777777";

        public static INotesDetailsListRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<INotesDetailsListRepository>();
        }

        public class GetAllByHeader : MigratedDatabaseTest {
            [Fact]
            public void GoodDetail_HeaderIdMatchesExpected() {
                // arrange
                int expected = 1;
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details[0]
                        .HeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedNotes() {
                // arrange
                string expected = "Walla walla bing bang";
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(GOOD_ITEM_NUMBER))
                       .Note
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                string expected = "FDF";
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(GOOD_ITEM_NUMBER))
                       .CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedDate() {
                // arrange
                DateTime expected = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc);
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(GOOD_ITEM_NUMBER))
                       .CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                bool expected = true;
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(GOOD_ITEM_NUMBER))
                       .Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
                // arrange
                string expected = "123456";
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(GOOD_ITEM_NUMBER))
                       .ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLineNumber() {
                // arrange
                int expected = 1;
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(GOOD_ITEM_NUMBER))
                       .LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedDate() {
                // arrange
                DateTime expected = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc);
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(GOOD_ITEM_NUMBER))
                       .ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsProperCount() {
                // arrange
                int expected = 4;
                INotesDetailsListRepository repo = MakeRepo();

                // act
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.Count()
                       .Should()
                       .Be(expected);
            }
        }

        public class Save : MigratedDatabaseTest {
            private static NotesListDetail MakeDetail() {
                return new NotesListDetail {
                    Note = "Testing ding dong doot",
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = NEW_ITEM_NUMBER,
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodDetail_DoesNotSaveSetCreatedDate() {
                // arrange
                DateTime expected = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc);
                INotesDetailsListRepository repo = MakeRepo();

                // act
                repo.Save(MakeDetail());
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .CreatedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotSaveSetModifiedDate() {
                // arrange
                DateTime expected = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc);
                INotesDetailsListRepository repo = MakeRepo();

                // act
                repo.Save(MakeDetail());
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .ModifiedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodDetail_NewNoteSavesCorrectly() {
                // arrange
                string expected = "Testing ding dong doot";
                INotesDetailsListRepository repo = MakeRepo();

                // act
                repo.Save(MakeDetail());
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .Note
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_NewCatalogIdSavesCorrectly() {
                // arrange
                string expected = "FRT";
                INotesDetailsListRepository repo = MakeRepo();

                // act
                repo.Save(MakeDetail());
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_NewEachSavesCorrectly() {
                // arrange
                bool expected = true;
                INotesDetailsListRepository repo = MakeRepo();

                // act
                repo.Save(MakeDetail());
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_NewItemNumberSavesCorrectly() {
                // arrange
                string expected = NEW_ITEM_NUMBER;
                INotesDetailsListRepository repo = MakeRepo();

                // act
                repo.Save(MakeDetail());
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_NewLineNumberSavesCorrectly() {
                // arrange
                int expected = 2;
                INotesDetailsListRepository repo = MakeRepo();

                // act
                repo.Save(MakeDetail());
                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void PartialDetail_ReturnsNullEach() {
                // arrange
                INotesDetailsListRepository repo = MakeRepo();

                // act
                NotesListDetail detail = MakeDetail();
                detail.Each = null;
                repo.Save(detail);

                List<NotesListDetail> details = repo.GetAll(GOOD_HEADER_ID);

                // assert
                details.First(x => x.ItemNumber.Equals(NEW_ITEM_NUMBER))
                       .Each
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullCatalogId_ThrowsSqlException()
            {
                // arrange
                INotesDetailsListRepository repo = MakeRepo();

                // act
                NotesListDetail detail = MakeDetail();
                detail.CatalogId = null;

                Action act = () => { repo.Save(detail); };

                // assert
                act.ShouldThrow<SqlException>();
            }
        }
    } // Class
} // Namespace