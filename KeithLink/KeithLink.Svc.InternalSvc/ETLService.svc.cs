// KeithLink
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.ETL.ElasticSearch;

using KeithLink.Svc.InternalSvc.Interfaces;
using KeithLink.Svc.Impl.ETL;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.InternalSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ETLService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ETLService.svc or ETLService.svc.cs at the Solution Explorer and start debugging.
	[GlobalErrorBehaviorAttribute(typeof(ErrorHandler))]
	public class ETLService : IETLService
    {
        #region attributes

        private readonly ICatalogLogic categoryLogic;
        private readonly ICustomerLogic customerLogic;
        private readonly ICategoriesImport _esCategoriesImportLogic;
        private readonly IHouseBrandsImport _esHouseBrandsImportLogic;
        private readonly IItemImport _esItemImportLogic;
        private readonly IListsImportLogic _listImportLogic;

        private readonly IEventLogRepository _log;

        #endregion

        #region constructor

        public ETLService(ICatalogLogic categoryLogic, ICustomerLogic customerLogic, ICategoriesImport esCategoriesImport, IHouseBrandsImport esHouseBrandsImport, IItemImport esItemImport, IListsImportLogic listImportLogic, IEventLogRepository log)

        {
            this.categoryLogic = categoryLogic;
            this.customerLogic = customerLogic;
            this._esCategoriesImportLogic = esCategoriesImport;
            this._esHouseBrandsImportLogic = esHouseBrandsImport;
            this._esItemImportLogic = esItemImport;
            this._listImportLogic = listImportLogic;
            this._log = log;
        }

        #endregion

        #region methods

        public bool ProcessAll()
        { // this isn't actually run in production
            DateTime startTime = DateTime.Now;

            Task process = Task.Factory.StartNew(() => _log.WriteInformationLog(String.Format("ETL - Loading process started @ {0}", startTime)));

            process.ContinueWith((t) => _esItemImportLogic.ImportItems())
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => customerLogic.ImportCustomerItemHistory())
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => _listImportLogic.ImportContractItems())
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => _listImportLogic.ImportWorksheetItems())
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => customerLogic.ImportDsrInfo())
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => _esCategoriesImportLogic.ImportDepartments())
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => _esHouseBrandsImportLogic.ImportHouseBrands())
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => customerLogic.ImportCustomersToOrganizationProfile(), TaskContinuationOptions.ExecuteSynchronously)
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) => categoryLogic.ImportCatalog(), TaskContinuationOptions.ExecuteSynchronously)
                .ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            process.ContinueWith((t) =>
            {
                TimeSpan took = DateTime.Now - startTime;
                _log.WriteInformationLog(String.Format("ETL - Loading process took: {0}", took.Minutes));
            }, TaskContinuationOptions.ExecuteSynchronously);

            return true;
        }

        /// <summary>
        /// PRocess catalog data
        /// </summary>
        /// <returns></returns>
        public bool ProcessCatalogData()
        {
            Task.Factory.StartNew(() => categoryLogic.ImportCatalog()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }

        /// <summary>
        /// Process staging customer data
        /// </summary>
        /// <returns></returns>
        public bool ProcessCustomerData()
        {

            Task.Factory.StartNew(() => customerLogic.ImportCustomersToOrganizationProfile()).ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            Task.Factory.StartNew(() => customerLogic.ImportDsrInfo()).ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            // this task actually gets called from the import customers to organization profile. If we wait on that task to complete here, the ETL process could time out and throw an error
            //Task.Factory.StartNew( () => customerLogic.ImportUsersWithAccess() ).ContinueWith( ( t ) =>
            //{ new ErrorHandler().HandleError( t.Exception ); }, TaskContinuationOptions.OnlyOnFaulted );

            return true;

        }

        /// <summary>
        /// Process customer items
        /// </summary>
        /// <returns></returns>
        public bool ProcessCustomerItemHistory() 
        {
            Task.Factory.StartNew( () => customerLogic.ImportCustomerItemHistory() ).ContinueWith( ( t ) 
                => { (new ErrorHandler()).HandleError( t.Exception ); }, TaskContinuationOptions.OnlyOnFaulted );

            return true;
        }
        
        /// <summary>
        /// Processes contract and worksheet lists
        /// </summary>
        /// <returns></returns>
        public bool ProcessContractAndWorksheetData()
        {
            Task.Factory.StartNew(() => _listImportLogic.ImportContractItems()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            Task.Factory.StartNew(() => _listImportLogic.ImportWorksheetItems()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
            
            return true;
        }

        public bool ProcessInternalUserAccess() {
            Task.Factory.StartNew(() => customerLogic.ImportUsersWithAccess()).ContinueWith((t) => 
            { new ErrorHandler().HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }

        /// <summary>
        /// Process staging data and import into Elastic Search
        /// </summary>
        /// <returns></returns>
        public bool ProcessElasticSearchData()
        {
            // DEPRECATED - Use ImportDepartments instead
            //Task.Factory.StartNew( () => _esCategoriesImportLogic.ImportCategories() ).ContinueWith( ( t ) =>
            //{ (new ErrorHandler()).HandleError( t.Exception ); }, TaskContinuationOptions.OnlyOnFaulted );

            Task.Factory.StartNew(() => _esHouseBrandsImportLogic.ImportHouseBrands()).ContinueWith((t) =>
             { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            Task.Factory.StartNew(() => _esItemImportLogic.ImportItems()).ContinueWith((t) =>
             { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            Task.Factory.StartNew(() => _esCategoriesImportLogic.ImportDepartments()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }

        public bool ProcessUNFICatalogData() {
            Task.Factory.StartNew(() => categoryLogic.ImportUNFICatalog()).ContinueWith((t) => { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }

        public bool ProcessUNFIElasticSearchData()
		{
			Task.Factory.StartNew(() => _esItemImportLogic.ImportUNFIItems()).ContinueWith((t) =>
			{ (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

			Task.Factory.StartNew(() => _esCategoriesImportLogic.ImportUnfiCategories()).ContinueWith((t) =>
			{ (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
			return true;
		}
        #endregion
	}
}
