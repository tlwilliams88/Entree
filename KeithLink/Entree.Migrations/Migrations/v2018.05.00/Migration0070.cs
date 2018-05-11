using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._05._00 {
    [Migration(70, "Added ETL Staging Growth and Recovery tables")]
    public class Migration0070 : BaseMigration {
        public Migration0070() {
            base.MigrationNumber = "0070";
        }
    }
}
