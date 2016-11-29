using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations.v1_11 {
    [Migration(1, "Baseline schema and data migrations for the new database versioning tool. Matches v1.11")]
    public class Baseline : Migration {

        public override void Up() {
            // Only run this if the baseline needs scaffolded
            if (this.ApplicationContext != null && this.ApplicationContext.Equals("BaselineSetup")) {
                this.Execute.Script(@"SQL\v1.11_Baseline\v1.11_BEK_Commerce_AppData.sql");
            }
        }

        public override void Down() {
            throw new NotImplementedException();
        }
    }
}
