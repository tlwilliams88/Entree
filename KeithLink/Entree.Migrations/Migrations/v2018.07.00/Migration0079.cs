using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._07._00
{
    [Migration(79, "add update contract name in process and fix delete")]
    public class Migration0079 : BaseMigration
    {
        public Migration0079()
        {
            base.MigrationNumber = "0079";
        }
    }
}
