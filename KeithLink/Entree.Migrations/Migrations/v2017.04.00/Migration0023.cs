using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(23, "New contract lists")]
    public class Migration0023 : Core.BaseMigrationClass {
        public Migration0023() {
            base.MigrationNumber = "0023";
        }
    }
}
