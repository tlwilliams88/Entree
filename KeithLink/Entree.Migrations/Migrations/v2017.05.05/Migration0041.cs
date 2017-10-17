using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(41, "Added updated branch support names")]
    public class Migration0041 : BaseMigration {
        public Migration0041() {
            MigrationNumber = "0041";
        }
    }
}
