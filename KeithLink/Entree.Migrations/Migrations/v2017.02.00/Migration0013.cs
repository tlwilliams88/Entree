using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(13, "Added ETL Staging tables for departments")]
    public class Migration13 : Migration {
        public override void Up() {
            //Execute.Script(@"SQL\v2017.2.0\up\tables\ETL.Staging_Departments_up.sql");
            //Execute.Script(@"SQL\v2017.2.0\up\stored procedures\ETL.ReadAllDepartments_up.sql");
            //Execute.Script(@"SQL\v2017.2.0\up\stored procedures\ETL.ReadFullItemData_up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0013", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down() {
            //Execute.Script(@"SQL\v2017.2.0\down\tables\ETL.Staging_Departments_down.sql");
            //Execute.Script(@"SQL\v2017.2.0\down\stored procedures\ETL.ReadAllDepartments_down.sql");
            //Execute.Script(@"SQL\v2017.2.0\down\stored procedures\ETL.ReadFullItemData_down.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0013", "down")) {
                this.Execute.Script(script);
            }
        }
    }
}
