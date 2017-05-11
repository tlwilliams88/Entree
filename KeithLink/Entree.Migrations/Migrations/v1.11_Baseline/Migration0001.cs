using Entree.Migrations.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(1, "Baseline schema and data migrations for the new database versioning tool. Matches v1.11")]
    public class Migration0001 : Migration {

        private const string APP_DATA = "AppData";
        private const string 

        public override void Up() {
            // Only run this if the baseline needs scaffolded
            if (this.ApplicationContext != null && this.ApplicationContext.Equals("BaselineSetup")) {
                
                foreach (string file in ScriptsCollectionHelper.GetAllMigrationFiles("0001")) {
                    this.Execute.Script(file);
                }

            }
        }

        public override void Down() {
            throw new NotImplementedException();
        }
    }
}
