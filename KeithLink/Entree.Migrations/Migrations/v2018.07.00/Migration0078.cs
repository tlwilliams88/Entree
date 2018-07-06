using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._07._00
{
    [Migration(78, "Added persistence and integration of user feedback with messaging.")]
    public class Migration0078 : BaseMigration
    {
        public Migration0078()
        {
            base.MigrationNumber = "0078";
        }
    }
}
