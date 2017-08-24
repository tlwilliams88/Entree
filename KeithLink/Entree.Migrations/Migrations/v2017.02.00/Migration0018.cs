using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations
{
    [Migration(18, "Add stored procedures to add or update catalog campaigns")]
    public class Migration0018 : Core.BaseMigrationClass
    {
        public Migration0018() {
            base.MigrationNumber = "0018";
        }
    }
}
