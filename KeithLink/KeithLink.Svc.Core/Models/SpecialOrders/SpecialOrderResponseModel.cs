using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.SpecialOrders
{
    [DataContract]
    public class SpecialOrderResponseModel : BaseQueueMessage
    {
        [DataMember(Name = "Header")]
        public ResponseHeader Header { get; set; }

        [DataMember(Name = "Item")]
        public ResponseItem Item { get; set; }
    }
}
