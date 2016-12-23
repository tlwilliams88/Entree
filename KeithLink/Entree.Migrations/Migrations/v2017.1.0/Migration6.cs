using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations {
    [Migration(6, "Add the ETL staging table for PDM")]
    public class Migration6 : Migration {
        public override void Up() {
            Execute.Script(@"SQL\v2017.1.0\up\tables\Staging_PDM_EnrichedProducts.sql");
            Execute.Script(@"SQL\v2017.1.0\up\stored procedures\ReadPDMData.sql");
        }

        public override void Down() {
            Execute.Script(@"SQL\v2017.1.0\down\tables\Staging_PDM_EnrichedProducts.sql");
            Execute.Script(@"SQL\v2017.1.0\down\stored procedures\ReadPDMData.sql");
        }
    }
}
