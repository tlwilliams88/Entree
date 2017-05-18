using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(12, "change_etl.readproprietaryitems_to_add_division")]
    public class Migration0012: Core.BaseMigrationClass {
        public Migration0012() {
            base.MigrationNumber = "0012";
        }
    }
}
