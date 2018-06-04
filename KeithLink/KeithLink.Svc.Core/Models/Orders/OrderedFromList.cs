using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.Core.Models.Orders
{
    public class OrderedFromList
    {
        public string ControlNumber { get; set; }
        public long? ListId { get; set; }
        public ListType ListType { get; set; }
    }
}
