using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(3, "Added CustomInventoryId Column to ListItems")]
    public class CustomInventoryItemId : Migration {
        public override void Up() {
            this.Execute.Script(@"SQL\v1.12.0\v1.12.0-up.sql");
        }

        public override void Down() {
            this.Execute.Script(@"SQL\v1.12.0\v1.12.0-down.sql");
        }
    }
}
