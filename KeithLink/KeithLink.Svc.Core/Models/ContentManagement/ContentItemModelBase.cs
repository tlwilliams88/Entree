using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ContentManagement
{
    public class ContentItemModelBase
    {
        public Int64 ContentItemId { get; set; }
        public string BranchId { get; set; }
        public string TagLine { get; set; }
        public string TargetUrlText { get; set; }
        public string TargetUrl { get; set; }
        public string CampaignId { get; set; }
        public string Content { get; set; }
        public bool IsContentHtml { get; set; }
        public DateTime ActiveDateStart { get; set; }
        public DateTime ActiveDateEnd { get; set; }
    }
}
