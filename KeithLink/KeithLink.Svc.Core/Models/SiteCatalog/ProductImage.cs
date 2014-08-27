using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name="ProductImage")]
    class ProductImage
    {
        #region properties
        [DataMember(Name="FileName")]
        public string FileName { get; set; }

        [DataMember(Name="Url")]
        public string Url { get; set; }
        #endregion
    }
}
