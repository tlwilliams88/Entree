﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentMigrator;

namespace Entree.Migrations.Migrations
{
    [Migration(9, "E&S Marketing campaigns schema creation")]
    public class Migration9 : Migration
    {
        public override void Up()
        {
            this.Execute.Script(@"SQL\v1.12.2\up\Migration9-ES_Catalog_schema-up.sql");
        }

        public override void Down()
        {
            this.Execute.Script(@"SQL\v1.12.2\down\Migration9-ES_Catalog_schema-down.sql");
        }

    }
}