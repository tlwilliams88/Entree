using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(20, "Add branchsupport for FEL")]
    public class Migration0020 : Core.BaseMigrationClass {
        public Migration0020() {
            base.MigrationNumber = "0020";
        }
    }
}
