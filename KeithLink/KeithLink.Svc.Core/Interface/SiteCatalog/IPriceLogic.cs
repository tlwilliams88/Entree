using System;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface IPriceLogic
    {
        PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Product> products);
    }
}
