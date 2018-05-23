using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
    public class RecommendedItemsOrderedAnalyticsModel
    {
        public string CartId { get; set; }
        public string ControlNumber { get; set; }
        public string ItemNumber { get; set; }
        public string UnitOfMeasure { get; set; }
        public int OrderSourceId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public int ProductGroupingInsightKey { get; set; }
        public int CustomerInsightVersionKey { get; set; }
    }
}
