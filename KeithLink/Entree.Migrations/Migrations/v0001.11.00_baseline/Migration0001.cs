using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(1, "Baseline schema and data migrations for the new database versioning tool. Matches v1.11")]
    public class Migration0001 : Core.BaseMigrationClass {

        public Migration0001() {
            base.MigrationNumber = "0001";
            base.FilterContext = "BaselineSetup";
        }
    }
}
