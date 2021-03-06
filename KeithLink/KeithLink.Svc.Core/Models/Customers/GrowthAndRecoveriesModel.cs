﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Models.Customers
{
    public class GrowthAndRecoveriesModel
    {
        public decimal Amount { get; set; }

        public string BranchId { get; set; }

        public int CustomerInsightVersionKey { get; set; }

        public string GroupingCode { get; set; }

        public string GroupingDescription { get; set; }

        public int GrowthAndRecoveryProductGroup { get; set; }

        public GrowthAndRecoveryType GrowthAndRecoveryTypeKey { get; set; }

        public int Id { get; set; }

        public int ProductGroupingInsightKey { get; set; }

    }
}
