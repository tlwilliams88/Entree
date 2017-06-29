using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class CustomListDetails : Migration {
        public override void Up() {
            Insert.IntoTable("CustomListDetails")
                  .InSchema("List")
                  .Row(new {
                      HeaderId = 1,
                      ItemNumber = "123456",
                      LineNumber = 100,
                      Each = true,
                      Par = 1,
                      Label = "Fake Label",
                      CatalogId = "FDF",
                      CustomInventoryItemId = 0,
                      Active = true,
                      CreatedUtc = new DateTime(2017, 6, 26, 15, 37, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 26, 15, 38, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("CustomListDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        LineNumber = 200,
                        Par = 0,
                        Active = true,
                        CreatedUtc = new DateTime(2017, 6, 26, 15, 37, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 26, 15, 38, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
