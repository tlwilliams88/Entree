using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core
{
    [DataContract(Name="category")]
    public class Category
    {
        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "subcategories")]
        public Category[] SubCategories { get; set; }
    }

    [DataContract(Name="CategoriesReturn")]
    public class CategoriesReturn
    {
        [DataMember(Name="categories")]
        public List<Category> Categories { get; set; }
    }
}
