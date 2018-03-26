using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers
{
    public class RecommendedItemsModel
    {
        public string ItemNumber  { get; set; }

        public string RecommendedItem { get; set; }

        public decimal Confidence { get; set; }

        public int ContextKey { get; set; }

        public string PrimaryPriceListCode { get; set; }

        public string SecondaryPriceListCode { get; set; }

    }
}
