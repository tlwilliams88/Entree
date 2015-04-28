using System;
using System.Runtime.Serialization;


namespace KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine {
    [DataContract]
    public class Product {
        [DataMember(Name="item_number")]
        public string ItemNumber { get; set; }

        [DataMember(Name="branch_id")]
        public string BranchId { get; set; }
    }
}
