using KeithLink.Svc.Core.Extensions.Reports;
using KeithLink.Svc.Core.Models.Lists;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Reports
{
    public class ItemBarcodeExtensionsTests
    {
        private static ListItemModel MakeListItemModel()
        {
            return new ListItemModel()
            {
                ItemNumber = "123456",
                Name = "Fake Name",
                PackSize = "Fake PackSize"
            };
        }

        public class ToItemBarcodeModel
        {
            [Fact]
            public void GoodListItem_ReturnsExpectedItemNumber()
            {
                // arrange
                var item = MakeListItemModel();
                var expected = MakeListItemModel().ItemNumber;
                byte[] testBarcode = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

                // act
                var results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedName()
            {
                // arrange
                var item = MakeListItemModel();
                var expected = MakeListItemModel().Name;
                byte[] testBarcode = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

                // act
                var results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedPackSize()
            {
                // arrange
                var item = MakeListItemModel();
                var expected = MakeListItemModel().PackSize;
                byte[] testBarcode = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

                // act
                var results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.PackSize
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedBarcode()
            {
                // arrange
                var item = MakeListItemModel();
                byte[] testBarcode = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                var expected = testBarcode;

                // act
                var results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.BarCode
                       .Should()
                       .BeEquivalentTo(expected);
            }
        }
    }
}