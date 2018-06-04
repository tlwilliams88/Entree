using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._05._00 {
    [Migration(71, "Add RecommendedItemsAnalytics tables in the Orders namespace")]
    public class Migration0071 : BaseMigration {
        public Migration0071() {
            base.MigrationNumber = "0071";
        }
    }
}
