using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class ContractDetails : Migration {
        public override void Up() {
            Insert.IntoTable("ContractDetails")
                  .InSchema("List")
                  .Row(new {
                      HeaderId = 1,
                      LineNumber = 1,
                      ItemNumber = "123456",
                      FromDate = new DateTime(2017, 6, 1),
                      ToDate = new DateTime(2017, 6, 30),
                      Each = false,
                      Category = "Fake Category",
                      CatalogId = "FDF",
                      CreatedUtc = new DateTime(2017, 6, 22, 15, 1, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 22, 15, 2, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("ContractDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        LineNumber = 2,
                        ItemNumber = "234567",
                        FromDate = new DateTime(2017, 6, 1),
                        ToDate = new DateTime(2017, 6, 30),
                        Each = true,
                        Category = "Fake Category",
                        CatalogId = "FDF",
                        CreatedUtc = new DateTime(2017, 6, 22, 15, 1, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 22, 15, 2, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("ContractDetails")
                  .InSchema("List")
                  .Row(new {
                        HeaderId = 1,
                        LineNumber = 3,
                        ItemNumber = "345678",
                        Category = "Fake Category",
                        CatalogId = "FDF",
                        CreatedUtc = new DateTime(2017, 6, 22, 15, 1, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 22, 15, 2, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // not used
        }
    }
}
