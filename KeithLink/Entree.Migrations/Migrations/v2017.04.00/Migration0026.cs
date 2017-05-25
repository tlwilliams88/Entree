using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(26, "Build and use new recentlyordered lists")]
    public class Migration0026 : Core.BaseMigrationClass {
        public Migration0026() {
            base.MigrationNumber = "0026";
        }
    }
}
