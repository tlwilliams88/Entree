using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Customers {
    [Profile("IntegrationTests")]
    public class RecommendedItems : Migration {
        public override void Up() {
            Insert.IntoTable("RecommendedItems")
                  .InSchema("Customers")
                  .Row(new {
                      ItemNumber = "111111",
                      RecommendedItem = "222221",
                      Confidence = .99,
                      ContextDescription = "1",
                      PrimaryPriceListCode = "1111",
                      SecondaryPriceListCode = "2222"
                  });
            Insert.IntoTable("RecommendedItems")
                  .InSchema("Customers")
                  .Row(new {
                      ItemNumber = "111112",
                      RecommendedItem = "222222",
                      Confidence = .97,
                      ContextDescription = "1",
                      PrimaryPriceListCode = "3333",
                      SecondaryPriceListCode = "4444"
                  });
            Insert.IntoTable("RecommendedItems")
                  .InSchema("Customers")
                  .Row(new {
                      ItemNumber = "111113",
                      RecommendedItem = "222223",
                      Confidence = .98,
                      ContextDescription = "1",
                      PrimaryPriceListCode = "5555",
                      SecondaryPriceListCode = "6666"
                  });
            Insert.IntoTable("RecommendedItems")
                  .InSchema("Customers")
                  .Row(new
                  {
                      ItemNumber = "111114",
                      RecommendedItem = "222224",
                      Confidence = .96,
                      ContextDescription = "1",
                      PrimaryPriceListCode = "5555",
                      SecondaryPriceListCode = "6666"
                  });
            Insert.IntoTable("RecommendedItems")
                  .InSchema("Customers")
                  .Row(new
                  {
                      ItemNumber = "111115",
                      RecommendedItem = "222225",
                      Confidence = .95,
                      ContextDescription = "1",
                      PrimaryPriceListCode = "5555",
                      SecondaryPriceListCode = "6666"
                  });
            Insert.IntoTable("RecommendedItems")
                  .InSchema("Customers")
                  .Row(new
                  {
                      ItemNumber = "111116",
                      RecommendedItem = "222226",
                      Confidence = .94,
                      ContextDescription = "1",
                      PrimaryPriceListCode = "5555",
                      SecondaryPriceListCode = "6666"
                  });
        }

        public override void Down() {
            // not used
        }
    }
}
