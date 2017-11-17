using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2017._06._03 {
    [Migration(63, "add flag for has filter on campaign header")]
    public class Migration0063 : BaseMigration {
        public Migration0063() {
            base.MigrationNumber = "0063";
        }
    }
}
