using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(28, "Build and use new reminder lists")]
    public class Migration0028 : Core.BaseMigrationClass {
        public Migration0028() {
            base.MigrationNumber = "0028";
        }
    }
}
