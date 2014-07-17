using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core
{
    [Serializable]
    public class Product
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string ProductId { get; set; }
        public string Name { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "price")]
        public decimal Price { get; set; }
        [DataMember(Name = "weight")]
        public string Weight { get; set; }
        public string Description { get; set; }
        public string Pack { get; set; }
        public string Size { get; set; }
        public string MfrNumber { get; set; }
        public string MfrName { get; set; }
        public string UPC { get; set; }
        public string Brand { get; set; }
    }
}
