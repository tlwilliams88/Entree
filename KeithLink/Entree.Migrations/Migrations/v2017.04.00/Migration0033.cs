using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(33, "add indexes, change stored procedures for lists")]
    public class Migration0033 : BaseMigrationClass {
        public Migration0033() {
            MigrationNumber = "0033";
        }
    }
}
