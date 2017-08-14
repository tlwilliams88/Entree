using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class HistoryHeaders : Migration {
        public override void Up() {
            Insert.IntoTable("HistoryHeaders")
                  .InSchema("List")
                  .Row(new { 
                    BranchId = "FRT",
                    CustomerNumber = "123456",
                    CreatedUtc = new DateTime(2017, 6, 30, 11, 47, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 11, 48, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("HistoryHeaders")
                  .InSchema("List")
                  .Row(new {
                        BranchId = "FRT",
                        CustomerNumber = "234567",
                        CreatedUtc = new DateTime(2017, 6, 30, 11, 47, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 11, 48, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("HistoryHeaders")
                  .InSchema("List")
                  .Row(new {
                        BranchId = "FRT",
                        CustomerNumber = "345678",
                        CreatedUtc = new DateTime(2017, 6, 30, 11, 47, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 11, 48, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("HistoryHeaders")
                  .InSchema("List")
                  .Row(new {
                        BranchId = "FRT",
                        CustomerNumber = "456789",
                        CreatedUtc = new DateTime(2017, 6, 30, 11, 47, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 30, 11, 48, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
