using System.Collections.Generic;
using KeithLink.Svc.Impl.Repository.Documents;
using KeithLink.Svc.Core.Interface.Documents;
using KeithLink.Svc.Impl.Repository.DataConnection;
using KeithLink.Svc.Core.Interface.DataConnection;
using KeithLink.Svc.Core.Models.Documents;

namespace KeithLink.Svc.Impl.Repository.Documents {
    public class AzureContainerRepositoryImpl : IAzureContainerRepository {
        #region attributes
        private IAzureContainerConnection _docRepository;
        #endregion

        #region ctor
        public AzureContainerRepositoryImpl(IAzureContainerConnection connection) {
            _docRepository = connection;
        }
        #endregion

        #region methods        
        public List<DocumentReturnModel> GetAllDocuments(string identifier) {
            return _docRepository.GetDocuments("bekblob", "hns/", identifier);
        }
        #endregion
    }
}
