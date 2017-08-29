using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(42, "Fixed notes migration")]
    public class Migration0042 : BaseMigration {
        public Migration0042() {
            MigrationNumber = "0042";
        }
    }
}
