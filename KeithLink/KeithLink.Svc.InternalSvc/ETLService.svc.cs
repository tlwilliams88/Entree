using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.InternalSvc.Interfaces;
using KeithLink.Common.Core.Logging;
using System.Threading.Tasks;

namespace KeithLink.Svc.InternalSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ETLService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ETLService.svc or ETLService.svc.cs at the Solution Explorer and start debugging.
    public class ETLService : IETLService
    {
        private readonly ICatalogLogic categoryLogic;
        private readonly ICustomerLogic customerLogic;
		private readonly IInvoiceLogic invoiceLogic;
        

        public ETLService(ICatalogLogic categoryLogic, ICustomerLogic customerLogic, IInvoiceLogic invoiceLogic)
        {
            this.categoryLogic = categoryLogic;
            this.customerLogic = customerLogic;
			this.invoiceLogic = invoiceLogic;
        }

        public bool ProcessStagedData()
        {
            Task.Factory.StartNew(() => categoryLogic.ProcessStagedData());
            Task.Factory.StartNew(() => customerLogic.ImportCustomersToOrganizationProfile());
			Task.Factory.StartNew(() => invoiceLogic.ImportInvoices());
            return true;
        }
        
        public bool UpdateElasticSearch()
        {
            Task.Factory.StartNew(() => categoryLogic.ImportItemsToElasticSearch());
			Task.Factory.StartNew(() => categoryLogic.ImportCategoriesToElasticSearch());
            return true;
        }
        
        public bool UpdateCustomerOrganizations()
        {
            Task.Factory.StartNew(() => this.customerLogic.ImportCustomersToOrganizationProfile());
            return true;
        }

        /*for testing only*/
        public bool ImportPrePopulatedLists()
        {
            Task.Factory.StartNew(() => this.categoryLogic.ImportPrePopulatedLists());
            return true;
        }


		public bool ImportInvoices()
		{
			Task.Factory.StartNew(() => invoiceLogic.ImportInvoices());
			return true;
		}
	}
}
