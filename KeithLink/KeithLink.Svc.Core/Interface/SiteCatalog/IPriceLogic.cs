using System;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface IPriceLogic
    {
        PriceReturn GetNonBekItemPrices(string branchId, string customerNumber, string source, DateTime shipDate, IEnumerable<Product> products);

        PriceReturn GetPrices(string branchId, string customerNumber, DateTime shipDate, IEnumerable<Product> products);

        Price GetPrice(string branchId, string customerNumber, DateTime shipDate, Product product);
    }
}
