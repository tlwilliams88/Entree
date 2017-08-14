using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Migrations.Core;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(39, "Fix settings for Contract Change Notices.")]
    public class Migration0039 : BaseMigrationClass {
        public Migration0039() {
            MigrationNumber = "0039";
        }
    }
}
