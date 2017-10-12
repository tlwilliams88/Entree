using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Reports;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Repository.SiteCatelog {
    public class ItemBarcodeImageRepositoryTests : BaseDITests {
        private static IItemBarcodeImageRepository MakeTestUnit() {
            ContainerBuilder cb = GetTestsContainer();

            IContainer testcontainer = cb.Build();

            return testcontainer.Resolve<IItemBarcodeImageRepository>();
        }

        public class GetBarcodeForList {
            [Fact]
            public void BadListModel_ReturnsNull() {
                // arrange
                IItemBarcodeImageRepository testunit = MakeTestUnit();

                // act
                List<ItemBarcodeModel> results = testunit.GetBarcodeForList(null);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodEmptyListModel_ReturnsEmptyListOfBarcodes() {
                // arrange
                IItemBarcodeImageRepository testunit = MakeTestUnit();
                ListModel test = new ListModel {
                    BranchId = "XXX",
                    CustomerNumber = "123456"
                };

                // act
                List<ItemBarcodeModel> results = testunit.GetBarcodeForList(test);

                // assert
                results.Count()
                       .Should()
                       .Be(0);
            }

            [Fact]
            public void GoodListModel_ReturnsListOfBarcodes() {
                // arrange
                IItemBarcodeImageRepository testunit = MakeTestUnit();
                ListModel test = new ListModel {
                    BranchId = "XXX",
                    CustomerNumber = "123456",
                    Items = new List<ListItemModel> {
                        new ListItemModel {ItemNumber = "123456"}
                    }
                };

                // act
                List<ItemBarcodeModel> results = testunit.GetBarcodeForList(test);

                // assert
                results.Should()
                       .NotBeNull();
            }
        }
    }
}