using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._01._00 {
    [Migration(66, "Add place to store where an item comes from on the frontend")]
    public class Migration0066 : BaseMigration {
        public Migration0066() {
            base.MigrationNumber = "0066";
        }
    }
}
