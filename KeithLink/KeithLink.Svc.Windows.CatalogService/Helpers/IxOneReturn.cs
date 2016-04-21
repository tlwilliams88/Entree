using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Windows.CatalogService.Helpers
{
    public class IxOneReturn
    {
        public IxOneReturn(string json)
        {
            JObject obj = JObject.Parse(json);
            JToken products = obj["Products"];
            Products = new List<IxOneProduct>();
            foreach(var prod in products.Children())
            {
                IxOneProduct product = new IxOneProduct();
                product.UPC = prod["UPC12"].ToString();
                JToken images = prod["StandardizedImage"];
                product.Filenames = new List<string>();
                foreach (var img in images.Children())
                {
                    product.Filenames.Add(img["OriginalFileName"].ToString());
                }
                Products.Add(product);
            }
        }
        public List<IxOneProduct> Products { get; set; }
    }
}
