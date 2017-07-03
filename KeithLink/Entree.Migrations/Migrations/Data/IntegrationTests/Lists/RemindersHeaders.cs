using System;

using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class RemindersHeaders : Migration {
        public override void Up() {
            Insert.IntoTable("RemindersHeader")
                  .InSchema("List")
                  .Row(new { 
                    BranchId = "FDF",
                    CustomListShares = "123456",
                    CreatedUtc = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 3, 16, 9, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("RemindersHeader")
                  .InSchema("List")
                  .Row(new {
                        BranchId = "FDF",
                        CustomListShares = "234567",
                        CreatedUtc = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 7, 3, 16, 9, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
