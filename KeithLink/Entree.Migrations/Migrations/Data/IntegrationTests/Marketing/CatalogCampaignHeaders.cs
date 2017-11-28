using System;

using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Marketing {
    [Profile("IntegrationTests")]
    public class CatalogCampaignHeader : Migration {
        public override void Up() {
            Insert.IntoTable("CatalogCampaignHeader")
                  .InSchema("Marketing")
                  .Row(new { 
                    Name = "Name1",
                    Description = "Description1",
                    Uri = "uri-1",
                    StartDate = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified),
                    EndDate = new DateTime(2030, 7, 3, 16, 9, 0, DateTimeKind.Unspecified),
                    Active = true,
                    HasFilter = true,
                  });
            Insert.IntoTable("CatalogCampaignHeader")
                  .InSchema("Marketing")
                  .Row(new { 
                    Name = "Active false",
                    Description = "Active False",
                    Uri = "active-false",
                    StartDate = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified),
                    EndDate = new DateTime(2030, 7, 3, 16, 9, 0, DateTimeKind.Unspecified),
                    Active = false,
                    HasFilter = false,
                  });
            Insert.IntoTable("CatalogCampaignHeader")
                  .InSchema("Marketing")
                  .Row(new { 
                    Name = "Expired dates",
                    Description = "Expired dates",
                    Uri = "expired-dates",
                    StartDate = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Unspecified),
                    EndDate = new DateTime(2017, 7, 4, 16, 9, 0, DateTimeKind.Unspecified),
                    Active = true,
                    HasFilter = false
                  });
        }

        public override void Down() {
            // do nothing
        }
    }
}
