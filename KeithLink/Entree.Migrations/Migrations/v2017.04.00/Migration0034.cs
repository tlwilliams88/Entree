using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(34, "add listtype to what can be saved from a cart")]
    public class Migration0034 : BaseMigration {
        public Migration0034() {
            MigrationNumber = "0034";
        }
    }
}
