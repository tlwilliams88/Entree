using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Dynamic;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "ProductsReturn")]
    public class ProductsReturn
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "totalcount")]
        public int TotalCount { get; set; }

        [DataMember(Name = "products")]
        public List<Product> Products { get; set; }

        [DataMember(Name = "facets")]
        public ExpandoObject Facets { get; set; }

        [DataMember(Name = "catalogCounts")]
        public Dictionary<string,int> CatalogCounts { get; set; }

        public ProductsReturn AddRange(ProductsReturn products)
        {
            this.Count += products.Count;
            this.TotalCount += products.TotalCount;
            this.Products.AddRange(products.Products);
            //this.Facets


            return this;
        }

    }
}
