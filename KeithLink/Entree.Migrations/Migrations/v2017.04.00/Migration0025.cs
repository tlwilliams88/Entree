﻿using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(25, "Build and use new recentlyviewed lists")]
    public class Migration0025 : Core.BaseMigrationClass {
        public Migration0025() {
            base.MigrationNumber = "0025";
        }
    }
}
