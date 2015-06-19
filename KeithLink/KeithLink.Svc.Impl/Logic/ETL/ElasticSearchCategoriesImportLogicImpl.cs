// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.InternalCatalog;

using KeithLink.Svc.Impl.Models;

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
    public class ElasticSearchCategoriesImportLogicImpl : IElasticSearchCategoriesImport {

        #region attributes

        private IStagingRepository _stagingRepository;
        private IElasticSearchRepository _elasticSearchRepository;
        private IEventLogRepository _eventLog;

        #endregion

        #region constructor

        public ElasticSearchCategoriesImportLogicImpl(IStagingRepository stagingRepository,
                                                      IElasticSearchRepository esRepository,
                                                      IEventLogRepository eventLogRepository) {

            _stagingRepository = stagingRepository;
            _elasticSearchRepository = esRepository;
            _eventLog = eventLogRepository;
        }

        #endregion

        #region methods

        /// <summary>
        /// Import categories to elasatic search
        /// </summary>
        public void ImportCategories()
        {
            try
            {
                DateTime start = DateTime.Now;
                _eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import categories to CS {0}", start.ToString()));

                var parentCategories = _stagingRepository.ReadParentCategories();
                var childCategories = _stagingRepository.ReadSubCategories();
                var categories = new BlockingCollection<ElasticSearchCategoryUpdate>();

                //Parent Categories
                //Parallel.ForEach(parentCategories.AsEnumerable(), row =>
                foreach (DataRow row in parentCategories.Rows)
                {
                    categories.Add(new ElasticSearchCategoryUpdate()
                    {
                        index = new ESCategoryRootData()
                        {
                            _id = row.GetString("CategoryId"),
                            data = new ESCategoryData()
                            {
                                parentcategoryid = null,
                                name = row.GetString("CategoryName"),
                                ppicode = row.GetString("PPICode"),
                                subcategories = PopulateSubCategories(row.GetString("CategoryId"), childCategories)
                            }
                        }
                    });
                };

                //Sub Categories
                //Parallel.ForEach(childCategories.AsEnumerable(), row =>
                foreach (DataRow row in childCategories.Rows)
                {
                    categories.Add(new ElasticSearchCategoryUpdate()
                    {
                        index = new ESCategoryRootData()
                        {
                            _id = row.GetString("CategoryId"),
                            data = new ESCategoryData()
                            {
                                parentcategoryid = row.GetString("ParentCategoryId"),
                                name = row.GetString("CategoryName"),
                                ppicode = row.GetString("PPICode")
                            }
                        }
                    });
                }

                _elasticSearchRepository.Create(string.Concat(categories.Select(c => c.ToJson())));
                
                TimeSpan took = DateTime.Now - start;
                _eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import categories to CS.  Process took {0}", took.ToString()));
            }
            catch (Exception e)
            {
                _eventLog.WriteErrorLog(String.Format("ETL: Error importing categories to CS -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));
            }
           
            
        }

        #endregion

        #region helper methods

        /// <summary>
        /// Helps build a tree of sub categories
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <param name="childCategories"></param>
        /// <returns></returns>
        private List<ESSubCategories> PopulateSubCategories(string parentCategoryId, DataTable childCategories)
        {
            var subCategories = new List<ESSubCategories>();

            var sub = childCategories.AsEnumerable().Where(c => c.Field<string>("ParentCategoryId") == parentCategoryId);

            foreach (var category in sub)
                subCategories.Add(new ESSubCategories()
                {
                    categoryid = category.Field<string>("CategoryId"),
                    name = category.Field<string>("CategoryName"),
                    ppicode = category.Field<string>("PPICode")
                });


            return subCategories;
        }
        
        #endregion
    }
}
