using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(40, "Add missing setting and stored procedure for contract change notifications.")]
    public class Migration0040 : BaseMigrationClass {
        public Migration0040() {
            MigrationNumber = "0040";
        }
    }
}
