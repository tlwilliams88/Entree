﻿// KeithLink
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.InternalSvc.Interfaces;

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
        private readonly IElasticSearchCategoriesImport _esCategoriesImportLogic;
        private readonly IElasticSearchHouseBrandsImport _esHouseBrandsImportLogic;
        private readonly IElasticSearchItemImport _esItemImportLogic;
        private readonly IListsImportLogic _listImportLogic;

        #endregion

        #region constructor

        public ETLService(ICatalogLogic categoryLogic, ICustomerLogic customerLogic, IElasticSearchCategoriesImport esCategoriesImport, IElasticSearchHouseBrandsImport esHouseBrandsImport, IElasticSearchItemImport esItemImport, IListsImportLogic listImportLogic)
        {
            this.categoryLogic = categoryLogic;
            this.customerLogic = customerLogic;
            this._esCategoriesImportLogic = esCategoriesImport;
            this._esHouseBrandsImportLogic = esHouseBrandsImport;
            this._esItemImportLogic = esItemImport;
            this._listImportLogic = listImportLogic;
        }

        #endregion

        #region methods

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
            
            Task.Factory.StartNew(() => customerLogic.ImportCustomersToOrganizationProfile()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
            
            Task.Factory.StartNew(() => customerLogic.ImportDsrInfo()).ContinueWith((t) =>
            { (new ErrorHandler()).HandleError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

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

        /// <summary>
        /// Process staging data and import into Elastic Search
        /// </summary>
        /// <returns></returns>
        public bool ProcessElasticSearchData()
        {
            Task.Factory.StartNew( () => _esCategoriesImportLogic.ImportCategories() ).ContinueWith( ( t ) =>
            { (new ErrorHandler()).HandleError( t.Exception ); }, TaskContinuationOptions.OnlyOnFaulted );
            
            Task.Factory.StartNew( () => _esHouseBrandsImportLogic.ImportHouseBrands() ).ContinueWith( ( t ) =>
            { (new ErrorHandler()).HandleError( t.Exception ); }, TaskContinuationOptions.OnlyOnFaulted );
            
            Task.Factory.StartNew( () => _esItemImportLogic.ImportItems() ).ContinueWith( ( t ) =>
            { (new ErrorHandler()).HandleError( t.Exception ); }, TaskContinuationOptions.OnlyOnFaulted );

            return true;
        }

        #endregion

    }
}
