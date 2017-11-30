using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2017._06._03 {
    [Migration(64, "Add CatalogCampaignImagesURL TO AppConfig")]
    public class Migration0064 : BaseMigration {
        public Migration0064() {
            base.MigrationNumber = "0064";
        }
    }
}
