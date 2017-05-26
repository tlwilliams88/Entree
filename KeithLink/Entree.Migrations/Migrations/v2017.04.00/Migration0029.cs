using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(29, "Build and use new mandatoryitems lists")]
    public class Migration0029 : Core.BaseMigrationClass {
        public Migration0029() {
            base.MigrationNumber = "0029";
        }
    }
}
