using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(7, "Modify the ProcessContractItemList stored procedure to update the position on contract items")]
    public class Migration0007: Core.BaseMigrationClass {
        public Migration0007() {
            base.MigrationNumber = "0007";
        }
    }
}
