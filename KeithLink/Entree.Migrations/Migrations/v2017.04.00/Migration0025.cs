using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(25, "New recentlyviewed lists")]
    public class Migration0025 : Core.BaseMigrationClass {
        public Migration0025() {
            base.MigrationNumber = "0025";
        }
    }
}
