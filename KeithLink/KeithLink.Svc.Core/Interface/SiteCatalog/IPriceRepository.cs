using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface IPriceRepository
    {
        PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Product> products);
    }
}
