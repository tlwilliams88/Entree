using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(21, "Build and use new history lists")]
    public class Migration21 : Migration {
        public override void Up() {
            Execute.Script(@"Scripts/AppData/List/Tables/ContractHeaders/2017.4.0/up.sql");
            Execute.Script(@"Scripts/AppData/List/Tables/ContractItems/2017.4.0/up.sql");
        }

        public override void Down() {
            Execute.Script(@"Scripts/AppData/List/Tables/ContractHeaders/2017.4.0/down.sql");
            Execute.Script(@"Scripts/AppData/List/Tables/ContractItems/2017.4.0/down.sql");
        }
    }
}
