using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Entree.Core.SiteCatalog.Models
{
    [DataContract]
    public class ProductImageReturn
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
