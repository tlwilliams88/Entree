using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Nest;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name="category")]
    public class Category
    {
        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        [ElasticProperty(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        [ElasticProperty(Name="description")]
        public string Description { get; set; }

        [DataMember(Name = "category_image")]
        [ElasticProperty(Name = "category_image")]
        public CategoryImage CategoryImage { get; set; }

        [DataMember(Name = "subcategories")]
        [ElasticProperty(Name="subcategories")]
        public SubCategory[] SubCategories { get; set; }
    }
}
