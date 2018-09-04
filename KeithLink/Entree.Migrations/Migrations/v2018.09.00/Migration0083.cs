using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._09._00
{
    [Migration(83, "Add AppSettings to support OrderHistory processing options and diagnostics.")]
    public class Migration0083 : BaseMigration
    {
        public Migration0083()
        {
            base.MigrationNumber = "0083";
        }
    }
}
