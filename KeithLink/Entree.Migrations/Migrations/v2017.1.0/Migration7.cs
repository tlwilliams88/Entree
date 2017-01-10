using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(7, "Modify the ProcessContractItemList stored procedure to update the position on contract items")]
    public class Migration7: Migration {
        public override void Up() {
            Execute.Script(@"SQL\v2017.1.0\up\stored procedures\ETL.ProcessContractItemList.sql");
        }

        public override void Down() {
            Execute.Script(@"SQL\v2017.1.0\down\stored procedures\ETL.ProcessContractItemList.sql");
        }
    }
}
