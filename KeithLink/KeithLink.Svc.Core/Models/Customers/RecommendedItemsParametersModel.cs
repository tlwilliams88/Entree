using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers
{
    public class RecommendedItemsParametersModel
    {
        public int CallSize { get; set; }

        public int SkipSize { get; set; }

        public string CustomerNumber { get; set; }

        public string BranchId { get; set; }

        public List<string> CartItemsList { get; set; }
    }
}
