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
            Execute.Script(@"SQL\v2017.2.0\up\tables\Marketing.CatalogCampaignHeader_up.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.Marketing.GetAllCatalogCampaignHeader_up.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.AddCatalogCampaignHeader_up.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.AddCatalogCampaignItemByHeader_up.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.UpdateCatalogCampaignHeader_up.sql");
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Marketing.UpdateCatalogCampaignItemByHeader_up.sql");
        }

        public override void Down()
        {
            Execute.Script(@"SQL\v2017.2.0\down\tables\Marketing.CatalogCampaignHeader_down.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.Marketing.GetAllCatalogCampaignHeader_down.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.AddCatalogCampaignHeader_down.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.AddCatalogCampaignItemByHeader_down.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.UpdateCatalogCampaignHeader_down.sql");
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Marketing.UpdateCatalogCampaignItemByHeader_down.sql");
        }
    }
}
