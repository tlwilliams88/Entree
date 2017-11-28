using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Marketing {
    [Profile("IntegrationTests")]
    public class CampaignCustomers : Migration {
        public override void Up() {
            Insert.IntoTable("CampaignCustomers")
                  .InSchema("Marketing")
                  .Row(new { 
                    CampaignId = 1,
                    BranchId = "FDF",
                    CustomerNumber = "123456"
                  });
            Insert.IntoTable("CampaignCustomers")
                  .InSchema("Marketing")
                  .Row(new {
                        CampaignId = 1,
                        BranchId = "FDF",
                        CustomerNumber = "123457"
                    });
        }

        public override void Down() {
        }
    }
}
