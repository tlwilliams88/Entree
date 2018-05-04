using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Customers {
    [Profile("IntegrationTests")]
    public class GrowthAndRecoveryProductGroups : Migration {
        public override void Up() {
            Insert.IntoTable("GrowthAndRecoveryProductGroups")
                  .InSchema("Customers")
                  .Row(new {
                      GrowthAndRecoveryProductGroup = 0,
                      Code = 0,
                      Description = "Class Code #0"
                  });
            Insert.IntoTable("GrowthAndRecoveryProductGroups")
                  .InSchema("Customers")
                  .Row(new {
                      GrowthAndRecoveryProductGroup = 1,
                      Code = 1,
                      Description = "Class Code #1"
                  });
            Insert.IntoTable("GrowthAndRecoveryProductGroups")
                  .InSchema("Customers")
                  .Row(new {
                      GrowthAndRecoveryProductGroup = 2,
                      Code = 2,
                      Description = "Class Code #2"
                  });
            Insert.IntoTable("GrowthAndRecoveryProductGroups")
                  .InSchema("Customers")
                  .Row(new {
                      GrowthAndRecoveryProductGroup = 3,
                      Code = "PS0000",
                      Description = "Price List Code"
                  });
        }

        public override void Down() {
            // not used
        }
    }
}
