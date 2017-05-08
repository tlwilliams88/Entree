using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Marketing
{
    [DataContract]
    public class CatalogCampaignItem
    {
        public Int64 Id { get; set; }
        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }
        public Int64 CatalogCampaignHeaderId { get; set; }
        [DataMember(Name = "active")]
        public bool Active { get; set; }
    }
}
