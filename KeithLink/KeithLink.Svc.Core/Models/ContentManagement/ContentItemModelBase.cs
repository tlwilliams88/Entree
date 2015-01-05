using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.ContentManagement
{
    [DataContract]
    public class ContentItemModelBase
    {
        [DataMember(Name="id")]
        public Int64 ContentItemId { get; set; }

        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }
        
        [DataMember(Name = "tagline")]
        public string TagLine { get; set; }

        [DataMember(Name = "targeturltext")]
        public string TargetUrlText { get; set; }

        [DataMember(Name = "targeturl")]
        public string TargetUrl { get; set; }

        [DataMember(Name = "campaignid")]
        public string CampaignId { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "iscontenthtml")]
        public bool IsContentHtml { get; set; }

        [DataMember(Name = "productid")]
        public string ProductId { get; set; }

        [DataMember(Name = "activesdatestart")]
        public DateTime ActiveDateStart { get; set; }

        [DataMember(Name = "activedateend")]
        public DateTime ActiveDateEnd { get; set; }
    }
}
