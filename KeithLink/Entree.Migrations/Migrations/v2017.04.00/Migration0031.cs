using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(31, "New custom lists")]
    public class Migration0031 : Core.BaseMigrationClass {
        public Migration0031() {
            base.MigrationNumber = "0031";
        }
    }
}
