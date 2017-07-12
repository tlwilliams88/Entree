using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class ReminderListDetailsRepositoryTests {
        private static IRemindersListDetailsRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IRemindersListDetailsRepository>();
        }

        public class DeleteReminderListDetail : MigratedDatabaseTest {
            [Fact]
            public void GoodDetailId_RemovesExpectedRecord() {
                // arrange
                var detailId = 4;
                var expected = 0;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                repo.DeleteReminderListDetail(detailId);
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results.Count(r => r.Id == detailId)
                       .Should()
                       .Be(expected);
            }
        }

        public class GetRemindersDetails : MigratedDatabaseTest {
            [Fact]
            public void GoodHeaderId_ReturnsExcpectedCount() {
                // arrange
                var headerId = 1;
                var expected = 3;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);
                
                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExcpectedActiveCount() {
                // arrange
                var headerId = 1;
                var expected = 3;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results.Count(i => i.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExcpectedInactiveCount() {
                // arrange
                var headerId = 1;
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results.Count(i => i.Active == false)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedHeaderId() {
                // arrange
                var headerId = 1;
                var expected = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedItemNumber() {
                // arrange
                var headerId = 1;
                var expected = "123456";
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .ItemNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedEach() {
                // arrange
                var headerId = 1;
                var expected = true;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedCatalogId() {
                // arrange
                var headerId = 1;
                var expected = "FRT";
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .CatalogId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedActive() {
                // arrange
                var headerId = 1;
                var expected = true;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedLineNumber() {
                // arrange
                var headerId = 1;
                var expected = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .LineNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedCreatedUtc() {
                // arrange
                var headerId = 1;
                var expected = new DateTime(2017, 7, 5, 9, 18, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExcpectedModifiedUtc() {
                // arrange
                var headerId = 1;
                var expected = new DateTime(2017, 7, 5, 9, 19, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[0]
                    .ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void NullEach_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[2]
                    .Each
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullCatalogId_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[2]
                    .CatalogId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullLineNumber_ReturnsDefaultValue() {
                // arrange
                var headerId = 1;
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetRemindersDetails(headerId);

                // assert
                results[2]
                    .LineNumber
                    .Should()
                    .Be(expected);
            }
        }

        public class SaveReminderListDetail : MigratedDatabaseTest { 
            private static ReminderItemsListDetail MakeDetail() {
                return  new ReminderItemsListDetail() {
                    Active = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 5, 10, 49, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "567890",
                    LineNumber = 5,
                    ModifiedUtc = new DateTime(2017, 7, 5, 10, 50, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodDetail_SaveCorrectActive() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .Active
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SaveCorrectCatalogId() {
                // arrange
                var detail = MakeDetail();
                var expected = "FRT";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .CatalogId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotSaveSetCreatedUtc() {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 5, 10, 49, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .CreatedUtc
                    .Should()
                    .NotBe(expected);
            }

            [Fact]
            public void GoodDetail_SaveCorrectEach() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SaveCorrectHeaderId() {
                // arrange
                var detail = MakeDetail();
                var expected = 1;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SaveCorrectItemNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = "567890";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .ItemNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SaveCorrectLineNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = 5;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .LineNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotSaveSetModifiedUtc() {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 5, 10, 50, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .ModifiedUtc
                    .Should()
                    .NotBe(expected);
            }

            [Fact]
            public void NullCatalogId_ReturnsNull() {
                // arrange
                var detail = new ReminderItemsListDetail() {
                    Active = true,
                    CreatedUtc = new DateTime(2017, 7, 5, 10, 49, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "567890",
                    LineNumber = 5,
                    ModifiedUtc = new DateTime(2017, 7, 5, 10, 50, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .CatalogId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullEach_ReturnsNull() {
                // arrange
                var detail = new ReminderItemsListDetail() {
                    Active = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 5, 10, 49, 0, DateTimeKind.Utc),
                    HeaderId = 1,
                    ItemNumber = "567890",
                    LineNumber = 5,
                    ModifiedUtc = new DateTime(2017, 7, 5, 10, 50, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveReminderListDetail(detail);
                var results = repo.GetRemindersDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId)
                                    .First();

                // assert
                result
                    .Each
                    .Should()
                    .BeNull();
            }
        }
    }
}
