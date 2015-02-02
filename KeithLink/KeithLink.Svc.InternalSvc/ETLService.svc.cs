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
	[GlobalErrorBehaviorAttribute(typeof(ErrorHandler))]
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

        public bool ProcessCatalogData()
        {
            Task.Factory.StartNew(() => categoryLogic.ProcessCatalogData()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }

        public bool ProcessCustomerData()
        {
            
            //Task.Factory.StartNew(() => customerLogic.ImportCustomersToOrganizationProfile()).ContinueWith((t) =>
            //{ (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
            
            Task.Factory.StartNew(() => customerLogic.ImportDsrInfo()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }
        
        public bool ProcessInvoiceData()
        {
            Task.Factory.StartNew(() => invoiceLogic.ImportInvoices()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }

        public bool ProcessContractAndWorksheetData()
        {
            Task.Factory.StartNew(() => categoryLogic.ProcessContractAndWorksheetData()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
            
            return true;
        }

        public bool ProcessElasticSearchData()
        {
            Task.Factory.StartNew(() => categoryLogic.ProcessElasticSearchData()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }
        
	}
}
