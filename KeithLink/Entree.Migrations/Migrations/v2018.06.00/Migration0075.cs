using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._06._00 {
    [Migration(75, "Added ETL for UNFI Items East")]
    public class Migration0075 : BaseMigration {
        public Migration0075() {
            base.MigrationNumber = "0075";
        }
    }
}
