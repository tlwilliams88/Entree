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
        List<Price> GetNonBekItemPrices(string branchId, string customerNumber, DateTime shipDate, string source, List<Product> products);

        List<Price> GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Product> products);
    }
}
