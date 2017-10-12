using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(46, "Added keywords field to elasticsearch and keywords ETL")]
    public class Migration0046 : BaseMigration {
        public Migration0046() {
            MigrationNumber = "0046";
        }
    }
}
