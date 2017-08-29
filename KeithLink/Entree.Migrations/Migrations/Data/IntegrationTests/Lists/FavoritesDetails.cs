using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class FavoritesDetails : Migration {
        public override void Up() {
            Insert.IntoTable("FavoritesDetails")
                  .InSchema("List")
                  .Row(new {
                      HeaderId = 1,
                      ItemNumber = "123456",
                      LineNumber = 1,
                      Each = true,
                      Label = "Fake Label",
                      CatalogId = "FDF",
                      Active = true,
                      CreatedUtc = new DateTime(2017, 6, 29, 16, 29, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 29, 16, 30, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("FavoritesDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "234567",
                        LineNumber = 2,
                        Each = true,
                        Label = "Fake Label",
                        CatalogId = "FDF",
                        Active = true,
                        CreatedUtc = new DateTime(2017, 6, 29, 16, 29, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 29, 16, 30, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("FavoritesDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "345678",
                        LineNumber = 3,
                        Each = true,
                        Label = "Fake Label",
                        CatalogId = "FDF",
                        Active = false,
                        CreatedUtc = new DateTime(2017, 6, 29, 16, 29, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 29, 16, 30, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("FavoritesDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "456789",
                        LineNumber = 4,
                        Active = true,
                        CreatedUtc = new DateTime(2017, 6, 29, 16, 29, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 29, 16, 30, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
