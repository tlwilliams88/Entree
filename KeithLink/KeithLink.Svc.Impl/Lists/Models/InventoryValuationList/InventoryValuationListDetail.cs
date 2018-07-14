namespace Entree.Core.Lists.Models.InventoryValuationList {
    public class InventoryValuationListDetail : BaseListDetail {
        public bool Active { get; set; }
        public long? CustomInventoryItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Label { get; set; }
    }
}
