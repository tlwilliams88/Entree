using Entree.Migrations.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentMigrator;

namespace Entree.Migrations.Migrations
{
    [Migration(10, "E&S Marketing campaigns schema creation")]
    public class Migration0010 : Core.BaseMigrationClass
    {
        public Migration0010() {
            base.MigrationNumber = "0010";
        }
    }
}
