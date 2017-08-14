using System;

using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class RecentlyViewedHeaders : Migration {
        public override void Up() {
            Insert.IntoTable("RecentlyViewedHeaders")
                  .InSchema("List")
                  .Row(new {
                      UserId = new Guid("e4ef9796-153d-423a-96e2-d85753d2e9bd"),
                      BranchId = "FDF",
                      CustomerNumber = "123456",
                      CreatedUtc = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 7, 3, 16, 9, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("RecentlyViewedHeaders")
                  .InSchema("List")
                  .Row(new {
                      UserId = new Guid("e4ef7701-701d-701a-96e2-d85753d2e9bd"),
                      BranchId = "FDF",
                      CustomerNumber = "234567",
                      CreatedUtc = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 7, 3, 16, 9, 0, DateTimeKind.Utc)
                  });
        }

        public override void Down() {
            // do nothing
        }
    }
}