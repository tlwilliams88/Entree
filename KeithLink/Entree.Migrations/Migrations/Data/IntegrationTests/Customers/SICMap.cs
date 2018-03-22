using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Customers {
    [Profile("IntegrationTests")]
    public class SICMap : Migration {
        public override void Up() {
            Insert.IntoTable("SICMap")
                  .InSchema("Customers")
                  .Row(new {
                      CustomerNumber = "123456",
                      BranchId = "FUT",
                      SIC = "1"
                  });
            Insert.IntoTable("SICMap")
                  .InSchema("Customers")
                  .Row(new {
                      CustomerNumber = "234567",
                      BranchId = "FUT",
                      SIC = "2"
                  });
            Insert.IntoTable("SICMap")
                  .InSchema("Customers")
                  .Row(new {
                      CustomerNumber = "345678",
                      BranchId = "FUT",
                      SIC = "3"
                  });
        }

        public override void Down() {
            // not used
        }
    }
}
