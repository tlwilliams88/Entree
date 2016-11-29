using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations
{
    [Migration(4, "Add label column to custom inventory list")]
    public class AddLabelToCustomInventoryItems : Migration
    {
        public override void Up()
        {
            this.Execute.Script(@"SQL\v1.12.0\v1.12.0-custom-inventory-label-column-up.sql");
        }

        public override void Down()
        {
            this.Execute.Script(@"SQL\v1.12.0\v1.12.0-custom-inventory-label-column-down.sql");
        }

    }
}
