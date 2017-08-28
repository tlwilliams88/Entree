using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(43, "Label field added to InventoryValuationListDetails, also added migration script for old IVRs that are missing data.")]
    public class Migration0043 : BaseMigration {
        public Migration0043() {
            MigrationNumber = "0043";
        }
    }
}
