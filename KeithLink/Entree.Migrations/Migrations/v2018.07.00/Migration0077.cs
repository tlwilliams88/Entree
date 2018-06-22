using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._07._00 {
    [Migration(77, "Adding Azure key to app settings table")]
    public class Migration0077 : BaseMigration {
        public Migration0077() {
            base.MigrationNumber = "0077";
        }
    }
}
