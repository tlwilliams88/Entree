using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(39, "Added missing delete stored procedures for flipping active flag on lists with logical deletes")]
    public class Migration0039 : BaseMigrationClass {
        public Migration0039() {
            MigrationNumber = "0039";
        }
    }
}
