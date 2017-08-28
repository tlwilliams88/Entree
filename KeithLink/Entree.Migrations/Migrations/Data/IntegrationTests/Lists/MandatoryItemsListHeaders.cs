using System;

using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class MandatoryItemsListHeaders : Migration {
        public override void Up() {
            Insert.IntoTable("MandatoryItemsHeaders")
                  .InSchema("List")
                  .Row(new { 
                    BranchId = "FRT",
                    CustomerNumber = "123456",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 11, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 12, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("MandatoryItemsHeaders")
                  .InSchema("List")
                  .Row(new {
                        BranchId = "FRT",
                        CustomerNumber = "222222",
                        CreatedUtc = new DateTime(2017, 6, 30, 16, 11, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 16, 12, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("MandatoryItemsHeaders")
                  .InSchema("List")
                  .Row(new {
                        BranchId = "FRT",
                        CustomerNumber = "234567",
                        CreatedUtc = new DateTime(2017, 6, 30, 16, 11, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 16, 12, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
