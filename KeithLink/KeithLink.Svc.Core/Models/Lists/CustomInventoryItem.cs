using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
    [Serializable]
    public class CustomInventoryItem
    {
        public string ItemNumber { get; set; }
        public string Name { get; set; }
        public string Pack { get; set; }
        public string Size { get; set; }
        public string Vendor { get; set; }
        public bool Each { get; set; }
        public string CasePrice { get; set; }
        public string PackagePrice { get; set; }
    }
}
