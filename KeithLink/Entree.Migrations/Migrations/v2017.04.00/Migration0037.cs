using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(37, "Added missing [List].[DeleteRecommendedItemDetails] stored procedure.")]
    public class Migration0037 : BaseMigrationClass {
        public Migration0037() {
            MigrationNumber = "0037";
        }
    }
}
