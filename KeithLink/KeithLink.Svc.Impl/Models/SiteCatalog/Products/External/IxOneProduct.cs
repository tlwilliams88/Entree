using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models.SiteCatalog.Products.External
{
    public class IxOneProduct
    {
        public string UPC { get; set; }
        public List<string> Filenames { get; set; }
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(UPC + ": ");
            foreach(string file in Filenames)
            {
                ret.Append(file + ", ");
            }
            return ret.ToString();
        }
    }
}
