using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    [Profile("IntegrationTests")]
    public class FavoriteListHeaders : Migration {
        public override void Up() {
            Insert.IntoTable("FavoritesHeaders")
                  .InSchema("List")
                  .Row(new {
                      UserId = new Guid("e4ef9796-153d-423a-96e2-d85753d2e9bd"),
                      BranchId = "FIT",
                      CustomerNumber = "503321",
                      CreatedUtc = new DateTime(2017, 6, 29, 14, 30, 0, DateTimeKind.Utc),
                      ModifiedUtc = new DateTime(2017, 6, 29, 14, 31, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("FavoritesHeaders")
                  .InSchema("List")
                  .Row(new {
                        UserId = new Guid("e4ef9796-153d-423a-96e2-d85753d2e9bd"),
                        BranchId = "FIT",
                        CustomerNumber = "503322",
                        CreatedUtc = new DateTime(2017, 6, 29, 14, 30, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 29, 14, 31, 0, DateTimeKind.Utc)
                    });
            Insert.IntoTable("FavoritesHeaders")
                  .InSchema("List")
                  .Row(new {
                        UserId = new Guid("db5280c8-7e5b-450c-ae7a-a55b50c377a3"),
                        BranchId = "FIT",
                        CustomerNumber = "503321",
                        CreatedUtc = new DateTime(2017, 6, 29, 14, 30, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 29, 14, 31, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
