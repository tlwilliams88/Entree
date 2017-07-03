using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class HistoryDetails : Migration {
        public override void Up() {
            Insert.IntoTable("HistoryDetails")
                  .InSchema("List")
                  .Row(new { 
                    HeaderId = 1,
                    LineNumber = 1,
                    ItemNumber = "123456",
                    Each = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 6, 30, 15, 15, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 15, 16, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("HistoryDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        LineNumber = 2,
                        ItemNumber = "234567",
                        Each = false,
                        CatalogId = "FRT",
                        CreatedUtc = new DateTime(2017, 6, 30, 15, 15, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 15, 16, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("HistoryDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        LineNumber = 3,
                        ItemNumber = "345678",
                        Each = true,
                        CatalogId = "FRT",
                        CreatedUtc = new DateTime(2017, 6, 30, 15, 15, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 15, 16, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("HistoryDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        LineNumber = 4,
                        ItemNumber = "456789",
                        CreatedUtc = new DateTime(2017, 6, 30, 15, 15, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 15, 16, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
