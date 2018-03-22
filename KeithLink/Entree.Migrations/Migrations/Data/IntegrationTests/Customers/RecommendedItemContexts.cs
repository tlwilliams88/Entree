using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Customers {
    [Profile("IntegrationTests")]
    public class RecommendedItemContext : Migration {
        public override void Up() {
            Insert.IntoTable("RecommendedItemContexts")
                  .InSchema("Customers")
                  .Row(new {
                      ContextKey = "1",
                      SIC = "1"
                  });
            Insert.IntoTable("RecommendedItemContexts")
                  .InSchema("Customers")
                  .Row(new
                  {
                      ContextKey = "2",
                      SIC = "2"
                  });
            Insert.IntoTable("RecommendedItemContexts")
                  .InSchema("Customers")
                  .Row(new
                  {
                      ContextKey = "3",
                      SIC = "3"
                  });
        }

        public override void Down() {
            // not used
        }
    }
}
