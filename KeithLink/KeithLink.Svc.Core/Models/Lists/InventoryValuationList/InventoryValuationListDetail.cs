using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.InventoryValuationList
{
    public class InventoryValuationListDetail
    {
        public long Id { get; set; }
        public long ParentInventoryValuationListHeaderId { get; set; }
        public long? CustomInventoryItemId { get; set; }
        public string ItemNumber { get; set; }
        public bool? Each { get; set; }
        public decimal Quantity { get; set; }
        public string CatalogId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
