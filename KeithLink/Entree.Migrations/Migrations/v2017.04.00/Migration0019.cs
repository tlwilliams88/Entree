using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Helpers;

namespace Entree.Migrations
{
    [Migration(19, "Add stored procedures to add or update catalog campaigns")]
    public class Migration19 : Migration
    {
        public override void Up()
        {
            //Execute.Script(@"Scripts\AppData\List\Tables\ListItemsDelta\Migration0019_v2017-4-0_up.sql");
            //Execute.Script(@"Scripts\AppData\List\Stored Procedures\ReadNextContractListChange\Migration0019_v2017-4-0_up.sql");
            //Execute.Script(@"Scripts\AppData\List\Stored Procedures\UpdateSentOnContractListChangeById\Migration0019_v2017-4-0_up.sql");
            //Execute.Script(@"Scripts\AppData\List\Stored Procedures\PurgeContractListChanges\Migration0019_v2017-4-0_up.sql");
            //Execute.Script(@"Scripts\AppData\Configuration\Seed\AppSettings\header.sql");
            //Execute.Script(@"Scripts\AppData\Configuration\Seed\AppSettings\Migration0019_v2017-4-0_up.sql");
            //Execute.Script(@"Scripts\AppData\Configuration\Seed\AppSettings\footer.sql");
            //Execute.Script(@"Scripts\AppData\Configuration\Message Templates\Seed\Migration0019_2017-4-0_up.sql");
            //Execute.Script(@"Scripts\AppData\ETL\Stored Procedures\ProcessContractItemList\Migration0019_v2017-4-0_up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0019", "up"))
            {
                this.Execute.Script(script);
            }
        }

        public override void Down()
        {
            //Execute.Script(@"Scripts\AppData\List\Tables\ListItemsDelta\Migration0019_v2017-4-0_down.sql");
            //Execute.Script(@"Scripts\AppData\List\Stored Procedures\ReadNextContractListChange\Migration0019_v2017-4-0_down.sql");
            //Execute.Script(@"Scripts\AppData\List\Stored Procedures\UpdateSentOnContractListChangeById\Migration0019_v2017-4-0_down.sql");
            //Execute.Script(@"Scripts\AppData\List\Stored Procedures\PurgeContractListChanges\Migration0019_v2017-4-0_down.sql");
            //Execute.Script(@"Scripts\AppData\ETL\Stored Procedures\ProcessContractItemList\Migration0019_v2017-4-0_down.sql");
            //Execute.Script(@"Scripts\AppData\Configuration\Seed\Message Templates\Migration0019_v2017-4-0_down.sql");
            //Execute.Script(@"Scripts\AppData\Configuration\Seed\AppSettings\Migration0019_v2017-4-0_down.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0019", "down"))
            {
                this.Execute.Script(script);
            }
        }
    }
}
