using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract]
    class ProductImageReturn
    {
        #region ctor
        public ProductImageReturn()
        {
            ProductImages = new List<ProductImage>();
        }
        #endregion

        #region properties
        [DataMember(Name="ProductImages")]
        public List<ProductImage> ProductImages { get; set; }
        #endregion
    }
}
