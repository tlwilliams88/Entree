using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(29, "New mandatoryitems lists")]
    public class Migration0029 : Core.BaseMigrationClass {
        public Migration0029() {
            base.MigrationNumber = "0029";
        }
    }
}
