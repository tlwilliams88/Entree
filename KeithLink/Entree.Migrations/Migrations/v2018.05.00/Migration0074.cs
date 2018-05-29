using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._05._00 {
    [Migration(74, "Added non-clustered index to ETL.Staging_ItemData")]
    public class Migration0074 : BaseMigration {
        public Migration0074() {
            base.MigrationNumber = "0074";
        }
    }
}
