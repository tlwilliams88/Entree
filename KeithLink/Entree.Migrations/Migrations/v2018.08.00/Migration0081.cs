using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._08._00
{
    [Migration(81, "Add OrderDateTime column to Orders.OrderHistoryHeader table to accurately reflect order entry time.")]
    public class Migration0081 : BaseMigration
    {
        public Migration0081()
        {
            base.MigrationNumber = "0081";
        }
    }
}
