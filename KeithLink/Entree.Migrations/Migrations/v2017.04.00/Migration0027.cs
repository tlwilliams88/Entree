using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(27, "New recommendeditems lists")]
    public class Migration0027 : Core.BaseMigrationClass {
        public Migration0027() {
            base.MigrationNumber = "0027";
        }
    }
}
