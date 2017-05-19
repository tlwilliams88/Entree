using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(22, "Build and use new favorite lists")]
    public class Migration0022 : Core.BaseMigrationClass {
        public Migration0022() {
            base.MigrationNumber = "0022";
        }
    }
}
