using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(44, "bugfix contract change notifications")]
    public class Migration0044 : BaseMigration {
        public Migration0044() {
            MigrationNumber = "0044";
        }
    }
}
