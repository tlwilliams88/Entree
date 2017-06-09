using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.Favorites
{
    public class FavoritesListDetail
    {
        public long Id { get; set; }
        public long ParentFavoritesHeaderId { get; set; }
        public string ItemNumber { get; set; }
        public bool? Each { get; set; }
        public string Label { get; set; }
        public string CatalogId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
