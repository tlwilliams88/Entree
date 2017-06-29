namespace KeithLink.Svc.Core.Models.Lists.CustomList {
    public class CustomListDetail : BaseListDetail {
        public bool Active { get; set; }
        public long? CustomInventoryItemId { get; set; }
        public string Label { get; set; }
        public int LineNumber { get; set; }
        public decimal Par { get; set; }
    }
}
