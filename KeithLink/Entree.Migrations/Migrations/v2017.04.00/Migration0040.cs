using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(40, "Add missing setting and stored procedure for contract change notifications.")]
    public class Migration0040 : BaseMigration {
        public Migration0040() {
            MigrationNumber = "0040";
        }
    }
}
