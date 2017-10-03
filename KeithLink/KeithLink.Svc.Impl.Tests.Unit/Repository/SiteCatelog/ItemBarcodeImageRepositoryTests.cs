using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Lists;

using Autofac;
using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Repository.SiteCatelog
{
    public class ItemBarcodeImageRepositoryTests : BaseDITests
    {
        private static IItemBarcodeImageRepository MakeTestUnit()
        {
            ContainerBuilder cb = GetTestsContainer();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IItemBarcodeImageRepository>();
        }

        public class GetBarcodeForList
        {
            [Fact]
            public void GoodListModel_ReturnsListOfBarcodes()
            {
                // arrange
                var testunit = MakeTestUnit();
                var test = new ListModel()
                {
                    BranchId = "XXX",
                    CustomerNumber = "123456",
                    Items = new List<ListItemModel>() {
                                                          new ListItemModel() { ItemNumber = "123456"}
                                                      }
                };

                // act
                var results = testunit.GetBarcodeForList(test);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodEmptyListModel_ReturnsEmptyListOfBarcodes()
            {
                // arrange
                var testunit = MakeTestUnit();
                var test = new ListModel()
                {
                    BranchId = "XXX",
                    CustomerNumber = "123456"
                };

                // act
                var results = testunit.GetBarcodeForList(test);

                // assert
                results.Count().Should()
                       .Be(0);
            }

            [Fact]
            public void BadListModel_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestUnit();

                // act
                var results = testunit.GetBarcodeForList(null);

                // assert
                results.Should()
                       .BeNull();
            }
        }
    }
}