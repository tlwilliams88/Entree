﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Lists {
    public class CustomListShares : Migration {
        public override void Up() {
            Insert.IntoTable("CustomListShares")
                  .InSchema("List")
                  .Row(new {
                    BranchId = "FDF",
                    CustomerNumber = "600123",
                    HeaderId = 1,
                    Active = true,
                    CreatedUtc = new DateTime(2017, 6, 29, 9, 17, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 29, 9, 18, 0, DateTimeKind.Utc)
                  });
            Insert.IntoTable("CustomListShares")
                  .InSchema("List")
                  .Row(new {
                        BranchId = "FDF",
                        CustomerNumber = "600124",
                        HeaderId = 1,
                        Active = true,
                        CreatedUtc = new DateTime(2017, 6, 29, 9, 17, 0, DateTimeKind.Utc),
                        ModifiedUtc = new DateTime(2017, 6, 29, 9, 18, 0, DateTimeKind.Utc)
                    });
        }

        public override void Down() {
            // do nothing
        }
    }
}
