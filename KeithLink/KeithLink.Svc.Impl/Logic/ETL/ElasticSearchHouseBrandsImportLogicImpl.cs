// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.InternalCatalog;

// Core
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Sql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.ETL {
    public class ElasticSearchHouseBrandsImportLogicImpl : IElasticSearchHouseBrandsImport {

        #region attributes

        private IStagingRepository _stagingRepository;
        private IElasticSearchRepository _elasticSearchRepository;
        private IEventLogRepository _eventLog;

        #endregion

        #region constructor

        public ElasticSearchHouseBrandsImportLogicImpl( IStagingRepository stagingRepository,
                                                  IElasticSearchRepository elasticSearchRepository,
                                                  IEventLogRepository eventLogRespository ) {

            _stagingRepository = stagingRepository;
            _elasticSearchRepository = elasticSearchRepository;
            _eventLog = eventLogRespository;
        }

        #endregion

        #region functions 

        public void ImportHouseBrands()
        {
            //For performance debugging purposes
            var startTime = DateTime.Now;

            var brandsDataTable = _stagingRepository.ReadBrandControlLabels();
            var brands = new BlockingCollection<Models.ElasticSearch.BrandControlLabels.BrandUpdate>();

            //Parallel.ForEach(brandsDataTable.AsEnumerable(), row =>reach
            foreach(DataRow row in brandsDataTable.Rows)
                {
                    brands.Add(new Models.ElasticSearch.BrandControlLabels.BrandUpdate() 
                    {
                        index = new Models.ElasticSearch.BrandControlLabels.RootData()
                        {
                            _id = row.GetString("ControlLabel"),
                            data = new Models.ElasticSearch.BrandControlLabels.BrandData()
                            {
                                BrandControlLabel = row.GetString("ControlLabel"),
                                ExtendedDescription = row.GetString("ExtendedDescription")
                            }
                        }
                    });
                }

            _elasticSearchRepository.Create(string.Concat(brands.Select(c => c.ToJson())));

            _eventLog.WriteInformationLog(string.Format("ImportHouseBrandsToElasticSearch Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        #endregion

    }

    




}
