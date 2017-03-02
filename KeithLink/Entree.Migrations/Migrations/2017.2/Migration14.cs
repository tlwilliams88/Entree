using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(14, "Modify the ETL stored procedure for reading the full item data by branch")]
    public class Migration14 : Migration {
        public override void Up() {
            Execute.Script(@"SQL\2017.2.0\up\appsettings\Configuration.AppSettings.sql");
            Execute.Script(@"SQL\2017.2.0\up\tables\Orders.Order2List.sql");
            Execute.Script(@"SQL\2017.2.0\up\stored procedures\Orders.ReadOrderListAssociation.sql");
            Execute.Script(@"SQL\2017.2.0\up\stored procedures\Orders.WriteOrderListAssociation.sql");
            Execute.Script(@"SQL\2017.2.0\up\stored procedures\Orders.PurgeOrderListAssociation.sql");
        }

        public override void Down() {
        }
    }
}
