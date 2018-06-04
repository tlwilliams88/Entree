using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._05._00 {
    [Migration(72, "Added changes to the ETL for loading item data to support SalesIQ")]
    public class Migration0072 : BaseMigration {
        public Migration0072() {
            base.MigrationNumber = "0072";
        }
    }
}
