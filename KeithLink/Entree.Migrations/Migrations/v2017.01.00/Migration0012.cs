using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(12, "change_etl.readproprietaryitems_to_add_division")]
    public class Migration12: Migration {
        public override void Up() {
            //Execute.Script(@"SQL\v2017.1.0\up\stored procedures\ETL.ReadProprietaryItems.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0012", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down() {
            //Execute.Script(@"SQL\v2017.1.0\down\stored procedures\ETL.ReadProprietaryItems.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0012", "down")) {
                this.Execute.Script(script);
            }
        }
    }
}
