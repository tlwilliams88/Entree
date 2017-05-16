using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(14, "Added last list ordered from joining information")]
    public class Migration14 : Migration {
        public override void Up() {
            //Execute.Script(@"SQL\v2017.2.0\up\appsettings\Configuration.AppSettings.sql");
            //Execute.Script(@"SQL\v2017.2.0\up\tables\Orders.OrderedFromList_up.sql");
            //Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Orders.ReadOrderListAssociation_up.sql");
            //Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Orders.WriteOrderListAssociation_up.sql");
            //Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Orders.DeleteOrderListAssociation_up.sql");
            //Execute.Script(@"SQL\v2017.2.0\up\stored procedures\Orders.PurgeOrderListAssociation_up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0014", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down() {
            //Execute.Script(@"SQL\v2017.2.0\down\tables\Orders.OrderedFromList_down.sql");
            //Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Orders.ReadOrderListAssociation_down.sql");
            //Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Orders.WriteOrderListAssociation_down.sql");
            //Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Orders.DeleteOrderListAssociation_down.sql");
            //Execute.Script(@"SQL\v2017.2.0\down\stored procedures\Orders.PurgeOrderListAssociation_down.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0014", "down")) {
                this.Execute.Script(script);
            }
        }
    }
}
