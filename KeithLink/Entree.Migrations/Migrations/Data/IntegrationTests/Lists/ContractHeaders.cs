using System;
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class ContractHeaders : Migration {
        public override void Up() {
            Insert.IntoTable("ContractHeaders")
                  .InSchema("List")
                  .Row(new {
                      ContractId = "12345678",
                      BranchId = "FDF",
                      CustomerNumber = "123456",
                      CreatedUtc = new DateTime(2017, 6, 16, 15, 28, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 16, 15, 28, 0, DateTimeKind.Utc)
                  });

            Insert.IntoTable("ContractHeaders")
                  .InSchema("List")
                  .Row(new {
                        ContractId = "ABC12345",
                        BranchId = "FDF",
                        CustomerNumber = "123456",
                        CreatedUtc = new DateTime(2017, 6, 22, 12, 13, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 22, 12, 13, 0, DateTimeKind.Utc)
                    });

            Insert.IntoTable("ContractHeaders")
                  .InSchema("List")
                  .Row(new {
                               ContractId = "ABC12345",
                               BranchId = "FDF",
                               CustomerNumber = "234567",
                               CreatedUtc = new DateTime(2017, 6, 22, 12, 14, 0, DateTimeKind.Utc),
                               ModifiedUtc = new DateTime(2017, 6, 22, 12, 14, 0, DateTimeKind.Utc)
                           });
        }

        public override void Down() {
            // not used
        }
    }
}
