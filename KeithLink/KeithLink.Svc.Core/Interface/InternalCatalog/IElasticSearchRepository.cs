using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.InternalCatalog {
    public interface IElasticSearchRepository {
		bool CheckIfIndexExist(string branchId);
        void Create(string json);
		void CreateEmptyIndex(string branchId);
		void DeleteBranch(string branchId);
		void MapProductProperties(string branchId, string json);
		void RefreshSynonyms(string branchId);
    }
}
