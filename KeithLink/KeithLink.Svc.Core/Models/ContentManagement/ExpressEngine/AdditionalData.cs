using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine {
    [DataContract]
    public class AdditionalData {
        #region ctor
        public AdditionalData() {
            ProductId = new List<Product>();
        }
        #endregion

        #region property
        [DataMember(Name="business")]
        public string Business { get; set; }

        [DataMember(Name="category")]
        public string Category { get; set; }

        [DataMember(Name="target_branch")]
        public string TargetBranch { get; set; }

        [DataMember(Name="product_id")]
        public List<Product> ProductId { get; set; }

        [DataMember(Name="brand")]
        public string Brand { get; set; }

        [DataMember(Name="campaign_id")]
        public string CampaignId { get; set; }
        #endregion
    }
}
