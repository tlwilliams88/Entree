using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Entree.Migrations.Migrations.Data.IntegrationTests.Orders;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Xunit;
using KeithLink.Svc.Core.Models.Orders;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Orders
{
    public class OrderedItemsFromListRepositoryTestscs
    {
        private static IOrderedItemsFromListRepository MakeRepo()
        {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();

            return diMap.Resolve<IOrderedItemsFromListRepository>();
        }

        public class Read : MigratedDatabaseTest
        {
            [Fact]
            public void BadControlNumberAndItemNumber_ReturnsNull()
            {
                // arrange
                var repo = MakeRepo();
                var controlNumber = "xxxxxx";
                var itemNumber = "123456";
                var expected = 0;

                // act
                var test = repo.Read(controlNumber, itemNumber);

                // assert
                test.Should()
                    .BeNull();
            }

            [Fact]
            public void GoodControlNumberAndItemNumber_ReturnsExpectedSourceList()
            {
                // arrange
                var repo = MakeRepo();
                var controlNumber = "000000";
                var itemNumber = "123456";
                var expected = "TestList";

                // act
                var test = repo.Read(controlNumber, itemNumber);

                // assert
                test.SourceList
                    .Should()
                    .Be(expected);
            }

        }

        public class Write : MigratedDatabaseTest {
            [Fact] public void TestControlNumberAndItemNumberWriteThenRead_ReturnsExpectedSourceList()
            {
                // arrange
                var repo = MakeRepo();
                var controlNumber = "111111";
                var itemNumber = "123456";
                var testRecord = new OrderItemFromList() {
                    ControlNumber = controlNumber,
                    ItemNumber = itemNumber,
                    SourceList = "TestList"
                };
                var expected = "TestList";

                // act
                repo.Write(testRecord);
                var test = repo.Read(controlNumber, itemNumber);

                // assert
                test.SourceList
                    .Should()
                    .Be(expected);
            }

        }

        public class Purge : MigratedDatabaseTest
        {
            [Fact]
            public void AnyCall_RunsWithoutError()
            {
                // arrange
                var repo = MakeRepo();
                var testNumberOfDays = 7;

                // act
                try
                {
                    repo.Purge(testNumberOfDays);
                    // assert
                    Assert.True(true); //no exception
                }
                catch
                {
                    Assert.True(false);//exception
                }
            }

        }
    }
}
