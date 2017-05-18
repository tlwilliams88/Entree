using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations
{
    [Migration(11, "Modify the ProcessContractItemList stored procedure to update the position on contract items")]
    public class Migration0011 : Core.BaseMigrationClass
    {
        public Migration0011() {
            base.MigrationNumber = "0011";
        }
    }
}
