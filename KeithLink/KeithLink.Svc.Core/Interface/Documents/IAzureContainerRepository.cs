using KeithLink.Svc.Core.Models.Documents;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Documents {
    public interface IAzureContainerRepository {
        List<DocumentReturnModel> GetAllDocuments(string identifier);
    }
}
