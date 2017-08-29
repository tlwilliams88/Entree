using Entree.Migrations.Core;
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
    [Migration(4, "Add label column to custom inventory list")]
    public class Migration0004 : BaseMigration
    {
        public Migration0004() {
            base.MigrationNumber = "0004";
        }
    }
}
