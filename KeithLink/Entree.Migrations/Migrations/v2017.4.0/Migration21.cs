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
            Execute.Script(@"Scripts\AppData\List\Tables\HistoryHeader\HistoryHeader_Migration0021_v2017-04-00_up.sql");
            Execute.Script(@"Scripts\AppData\List\Tables\HistoryDetail\HistoryDetail_Migration0021_v2017-04-00_up.sql");
            Execute.Script(@"Scripts\AppData\ETL\Stored Procedures\ProcessWorksheetList\ProcessWorksheetList_Migration0021_v2017-04-00_up.sql");
        }

        public override void Down() {
            Execute.Script(@"Scripts\AppData\List\Tables\HistoryHeader\HistoryHeader_Migration0021_v2017-04-00_down.sql");
            Execute.Script(@"Scripts\AppData\List\Tables\HistoryDetail\HistoryDetail_Migration0021_v2017-04-00_down.sql");
            Execute.Script(@"Scripts\AppData\ETL\Stored Procedures\ProcessWorksheetList\ProcessWorksheetList_Migration0021_v2017-4-0_up.sql");
        }
    }
}
