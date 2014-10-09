using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.ETL
{
    public interface IStagingRepository
    {
        DataTable ReadAllBranches();
        DataTable ReadBrandControlLabels();
        DataTable ReadItems(string branchId);
        DataTable ReadSubCategories();
        DataTable ReadParentCategories();
        DataTable ReadFullItemForElasticSearch();
        DataSet ReadGSDataForItems();
		DataTable ReadProprietaryItems();
        DataTable ReadUniqueUsers();
        DataTable ReadCustomersByUser(string UserId);
        DataTable ReadContracts(string CustomerNumber, string DivisionName);
        DataTable ReadContractItems(string CustomerNumber, string DivisionName, string ContractNumber);
    }
}
