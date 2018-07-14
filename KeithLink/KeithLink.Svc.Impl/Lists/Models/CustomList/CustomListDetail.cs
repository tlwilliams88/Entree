namespace Entree.Core.Lists.Models.CustomList {
    public class CustomListDetail : BaseListDetail {
        public bool Active { get; set; }
        public long? CustomInventoryItemId { get; set; }
        public string Label { get; set; }
        public decimal Par { get; set; }
    }
}
