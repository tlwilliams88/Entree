using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(36, "Added missing [List].[SaveRecentlyViewedHeaders] stored procedure.")]
    public class Migration0036 : BaseMigrationClass {
        public Migration0036() {
            MigrationNumber = "0036";
        }
    }
}
