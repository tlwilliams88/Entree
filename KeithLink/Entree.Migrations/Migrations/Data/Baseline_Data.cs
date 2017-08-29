using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations.Data {
    [Migration(2, "Baseline seed data")]
    public class Baseline : Migration {
        public override void Up() {
            // Only run this if the baseline needs scaffolded
            if (this.ApplicationContext != null && this.ApplicationContext.Equals("BaselineSetup")) {
                this.Execute.Script(@"SQL\v1.11_Baseline\v1.11_BEK_Commerce_AppData_Seed.sql");
                this.Execute.Script(@"SQL\Messaging\Configuration.MessageTemplates.Bootstrap.sql");
            }
        }

        public override void Down() {
            //throw new NotImplementedException();
        }
    }
}
