using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(38, "Added missing [List].[SaveNotesHeader] stored procedure.")]
    public class Migration0038 : BaseMigrationClass {
        public Migration0038() {
            MigrationNumber = "0038";
        }
    }
}
