using Entree.Migrations.Core;
using BEK.FluentMigratorBase; 
using FluentMigrator;


namespace Entree.Migrations {
    [Migration(35, "Updating divison names according to new naming convention.  Example, no one from Louisiana wants to order from San Antonio.")]
    public class Migration0035 : Core.BaseMigrationClass {
        public Migration0035() {
            base.MigrationNumber = "0035";
        }
    }
}