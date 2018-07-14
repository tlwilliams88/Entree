using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.SiteCatalog
{
    public interface ICategoryImageRepository
    {
        Models.SiteCatalog.CategoryImageReturn GetImageByCategory(string categoryId);
    }
}
