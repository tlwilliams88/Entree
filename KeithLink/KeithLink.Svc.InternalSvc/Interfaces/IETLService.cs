using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IETLService" in both code and config file together.
    [ServiceContract]
    public interface IETLService
    {
        [OperationContract]
        bool ProcessCatalogData();
        [OperationContract]
        bool ProcessCustomerData();
        [OperationContract]
        bool ProcessCustomerItemHistory();
        [OperationContract]
        bool ProcessInvoiceData();
        [OperationContract]
        bool ProcessContractAndWorksheetData();
		[OperationContract]
		bool ProcessElasticSearchData();
    }
}
