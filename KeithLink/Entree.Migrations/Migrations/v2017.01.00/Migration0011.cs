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
    public class Migration11 : Migration
    {
        public override void Up()
        {
            //Execute.Script(@"SQL\v2017.1.0\up\templates\Configuration.MessageTemplates-AddSpecialOrderConfirmations.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0011", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down()
        {
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0011", "down")) {
                this.Execute.Script(script);
            }
        }
    }
}
