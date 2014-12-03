using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.ETL
{
    public interface ICatalogLogic
    {
        void ProcessCatalogData();
        void ProcessContractAndWorksheetData();
        void ProcessElasticSearchData();
        //void ImportCatalog();
        //void ImportProfiles();
        //void ImportItemsToElasticSearch();
        //void ImportCategoriesToElasticSearch();
        //void ImportPrePopulatedLists();
    }
}
