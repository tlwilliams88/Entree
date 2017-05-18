using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(8, "Modify the ETL stored procedure for reading the full item data by branch")]
    public class Migration0008 : Core.BaseMigrationClass {
        public Migration0008() {
            base.MigrationNumber = "0008";
        }
    }
}
