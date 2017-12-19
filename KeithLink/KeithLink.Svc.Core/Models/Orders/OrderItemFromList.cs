using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.Core.Models.Orders
{
    public class OrderItemFromList
    {
        public string ControlNumber { get; set; }
        public string ItemNumber { get; set; }
        public string SourceList { get; set; }
    }
}
