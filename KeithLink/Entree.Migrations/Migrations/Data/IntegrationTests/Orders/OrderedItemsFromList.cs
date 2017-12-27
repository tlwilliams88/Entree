using System;

using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Orders {
    [Profile("IntegrationTests")]
    public class OrderedItemsFromList : Migration {
        public override void Up() {
            Insert.IntoTable("OrderedItemsFromList")
                  .InSchema("Orders")
                  .Row(new {
                      ControlNumber = "000000",
                      ItemNumber = "123456",
                      SourceList = "TestList",
                      CreatedUtc = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified)
                  });
        }

        public override void Down() {}
    }
}