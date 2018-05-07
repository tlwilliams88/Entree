using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Customers {
    [Profile("IntegrationTests")]
    public class GrowthAndRecoveries : Migration {
        public override void Up() {
            Insert.IntoTable("GrowthAndRecoveries")
                  .InSchema("Customers")
                  .Row(new {
                      BranchId = "FUT",
                      CustomerNumber = "111111",
                      Amount = "100.00",
                      GrowthAndRecoveryProductGroup = 1,
                      GrowthAndRecoveryTypeKey = 1
                  });
            Insert.IntoTable("GrowthAndRecoveries")
                  .InSchema("Customers")
                  .Row(new {
                      BranchId = "FUT",
                      CustomerNumber = "111111",
                      Amount = "90.00",
                      GrowthAndRecoveryProductGroup = 2,
                      GrowthAndRecoveryTypeKey = 2
                  });
            Insert.IntoTable("GrowthAndRecoveries")
                  .InSchema("Customers")
                  .Row(new {
                      BranchId = "FUT",
                      CustomerNumber = "111111",
                      Amount = "90.00",
                      GrowthAndRecoveryProductGroup = 1,
                      GrowthAndRecoveryTypeKey = 3
                  });
            Insert.IntoTable("GrowthAndRecoveries")
                  .InSchema("Customers")
                  .Row(new {
                      BranchId = "FUT",
                      CustomerNumber = "222222",
                      Amount = "100.00",
                      GrowthAndRecoveryProductGroup = 1,
                      GrowthAndRecoveryTypeKey = 1
                  });
            Insert.IntoTable("GrowthAndRecoveries")
                  .InSchema("Customers")
                  .Row(new
                  {
                      BranchId = "FUT",
                      CustomerNumber = "222222",
                      Amount = "50.00",
                      GrowthAndRecoveryProductGroup = 1,
                      GrowthAndRecoveryTypeKey = 1
                  });
        }

        public override void Down() {
            // not used
        }
    }
}
