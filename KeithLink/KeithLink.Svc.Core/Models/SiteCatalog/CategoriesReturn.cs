using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Nest;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name="CategoriesReturn")]
    public class CategoriesReturn
    {
        [DataMember(Name = "categories")]
        [ElasticProperty(Name = "categories")]
        public List<Category> Categories { get; set; }
    }
}
