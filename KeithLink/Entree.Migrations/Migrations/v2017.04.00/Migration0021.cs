using Entree.Migrations.Helpers;

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
            //Execute.Script(@"Scripts\AppData\List\Tables\HistoryHeader\HistoryHeader_0021_2017-04-00_up.sql");
            //Execute.Script(@"Scripts\AppData\List\Tables\HistoryDetail\HistoryDetail_0021_2017-04-00_up.sql");
            //Execute.Script(@"Scripts\AppData\ETL\Stored Procedures\ProcessWorksheetList\ProcessWorksheetList_0021_2017-04-00_up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0021", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down() {
            //Execute.Script(@"Scripts\AppData\List\Tables\HistoryHeader\HistoryHeader_0021_2017-04-00_down.sql");
            //Execute.Script(@"Scripts\AppData\List\Tables\HistoryDetail\HistoryDetail_0021_2017-04-00_down.sql");
            //Execute.Script(@"Scripts\AppData\ETL\Stored Procedures\ProcessWorksheetList\ProcessWorksheetList_0021_2017-04-00_up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0021", "down")) {
                this.Execute.Script(script);
            }
        }
    }
}
