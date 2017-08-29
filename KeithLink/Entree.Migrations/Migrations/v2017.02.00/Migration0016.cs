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
    [Migration(16, "Add config settings to exclude specific house brands")]
    public class Migration0016 : Core.BaseMigrationClass
    {
        public Migration0016() {
            base.MigrationNumber = "0016";
        }
    }
}
