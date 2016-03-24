using KeithLink.Svc.Core.Models.Invoices;

namespace KeithLink.Svc.Core.Interface.Invoices {
    public interface ITermLogic {
        TermModel ReadTermInformation(string branchId, string termCode);
    }
}
