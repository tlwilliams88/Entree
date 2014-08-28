using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface IProductImageRepository
    {
        Models.SiteCatalog.ProductImageReturn GetImageList(string itemNumber);
    }
}
