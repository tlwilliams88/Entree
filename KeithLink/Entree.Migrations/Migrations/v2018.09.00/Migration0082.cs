using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations.Migrations.v2018._09._00
{
    [Migration(82, "Add UserEmailAddress column to Orders.OrderHistoryHeader table to track the person who entered the order.")]
    public class Migration0082 : BaseMigration
    {
        public Migration0082()
        {
            base.MigrationNumber = "0082";
        }
    }
}
