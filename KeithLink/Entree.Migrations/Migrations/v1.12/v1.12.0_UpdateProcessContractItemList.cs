using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(5, "Update ETL.ProcessContractItemList stored procedure")]
    public class UpdateProcessContractItemList : Migration {
        public override void Up() {
            this.Execute.Script(@"SQL\v1.12.0\v1.12.0-ProcessContractItemList-up.sql");
        }

        public override void Down() {
            this.Execute.Script(@"SQL\v1.12.0\v1.12.0-ProcessContractItemList-down.sql");
        }
    }
}
