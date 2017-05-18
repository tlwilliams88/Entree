using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(15, "Add days to contract deleted items")]
    public class Migration0015 : Core.BaseMigrationClass
    {
        public Migration0015() {
            base.MigrationNumber = "0015";
        }
    }
}
