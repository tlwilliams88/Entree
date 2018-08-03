using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._08._00
{
    [Migration(80, "Adding column to customers staging table to support minimum order amount")]
    public class Migration0080 : BaseMigration
    {
        public Migration0080()
        {
            base.MigrationNumber = "0080";
        }
    }
}