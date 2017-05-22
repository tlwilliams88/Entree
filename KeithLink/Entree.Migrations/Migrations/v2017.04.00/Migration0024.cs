using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(24, "Build and use new notes lists")]
    public class Migration0024 : Core.BaseMigrationClass {
        public Migration0024() {
            base.MigrationNumber = "0024";
        }
    }
}
