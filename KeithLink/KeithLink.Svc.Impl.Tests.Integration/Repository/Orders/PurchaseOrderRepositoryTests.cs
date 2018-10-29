using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Entree.Migrations.Migrations.Data.IntegrationTests.Orders;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Xunit;
using KeithLink.Svc.Core.Models.Orders;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Orders
{
    public class PurchaseOrderRepositoryTests
    {
        private static IPurchaseOrderRepository MakeRepo()
        {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();

            return diMap.Resolve<IPurchaseOrderRepository>();
        }

        public class GetPurchaseOrdersByStatus : MigratedDatabaseTest
        {
            [Fact]
            public void FindsOrdersForStatus()
            {
                // arrange
                var repo = MakeRepo();

                // act
                List<PurchaseOrder> purchaseOrders = repo.GetPurchaseOrdersByStatus("Submitted");

                // assert
                purchaseOrders.Should().NotBeNull();
                purchaseOrders.Count().Should().BeGreaterThan(0);
            }

        }

        public class UpdatePurchaseOrderPrices : MigratedDatabaseTest
        {
            [Fact]
            public void PricesAreUpdated()
            {
                // arrange
                var repo = MakeRepo();
                var trackingNumber = "0777774";
                IEnumerable<LineItem> lineItems = new List<LineItem>()
                {
                    //new LineItem { ProductId = "023110", PlacedPrice = 40.36m, ListPrice = 40.36m },
                    //new LineItem { ProductId = "057112", PlacedPrice = 43.97m, ListPrice = 43.97m },
                    new LineItem { ProductId = "023110", PlacedPrice = 2.0m, ListPrice = 2.0m },
                    new LineItem { ProductId = "057112", PlacedPrice = 2.0m, ListPrice = 2.0m },
                };

                // act
                repo.UpdatePurchaseOrderPrices(trackingNumber, lineItems);

                PurchaseOrder purchaseOrder = repo.ReadPurchaseOrderByTrackingNumber(trackingNumber);
                purchaseOrder.Should().NotBeNull();

                Order order = purchaseOrder.ToOrder();

                var orderLines = order.Items
                    .Join(lineItems,
                        orderLine => orderLine.ItemNumber,
                        lineItem => lineItem.ProductId,
                        (orderLine, lineItem) => new { orderLine, lineItem });

                // assert
                orderLines.Count().Should().Be(lineItems.Count());
                orderLines.ToList().ForEach(ol => ol.orderLine.Price.Should().Be((double)ol.lineItem.PlacedPrice));
            }

        }
    }
}
