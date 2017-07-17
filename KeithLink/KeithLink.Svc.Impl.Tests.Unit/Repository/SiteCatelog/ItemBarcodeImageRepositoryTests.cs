using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Repository.SiteCatelog
{
    public class ItemBarcodeImageRepositoryTests
    {
        private static IContainer GetTestsContainer()
        {
            ContainerBuilder cb = DependencyMapFactory.GetTestsContainer();

            return cb.Build();
        }
        public class GetBarcodeForList
        {
            [Fact]
            public void GoodListModel_ReturnsListOfBarcodes()
            {
                // arrange
                var container = GetTestsContainer();
                var testunit = GetTestsContainer()
                        .Resolve<IItemBarcodeImageRepository>();
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
                var container = GetTestsContainer();
                var testunit = GetTestsContainer()
                        .Resolve<IItemBarcodeImageRepository>();
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
                var container = GetTestsContainer();
                var testunit = GetTestsContainer()
                        .Resolve<IItemBarcodeImageRepository>();

                // act
                var results = testunit.GetBarcodeForList(null);

                // assert
                results.Should()
                       .BeNull();
            }
        }
    }
}
