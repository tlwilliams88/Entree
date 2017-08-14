using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(41, "Modify how contract change notifications work to simplify.")]
    public class Migration0041 : BaseMigrationClass {
        public Migration0041() {
            MigrationNumber = "0041";
        }
    }
}
