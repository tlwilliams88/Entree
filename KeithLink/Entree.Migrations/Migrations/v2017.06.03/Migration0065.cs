using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2017._06._03 {
    [Migration(65, "Add LinkToUrl field to CatalogCampaignHeader")]
    public class Migration0065 : BaseMigration {
        public Migration0065() {
            base.MigrationNumber = "0065";
        }
    }
}
