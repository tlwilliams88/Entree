using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(17, "Add config setting for public api tokens to accept and whether to serve public api")]
    public class Migration17 : Migration {
        public override void Up() {
            Execute.Script(@"SQL\v2017.2.0\up\appsettings\ValidPublicApiTokens_up.sql");
        }

        public override void Down() {
            Execute.Script(@"SQL\v2017.2.0\down\appsettings\ValidPublicApiTokens_down.sql");
        }
    }
}
