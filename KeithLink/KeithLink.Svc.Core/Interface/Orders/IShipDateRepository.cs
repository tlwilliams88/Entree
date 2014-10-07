namespace KeithLink.Svc.Core.Interface.Orders {
    public interface IShipDateRepository {
        string GetShipDates(string branchId, string customerNumber);
    }
}
