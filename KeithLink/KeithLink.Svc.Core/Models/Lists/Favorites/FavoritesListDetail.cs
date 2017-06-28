namespace KeithLink.Svc.Core.Models.Lists.Favorites {
    public class FavoritesListDetail : BaseListDetail {
        public bool Active { get; set; }
        public string Label { get; set; }
        public int LineNumber { get; set; }
    }
}
