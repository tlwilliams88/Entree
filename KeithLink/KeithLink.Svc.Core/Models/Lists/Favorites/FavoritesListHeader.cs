using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.Favorites
{
    public class FavoritesListHeader
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string CustomerNumber { get; set; }
        public string BranchId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
        public List<FavoritesListDetail> Items { get; set; }
    }
}
