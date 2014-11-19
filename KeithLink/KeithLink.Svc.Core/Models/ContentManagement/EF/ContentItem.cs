using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.ContentManagement.EF
{
    public class ContentItem : BaseEFModel
    {
        [MaxLength(3)]
        public string BranchId { get; set; }

        [MaxLength(1024)]
        public string ImageUrl { get; set; }

        [MaxLength(256)]
        public string TagLine { get; set; }

        [MaxLength(256)]
        public string TargetUrlText { get; set; }

        [MaxLength(1024)]
        public string TargetUrl { get; set; }

        [MaxLength(1024)]
        public string CampaignId { get; set; }

        [MaxLength(1024)]
        public string Content { get; set; }

        [MaxLength(24)]
        public string ProductId { get; set; }

        public bool IsContentHtml { get; set; }
        public DateTime ActiveDateStart { get; set; }
        public DateTime ActiveDateEnd { get; set; }
    }
}
