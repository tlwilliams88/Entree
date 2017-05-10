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
            Execute.Script(@"SQL\v2017.2.0\up\stored procedures\ETL.ProcessContractItemList_up.sql");
        }

        public override void Down() {
            Execute.Script(@"SQL\v2017.2.0\down\stored procedures\ETL.ProcessContractItemList_down.sql");
        }
    }
}
