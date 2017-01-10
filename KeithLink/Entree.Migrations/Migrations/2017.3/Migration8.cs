using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(8, "Modify the ETL stored procedure for reading the full item data by branch")]
    public class Migration8 : Migration {
        public override void Up() {
            Execute.Script(@"SQL\2017.3.0\up\stored procedures\ETL.ReadFullItemData.sql");
        }

        public override void Down() {
            Execute.Script(@"SQL\2017.3.0\down\stored procedures\ETL.ReadFullItemData.sql");
        }
    }
}
