using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(20, "Add branchsupport for FEL")]
    public class Migration20 : Migration {
        public override void Up() {
            Execute.Script(@"SQL\v2017.3.0\up\branchsupports\Branchsupports.Branchsupport_FEL_up.sql");
        }

        public override void Down() {
            Execute.Script(@"SQL\v2017.3.0\down\branchsupports\Branchsupports.Branchsupport_FEL_down.sql");
        }
    }
}
