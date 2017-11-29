using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase;
using FluentMigrator;

namespace Entree.Migrations {
    [Migration(61, "Added respect for StartDate, EndDate, and Active flags in CatalogCampaignHeader procs")]
    public class Migration0061 : BaseMigration {
        public Migration0061() {
            MigrationNumber = "0061";
        }
    }
}
