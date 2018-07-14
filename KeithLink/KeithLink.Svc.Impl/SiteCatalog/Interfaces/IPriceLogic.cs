using System;
using System.Collections.Generic;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.SiteCatalog
{
    public interface IPriceLogic
    {
        PriceReturn GetNonBekItemPrices(string branchId, string customerNumber, string source, DateTime shipDate, List<Product> products);

        PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Product> products);
    }
}
