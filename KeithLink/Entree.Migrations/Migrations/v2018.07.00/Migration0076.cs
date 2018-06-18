using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._07._00
{
    [Migration(76, "Added new config property for menumax sso shared key")]
    public class Migration0076 : BaseMigration
    {
        public Migration0076()
        {
            base.MigrationNumber = "0076";
        }
    }
}
