using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Marketing
{
    [DataContract]
    public class CatalogCampaignHeader
    {
        public Int64 Id { get; set; }
        [DataMember(Name = "uri")]
        public string Uri { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "active")]
        public bool Active { get; set; }
        [DataMember(Name = "startdate")]
        public DateTime StartDate { get; set; }
        [DataMember(Name = "enddate")]
        public DateTime EndDate { get; set; }
        [DataMember(Name = "items")]
        public List<CatalogCampaignItem> Items { get; set; }
    }
}
