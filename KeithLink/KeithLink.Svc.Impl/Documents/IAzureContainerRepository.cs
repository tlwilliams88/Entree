using Entree.Core.Models.Documents;
using System.Collections.Generic;

namespace Entree.Core.Interface.Documents {
    public interface IAzureContainerRepository {
        List<DocumentReturnModel> GetAllDocuments(string identifier);
    }
}
