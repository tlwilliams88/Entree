using KeithLink.Svc.Core.Interface.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.InternalCatalog {
    /// <summary>
    /// Interface for dealing with the elasticsearch api
    /// </summary>
    public interface IElasticSearchRepository {
		bool CheckIfIndexExist(string branchId);
        void Create(string json);
		void CreateEmptyIndex(string branchId);
		void DeleteBranch(string branchId);
        /// <summary>
        /// Method for reading all the meta information from the documents within product type within the specified branch
        /// and return a list of the string ids of those documents
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        List<string> ReadListOfProductsByBranch(string branchId);
        void MapProductProperties(string branchId, string json);
		void RefreshSynonyms(string branchId);
    }
}
