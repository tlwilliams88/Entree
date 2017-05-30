using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(30, "New inventory valuation lists")]
    public class Migration0030 : Core.BaseMigrationClass {
        public Migration0030() {
            base.MigrationNumber = "0030";
        }
    }
}
