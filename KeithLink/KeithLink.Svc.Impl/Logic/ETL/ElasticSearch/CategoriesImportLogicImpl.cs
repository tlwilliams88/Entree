// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.ETL.ElasticSearch;
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
    public class CategoriesImportLogicImpl : ICategoriesImport {

        #region attributes

        private IStagingRepository _stagingRepository;
        private IElasticSearchRepository _elasticSearchRepository;
        private IEventLogRepository _eventLog;

        #endregion

        #region constructor

        public CategoriesImportLogicImpl(IStagingRepository stagingRepository,
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
        /// DEPRECATED. Do not use anymore. Use ImportDepartments instead.
        /// </summary>
        public void ImportCategories() {
            try {
                DateTime start = DateTime.Now;
                _eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import categories to CS {0}", start.ToString()));

                var parentCategories = _stagingRepository.ReadParentCategories();
                var childCategories = _stagingRepository.ReadSubCategories();
                var categories = new BlockingCollection<ElasticSearchCategoryUpdate>();

                //Parent Categories
                //Parallel.ForEach(parentCategories.AsEnumerable(), row =>
                foreach (DataRow row in parentCategories.Rows) {
                    categories.Add(new ElasticSearchCategoryUpdate() {
                                                                         index = new ESCategoryRootData() {
                                                                                                              _id = row.GetString("CategoryId"),
                                                                                                              data = new ESCategoryData() {
                                                                                                                                              parentcategoryid = null,
                                                                                                                                              name = row.GetString("CategoryName"),
                                                                                                                                              ppicode = row.GetString("PPICode"),
                                                                                                                                              subcategories = PopulateSubCategories(row.GetString("CategoryId"), childCategories)
                                                                                                                                          }
                                                                                                          }
                                                                     });
                }
                ;

                //Sub Categories
                //Parallel.ForEach(childCategories.AsEnumerable(), row =>
                foreach (DataRow row in childCategories.Rows) {
                    categories.Add(new ElasticSearchCategoryUpdate() {
                                                                         index = new ESCategoryRootData() {
                                                                                                              _id = row.GetString("CategoryId"),
                                                                                                              data = new ESCategoryData() {
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
            } catch (Exception e) {
                _eventLog.WriteErrorLog(String.Format("ETL: Error importing categories to CS -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));
            }


        }

        /// <summary>
        /// Loading categories from departments for a simpler category list
        /// </summary>
        public void ImportDepartments() {
            DateTime start = StartLog("Department Import");

            DataTable departments = _stagingRepository.ReadDepartmentCategories();
            var categories = new BlockingCollection<ElasticSearchCategoryUpdate>();

            // Only get parent categories
            foreach (DataRow row in departments.AsEnumerable()
                                               .Where(x => x.GetInt("ParentDepartment")
                                                            .Equals(0))) {
                categories.Add(new ElasticSearchCategoryUpdate() {
                                                                     index = new ESCategoryRootData() {
                                                                                                          _id = row.GetString("DepartmentId"),
                                                                                                          data = new ESCategoryData() {
                                                                                                                                          parentcategoryid = null,
                                                                                                                                          name = row.GetString("DepartmentName"),
                                                                                                                                          subcategories = PopulateSubDepartments(row.GetString("DepartmentId"), departments)
                                                                                                                                      }
                                                                                                      }
                                                                 });
            }

            _elasticSearchRepository.Create(string.Concat(categories.Select(c => c.ToJson())));

            StopLog(start, "Department Import");
        }

        /// <summary>
        /// Import categories to elasatic search
        /// </summary>
        public void ImportUnfiCategories() {
            try {
                DateTime start = DateTime.Now;
                _eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import categories to CS {0}", start.ToString()));

                var parentCategories = _stagingRepository.ReadUnfiCategories();
                var childCategories = _stagingRepository.ReadUnfiSubCategories();
                var categories = new BlockingCollection<ElasticSearchCategoryUpdate>();

                //Parent Categories
                //Parallel.ForEach(parentCategories.AsEnumerable(), row =>
                foreach (DataRow row in parentCategories.Rows) {
                    categories.Add(new ElasticSearchCategoryUpdate() {
                                                                         index = new ESCategoryRootData() {
                                                                                                              _index = "unfi_categories",
                                                                                                              _id = row.GetString("CategoryId"),
                                                                                                              data = new ESCategoryData() {
                                                                                                                                              parentcategoryid = null,
                                                                                                                                              name = row.GetString("CategoryName"),
                                                                                                                                              ppicode = row.GetString("CategoryId"),
                                                                                                                                              department = row.GetString("Department"),
                                                                                                                                              subcategories = PopulateUnfiSubCategories(row.GetString("CategoryId"), childCategories)
                                                                                                                                          }
                                                                                                          }
                                                                     });
                }
                ;

                //Sub Categories
                //Parallel.ForEach(childCategories.AsEnumerable(), row =>
                foreach (DataRow row in childCategories.Rows) {
                    categories.Add(new ElasticSearchCategoryUpdate() {
                                                                         index = new ESCategoryRootData() {
                                                                                                              _index = "unfi_categories",
                                                                                                              _id = row.GetString("CategoryId"),
                                                                                                              data = new ESCategoryData() {
                                                                                                                                              parentcategoryid = row.GetString("ParentCategoryId"),
                                                                                                                                              name = row.GetString("CategoryName"),
                                                                                                                                              ppicode = row.GetString("CategoryId"),
                                                                                                                                              department = row.GetString("Department"),
                                                                                                                                          }
                                                                                                          }
                                                                     });
                }

                _elasticSearchRepository.Create(string.Concat(categories.Select(c => c.ToJson())));

                TimeSpan took = DateTime.Now - start;
                _eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import categories to CS.  Process took {0}", took.ToString()));
            } catch (Exception e) {
                _eventLog.WriteErrorLog(String.Format("ETL: Error importing categories to CS -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));

                KeithLink.Common.Impl.Email.ExceptionEmail.Send(e,
                                                                "ETL: Error importing categories to CS -- whole process failed.");
            }


        }

        #endregion

        #region helper methods

        /// <summary>
        /// Helps build a tree of sub categories
        /// DEPRECATED - See ImportDepartments
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <param name="childCategories"></param>
        /// <returns></returns>
        private List<ESSubCategories> PopulateSubCategories(string parentCategoryId, DataTable childCategories) {
            var subCategories = new List<ESSubCategories>();

            var sub = childCategories.AsEnumerable()
                                     .Where(c => c.Field<string>("ParentCategoryId") == parentCategoryId);

            foreach (var category in sub)
                subCategories.Add(new ESSubCategories() {
                                                            categoryid = category.Field<string>("CategoryId"),
                                                            name = category.Field<string>("CategoryName"),
                                                            ppicode = category.Field<string>("PPICode")
                                                        });


            return subCategories;
        }

        private List<ESSubCategories> PopulateUnfiSubCategories(string parentCategoryId, DataTable childCategories) {
            var subCategories = new List<ESSubCategories>();

            var sub = childCategories.AsEnumerable()
                                     .Where(c => c.Field<string>("ParentCategoryId") == parentCategoryId);

            foreach (var category in sub)
                subCategories.Add(new ESSubCategories() {
                                                            categoryid = category.Field<string>("CategoryId"),
                                                            name = category.Field<string>("CategoryName"),
                                                            ppicode = category.Field<string>("CategoryId")
                                                        });


            return subCategories;
        }

        /// <summary>
        /// Populate the main department categories with sub-categories
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="departments"></param>
        /// <returns></returns>
        private List<ESSubCategories> PopulateSubDepartments(string departmentId, DataTable departments) {
            List<ESSubCategories> returnValue = new List<ESSubCategories>();

            // Only return categories that belong to the main department/category. 
            // The format for those is: 00
            // The first digit is the primary category, the second is the sub category id.
            // So 10 belongs to category 1, et cetera.
            var subDepartments = departments.AsEnumerable()
                                            .Where(
                                                   x => x.GetInt("ParentDepartment") > 0 &&
                                                        x.GetInt("ParentDepartment")
                                                         .ToString()
                                                         .StartsWith(departmentId)
                                                  )
                                            .ToList();

            foreach (DataRow subDepartment in subDepartments) {
                returnValue.Add(new ESSubCategories() {
                                                          categoryid = subDepartment.GetString("DepartmentId"),
                                                          name = subDepartment.GetString("DepartmentName")
                                                      });
            }


            return returnValue;
        }

        private DateTime StartLog(string processName) {
            DateTime returnValue = DateTime.Now;
            _eventLog.WriteInformationLog(String.Format("[Started],ETL Import,{0},ES,{1}", processName, returnValue));

            return returnValue;
        }

        private void StopLog(DateTime startTime, string processName) {
            TimeSpan took = DateTime.Now - startTime;
            _eventLog.WriteInformationLog(String.Format("[Finished],ETL Import,{0},ES,{1}", processName, took));
        }

        #endregion
    }
}
