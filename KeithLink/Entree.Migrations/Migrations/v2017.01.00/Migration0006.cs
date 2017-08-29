using Entree.Migrations.Helpers;

using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(6, "Add the ETL staging table for PDM")]
    public class Migration0006 : Core.BaseMigrationClass {
        public Migration0006() {
            base.MigrationNumber = "0006";
        }
    }
}
