using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(32, "New custom list shares")]
    public class Migration0032 : Core.BaseMigrationClass {
        public Migration0032() {
            base.MigrationNumber = "0032";
        }
    }
}
