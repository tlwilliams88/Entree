using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine {
    [DataContract]
    public class ContentItem {
        #region ctor
        public ContentItem() {
            AdditionalData = new List<AdditionalData>();
        }
        #endregion

        #region properties
        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="url_title")]
        public string UrlTitle { get; set; }

        [DataMember(Name="entry_id")]
        public int EntryId { get; set; }

        [DataMember(Name="status")]
        public string Status { get; set; }

        [DataMember(Name="entry_date")]
        public string EntryDate { get; set; }

        [DataMember(Name="edit_date")]
        public string EditDate { get; set; }

        [DataMember(Name="epxiration_date")]
        public string ExpirationDate { get; set; }

        [DataMember(Name="food_featured_banner")]
        public string BannerUrl { get; set; }

        [DataMember(Name="food_summary")]
        public string Summary { get; set; }

        [DataMember(Name="additional_data")]
        public List<AdditionalData> AdditionalData { get; set; }
        #endregion
    }
}
