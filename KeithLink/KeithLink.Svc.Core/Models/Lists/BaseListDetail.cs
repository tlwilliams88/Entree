namespace KeithLink.Svc.Core.Models.Lists {
    public abstract class BaseListDetail : AuditableEntity {
        public string CatalogId { get; set; }
        public bool? Each { get; set; }
        public long HeaderId { get; set; }
        public string ItemNumber { get; set; }
        public int LineNumber { get; set; }
    }
}
