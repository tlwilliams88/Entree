using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(16, "Add config settings to exclude specific house brands")]
    public class Migration16 : Migration {
        public override void Up() {
            //Execute.Script(@"SQL\v2017.2.0\up\appsettings\BrandsToExclude_up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0016", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down() {
            //Execute.Script(@"SQL\v2017.2.0\down\appsettings\BrandsToExclude_down.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0016", "down")) {
                this.Execute.Script(script);
            }
        }
    }
}
