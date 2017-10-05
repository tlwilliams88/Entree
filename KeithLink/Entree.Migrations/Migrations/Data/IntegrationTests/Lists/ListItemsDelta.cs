using System;

using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class ListItemsDelta : Migration {
        public override void Up() {
            Insert.IntoTable("ListItemsDelta")
                  .InSchema("List")
                  .Row(new {
                      ParentList_Id = 1,
                      ItemNumber = "123456",
                      Each = false,
                      CatalogId = "FDF",
                      Status = "Added",
                      CreatedUtc = new DateTime(2017, 6, 22, 15, 1, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 22, 15, 2, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("ListItemsDelta")
                  .InSchema("List")
                  .Row(new {
                      ParentList_Id = 1,
                      ItemNumber = "234567",
                      Each = true,
                      CatalogId = "FDF",
                      Status = "Added",
                      CreatedUtc = new DateTime(2017, 6, 22, 15, 1, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 22, 15, 2, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("ListItemsDelta")
                  .InSchema("List")
                  .Row(new {
                      ParentList_Id = 1,
                      ItemNumber = "345678",
                      CatalogId = "FDF",
                      Status = "Added",
                      CreatedUtc = new DateTime(2017, 6, 22, 15, 1, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 22, 15, 2, 0, DateTimeKind.Utc)
                  });
        }

        public override void Down() {
            // not used
        }
    }
}