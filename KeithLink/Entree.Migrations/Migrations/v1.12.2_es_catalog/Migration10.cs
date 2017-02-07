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
            this.Execute.Script(@"SQL\v1.12.2\up\Migration10-ES_Catalog_uri_name-up.sql");
        }

        public override void Down()
        {
            this.Execute.Script(@"SQL\v1.12.2\down\Migration10-ES_Catalog_uri_name-down.sql");
        }

    }
}
