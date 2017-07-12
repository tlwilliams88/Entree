using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class MandatoryItemsListDetails : Migration {
        public override void Up() {
            Insert.IntoTable("MandatoryItemsListDetails")
                  .InSchema("List")
                  .Row(new { 
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 1,
                    Each = true,
                    CatalogId = "FDF",
                    Active = true,
                    CreatedUtc = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("MandatoryItemsListDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "234567",
                        LineNumber = 2,
                        Each = false,
                        CatalogId = "FDF",
                        Active = true,
                        CreatedUtc = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("MandatoryItemsListDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "345678",
                        LineNumber = 3,
                        Each = false,
                        CatalogId = "FDF",
                        Active = false,
                        CreatedUtc = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("MandatoryItemsListDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "456789",
                        LineNumber = 4,
                        Active = true,
                        CreatedUtc = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("MandatoryItemsListDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 2,
                        ItemNumber = "234567",
                        LineNumber = 2,
                        Each = false,
                        CatalogId = "FDF",
                        Active = true,
                        CreatedUtc = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
