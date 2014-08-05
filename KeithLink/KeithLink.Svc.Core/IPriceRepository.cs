using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Catalog;

namespace KeithLink.Svc.Core
{
    public interface IPriceRepository
    {
        PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Product> products);
    }
}
