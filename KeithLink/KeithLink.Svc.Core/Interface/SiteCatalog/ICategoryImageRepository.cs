using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface ICategoryImageRepository
    {
        Models.SiteCatalog.CategoryImageReturn GetImageByCategory(string categoryId);
    }
}
