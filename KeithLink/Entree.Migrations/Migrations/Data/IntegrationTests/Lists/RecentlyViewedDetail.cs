using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class RecentlyViewedDetails : Migration {
        public override void Up() {
            Insert.IntoTable("RecentlyViewedDetails")
                  .InSchema("List")
                  .Row(new { 
                    HeaderId = 1,
                    ItemNumber = "123456",
                    Each = true,
                    CatalogId = "FRT",
                    LineNumber = 1,
                    CreatedUtc = new DateTime(2017, 7, 5, 9, 18, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 5, 9, 19, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("RecentlyViewedDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "234567",
                        Each = false,
                        CatalogId = "FRT",
                        LineNumber = 2,
                        CreatedUtc = new DateTime(2017, 7, 5, 9, 18, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 5, 9, 19, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("RecentlyViewedDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "345678",
                        Each = true,
                        CatalogId = "FRT",
                        LineNumber = 3,
                        CreatedUtc = new DateTime(2017, 7, 5, 9, 18, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 5, 9, 19, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("RecentlyViewedDetails")
                  .InSchema("List")
                  .Row(new
                  {
                      HeaderId = 1,
                      ItemNumber = "444444",
                      Each = false,
                      CatalogId = "FRT",
                      LineNumber = 4,
                      CreatedUtc = DateTime.UtcNow,
                      ModifiedUtc = DateTime.UtcNow
                  });
            Insert.IntoTable("RecentlyViewedDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        ItemNumber = "456789",
                        CreatedUtc = new DateTime(2017, 7, 5, 9, 18, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 5, 9, 19, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
