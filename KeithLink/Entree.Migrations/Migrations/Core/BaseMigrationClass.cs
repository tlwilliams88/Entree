using Entree.Migrations.Helpers;

using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations.Core
{
    public abstract class BaseMigrationClass : Migration
    {
        #region constructor
        public BaseMigrationClass() {
        }
        #endregion

        #region functions
        public override void Up() {
            if (FilterContext != null) {
                if (this.ApplicationContext != null && this.ApplicationContext.Equals(FilterContext)) {
                    RunScripts("up");
                }
            } else {
                RunScripts("up");
            }
        }

        public override void Down() {
            if (FilterContext != null) {
                if (this.ApplicationContext != null && this.ApplicationContext.Equals(FilterContext)) {
                    RunScripts("down");
                }
            } else {
                RunScripts("down");
            }
        }

        private void RunScripts(string direction) {
            foreach (string script in ScriptsCollectionHelper.GetAllMigrationFiles(MigrationNumber, direction)) {
                this.Execute.Script(script);
            }
        }
        #endregion

        #region properties
        public string MigrationNumber { get; set; }
        public string FilterContext { get; set; }
        #endregion
    }
}
