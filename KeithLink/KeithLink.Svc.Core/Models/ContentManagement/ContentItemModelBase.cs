using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Core.Models.ContentManagement
{
    [DataContract]
    public class ContentItemModelBase
    {
        [DataMember(Name="id")]
        public Int64 ContentItemId { get; set; }

        [DataMember(Name = "branchid")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BranchId { get; set; }
        
        [DataMember(Name = "tagline")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TagLine { get; set; }

        [DataMember(Name = "targeturltext")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TargetUrlText { get; set; }

        [DataMember(Name = "targeturl")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TargetUrl { get; set; }

        [DataMember(Name = "campaignid")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CampaignId { get; set; }

        [DataMember(Name = "content")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        [DataMember(Name = "iscontenthtml")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore,DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsContentHtml { get; set; }

        [DataMember(Name = "productid")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ProductId { get; set; }

        [DataMember(Name = "activesdatestart")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime ActiveDateStart { get; set; }

        [DataMember(Name = "activedateend")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime ActiveDateEnd { get; set; }
    }
}
