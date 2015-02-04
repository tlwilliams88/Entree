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
        DataTable ReadCustomers();
        DataTable ReadCSUsers();
        DataTable ReadContractItems(string customerNumber, string divisionName, string contractNumber);
        DataTable ReadWorksheetItems(string customerNumber, string divisionName);
        DataTable ReadDsrInfo();
        DataTable ReadDsrImages();
		void ProcessInvoices();
    }
}
