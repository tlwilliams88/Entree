﻿using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(27, "Build and use new recommendeditems lists")]
    public class Migration0027 : Core.BaseMigrationClass {
        public Migration0027() {
            base.MigrationNumber = "0027";
        }
    }
}
