using Entree.Migrations.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations
{
    [Migration(9, "E&S Marketing campaigns schema creation")]
    public class Migration0009 : Core.BaseMigrationClass
    {
        public Migration0009() {
            base.MigrationNumber = "0009";
        }

    }
}
