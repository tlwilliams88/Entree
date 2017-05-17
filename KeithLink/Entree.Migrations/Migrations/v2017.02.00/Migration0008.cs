using Entree.Migrations.Helpers;

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
            //Execute.Script(@"SQL\v2017.2.0\up\stored procedures\ETL.ReadFullItemData_up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0008", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down() {
            //Execute.Script(@"SQL\v2017.2.0\down\stored procedures\ETL.ReadFullItemData_down.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0008", "down")) {
                this.Execute.Script(script);
            }
        }
    }
}
