using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._04._00 {
    [Migration(68, "Add setting to toggle showing recommended items")]
    public class Migration0068 : BaseMigration {
        public Migration0068() {
            base.MigrationNumber = "0068";
        }
    }
}
