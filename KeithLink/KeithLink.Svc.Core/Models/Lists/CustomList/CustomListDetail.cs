using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.CustomList
{
    public class CustomListDetail
    {
        public long Id { get; set; }
        public string ItemNumber { get; set; }
        public bool? Each { get; set; }
        public string CatalogId { get; set; }
        public decimal Par { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
