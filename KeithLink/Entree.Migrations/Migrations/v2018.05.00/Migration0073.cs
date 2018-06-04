using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._05._00 {
    [Migration(73, "Added productgroupinginsightkey and customerversioninsightkey to growthandrecoveries table. Updating view.")]
    public class Migration0073 : BaseMigration {
        public Migration0073() {
            base.MigrationNumber = "0073";
        }
    }
}
