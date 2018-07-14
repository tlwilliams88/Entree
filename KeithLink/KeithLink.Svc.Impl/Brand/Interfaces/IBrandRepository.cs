using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Brand.Models;

namespace Entree.Core.Brand.Interfaces
{
    public interface IBrandRepository
    {
        BrandsReturn GetHouseBrands();
    }
}
