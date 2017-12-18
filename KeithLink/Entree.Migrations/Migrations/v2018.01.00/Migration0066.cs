using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2017._06._03 {
    [Migration(66, "Add LinkToUrl field to CatalogCampaignHeader")]
    public class Migration0066 : BaseMigration {
        public Migration0066() {
            base.MigrationNumber = "0066";
        }
    }
}
