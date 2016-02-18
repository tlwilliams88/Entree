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
    [DataContract(Name = "specialorderresponse")]
    public class SpecialOrderResponseModel : BaseQueueMessage
    {
        [DataMember(Name = "header")]
        public ResponseHeader Header { get; set; }

        [DataMember(Name = "item")]
        public ResponseItem Item { get; set; }
    }
}
