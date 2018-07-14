using Entree.Core.Models.Invoices;

namespace Entree.Core.Interface.Invoices {
    public interface ITermLogic {
        TermModel ReadTermInformation(string branchId, string termCode);
    }
}
