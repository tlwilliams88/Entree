using Entree.Migrations.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentMigrator;

namespace Entree.Migrations.Migrations
{
    [Migration(10, "E&S Marketing campaigns schema creation")]
    public class Migration10 : Migration
    {
        public override void Up()
        {
            //this.Execute.Script(@"SQL\v1.12.2\up\Migration10-ES_Catalog_uri_name-up.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0010", "up")) {
                this.Execute.Script(script);
            }
        }

        public override void Down()
        {
            //this.Execute.Script(@"SQL\v1.12.2\down\Migration10-ES_Catalog_uri_name-down.sql");
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles("0010", "down")) {
                this.Execute.Script(script);
            }
        }

    }
}
