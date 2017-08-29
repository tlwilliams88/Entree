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
    [Migration(3, "Added CustomInventoryId Column to ListItems")]
    public class Migration0003 : Core.BaseMigrationClass
    {
        public Migration0003() {
            base.MigrationNumber = "0003";
        }
    }
}
