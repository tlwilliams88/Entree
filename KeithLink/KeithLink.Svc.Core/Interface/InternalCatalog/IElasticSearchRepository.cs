using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.InternalCatalog
{
    public interface IElasticSearchRepository
    {
        void Create(string json);
		void MapProductProperties(string branchId, string json);
		void DeleteBranch(string branchId);
		bool CheckIfIndexExist(string branchId);
		void CreateEmptyIndex(string branchId);
		void RefreshSynonyms(string branchId);
    }
}
