using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(60, "Added keywords field to elasticsearch and keywords ETL")]
    public class Migration0060 : BaseMigration {
        public Migration0060() {
            MigrationNumber = "0060";
        }
    }
}
