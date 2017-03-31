using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations
{
    [Migration(18, "Add stored procedures to add or update catalog campaigns")]
    public class Migration18 : Migration
    {
        public override void Up()
        {
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.AddCatalogCampaignHeader.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.AddCatalogCampaignItemByHeader.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.UpdateCatalogCampaignHeader.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.UpdateCatalogCampaignItemByHeader.sql");
        }

        public override void Down()
        {
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.AddCatalogCampaignHeader.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.AddCatalogCampaignItemByHeader.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.UpdateCatalogCampaignHeader.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.UpdateCatalogCampaignItemByHeader.sql");
        }
    }
}
