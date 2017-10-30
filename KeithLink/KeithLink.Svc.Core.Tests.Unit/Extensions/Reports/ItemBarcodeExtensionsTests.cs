using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Extensions.Reports;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Reports;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Reports {
    public class ItemBarcodeExtensionsTests {
        private static ListItemModel MakeListItemModel() {
            return new ListItemModel {
                ItemNumber = "123456",
                Name = "Fake Name",
                PackSize = "Fake PackSize"
            };
        }

        public class ToItemBarcodeModel {
            [Fact]
            public void GoodListItem_ReturnsExpectedBarcode() {
                // arrange
                ListItemModel item = MakeListItemModel();
                byte[] testBarcode = {
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20
                };
                byte[] expected = testBarcode;

                // act
                ItemBarcodeModel results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.BarCode
                       .Should()
                       .BeEquivalentTo(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedItemNumber() {
                // arrange
                ListItemModel item = MakeListItemModel();
                string expected = MakeListItemModel()
                        .ItemNumber;
                byte[] testBarcode = {
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20
                };

                // act
                ItemBarcodeModel results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedName() {
                // arrange
                ListItemModel item = MakeListItemModel();
                string expected = MakeListItemModel()
                        .Name;
                byte[] testBarcode = {
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20
                };

                // act
                ItemBarcodeModel results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedPackSize() {
                // arrange
                ListItemModel item = MakeListItemModel();
                string expected = MakeListItemModel()
                        .PackSize;
                byte[] testBarcode = {
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20,
                    0x20
                };

                // act
                ItemBarcodeModel results = item.ToItemBarcodeModel(testBarcode);

                // assert
                results.PackSize
                       .Should()
                       .Be(expected);
            }
        }
    }
}