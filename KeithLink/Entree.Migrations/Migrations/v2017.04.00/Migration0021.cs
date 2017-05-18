using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(21, "Build and use new history lists")]
    public class Migration0021 : Core.BaseMigrationClass {
        public Migration0021() {
            base.MigrationNumber = "0021";
        }
    }
}
