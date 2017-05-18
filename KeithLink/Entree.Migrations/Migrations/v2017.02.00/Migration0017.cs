using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(17, "Add config setting for public api tokens to accept and whether to serve public api")]
    public class Migration0017 : Core.BaseMigrationClass {
        public Migration0017() {
            base.MigrationNumber = "0017";
        }
    }
}
