using KeithLink.Svc.Core.Models.Customers;

namespace KeithLink.Svc.Core.Interface.Customers
{
    public interface IMinimumOrderAmountRepository {
        MinimumOrderAmountModel GetMinimumOrderAmount(string customerNumber, string branchId);
    }
}
