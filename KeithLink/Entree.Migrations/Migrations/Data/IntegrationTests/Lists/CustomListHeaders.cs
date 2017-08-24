using System;

using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class CustomListHeaders : Migration {
        public override void Up() {
            Insert.IntoTable("CustomListHeaders")
                  .InSchema("List")
                  .Row(new {
                      UserId = new Guid("60ffa8da-737d-4dbf-bacb-fe9774b9731f"),
                      BranchId = "FDF",
                      CustomerNumber = "123456",
                      Name = "Fake Custom List 1",
                      Active = true,
                      CreatedUtc = new DateTime(2017, 6, 23, 11, 2, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 23, 11, 3, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("CustomListHeaders")
                  .InSchema("List")
                  .Row(new {
                        UserId = new Guid("60ffa8da-737d-4dbf-bacb-fe9774b9731f"),
                        BranchId = "FDF",
                        CustomerNumber = "123456",
                        Name = "Fake Custom List 2",
                        Active = true,
                        CreatedUtc = new DateTime(2017, 6, 23, 11, 2, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 23, 11, 3, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("CustomListHeaders")
                  .InSchema("List")
                  .Row(new {
                        UserId = new Guid("60ffa8da-737d-4dbf-bacb-fe9774b9731f"),
                        BranchId = "FDF",
                        CustomerNumber = "123456",
                        Name = "Fake Custom List 3",
                        Active = false,
                        CreatedUtc = new DateTime(2017, 6, 23, 11, 2, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 23, 11, 3, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("CustomListHeaders")
                  .InSchema("List")
                  .Row(new {
                        UserId = new Guid("60ffa8da-737d-4dbf-bacb-fe9774b9731f"),
                        BranchId = "FDF",
                        CustomerNumber = "123456",
                        Name = "Fake Custom List 4",
                        Active = true,
                        CreatedUtc = new DateTime(2017, 6, 23, 11, 2, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 23, 11, 3, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("CustomListHeaders")
                  .InSchema("List")
                  .Row(new {
                        UserId = new Guid("60ffa8da-737d-4dbf-bacb-fe9774b9731f"),
                        BranchId = "FDF",
                        CustomerNumber = "234567",
                        Name = "Fake Custom List 5",
                        Active = true,
                        CreatedUtc = new DateTime(2017, 6, 23, 11, 2, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 23, 11, 3, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do not use
        }
    }
}
