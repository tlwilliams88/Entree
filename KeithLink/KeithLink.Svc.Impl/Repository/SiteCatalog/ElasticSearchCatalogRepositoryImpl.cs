using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;

using Elasticsearch.Net;
using Nest;
using RestSharp;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using KeithLink.Svc.Impl.Seams;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class ElasticSearchCatalogRepositoryImpl : ICatalogRepository
    {
        #region attributes
        private Helpers.ElasticSearch _eshelper;
        // In the request to ElasticSearch, there are different fields that are search for category/subcategory codes for BEK vs nonBEK 
        // products
        private string _catalog = null;
        #endregion

        #region constructor
        public ElasticSearchCatalogRepositoryImpl()
        {
            _eshelper = new Helpers.ElasticSearch();
            _catalog = "bek";
        }
        #endregion

        #region methods
        private dynamic BuildBoolFunctionScoreQuery(int from, int size, string sortField, string sortDir, ExpandoObject query)
        {
            return new {
                from = from,
                size = size,
                query = new {
                    function_score = new {
                        query,
                        functions = BuildItemBoostFunctions(),
                        score_mode = "max",
                        boost_mode = "multiply"
                    }
                },
                sort = BuildSort(sortField, sortDir),
                aggregations = ElasticSearchAggregations
            };
        }

        private dynamic BuildBoolFunctionScoreQueryNoFields(int from, int size, string sortField, string sortDir, ExpandoObject query)
        {
            return new {
                from = from,
                size = size,
                query = new {
                    function_score = new {
                        query,
                        functions = BuildItemBoostFunctions(),
                        score_mode = "max",
                        boost_mode = "multiply"
                    }
                },
                fields = new string[0],
                sort = BuildSort(sortField, sortDir),
                aggregations = ElasticSearchAggregations
            };
        }

        private dynamic BuildBoolMultiMatchQuery(SearchInputModel searchModel, List<dynamic> filterTerms, List<string> fieldsToSearch, string searchExpression)
        {
            List<dynamic> statusFields = BuildStatusFilter();

            List<dynamic> musts = new List<dynamic>();
            if (searchExpression != null)
            {
                musts.Add(new {
                    multi_match = new {
                        query = searchExpression,
                        @type = "most_fields",
                        fields = fieldsToSearch,
                        @operator = "and"
                    },
                });
            }

            musts.Add(filterTerms);

            return new {
                from = searchModel.From,
                size = searchModel.Size,
                query = new {
                    @bool = new {
                        must = musts,
                        must_not = statusFields
                    }
                },
                sort = BuildSort(searchModel.SField, searchModel.SDir),
                aggregations = ElasticSearchAggregations
            };

        }

        private dynamic BuildBoolMultiMatchQueryNoFields(SearchInputModel searchModel, List<dynamic> filterTerms, List<string> fieldsToSearch, string searchExpression)
        {
            List<dynamic> statusFields = BuildStatusFilter();

            List<dynamic> musts = new List<dynamic>();
            if (searchExpression != null)
            {
                musts.Add(new {
                    multi_match = new {
                        query = searchExpression,
                        @type = "most_fields",
                        fields = fieldsToSearch,
                        @operator = "and"
                    },
                });
            }

            musts.Add(filterTerms);

            return new {
                from = searchModel.From,
                size = searchModel.Size,
                query = new {
                    @bool = new {
                        must = musts,
                        must_not = statusFields
                    }
                },
                sort = BuildSort(searchModel.SField, searchModel.SDir),
                aggregations = ElasticSearchAggregations
            };

        }

        /// <summary>
        /// filter out items with unwanted statuses
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        private dynamic BuildCategoryFilter(string category)
        {
            return new {
                multi_match =
                    new {
                        query = category,
                        fields =
                        new List<string>() { "categoryname_not_analyzed", "parentcategoryname_not_analyzed", "categoryid", "parentcategoryid" }
                    }
            };
        }


        private dynamic BuildProprietaryItemFilter(UserSelectedContext catalogInfo, string department)
        {
            List<dynamic> proprietaryItems = new List<dynamic>();

            //Build filter for proprietary items
            if (!string.IsNullOrEmpty(catalogInfo.CustomerId))
                proprietaryItems.Add(new { query_string = new { query = string.Format("isproprietary:false OR (isproprietary:true AND proprietarycustomers: {0})", catalogInfo.CustomerId) } });
            else
                proprietaryItems.Add(new { match = new { isproprietary = false } }); //No CustomerId (Guest), filter out all proprietary items

            //if (!string.IsNullOrEmpty(department)) {
            //    proprietaryItems.Add(new { match = new { @department = department } });
            //}

            return proprietaryItems;
        }

        private List<dynamic> BuildFacetsFilter(string facetFilters)
        {
            List<dynamic> mustClause = new List<dynamic>();
            string facetSeparator = "___";
            string[] facets = facetFilters.Split(new string[] { facetSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] values = s.Substring(s.IndexOf(":") + 1).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValues[0].Equals("specialfilters") == false) // pull specialfilters out of ES query
                {
                    string keyValue = ElasticSearchAggregationsMap[keyValues[0]];
                    string selectedValues = String.Join("\",\"", values);
                    ExpandoObject keyValueSelectedValues = new ExpandoObject();
                    (keyValueSelectedValues as IDictionary<string, object>).Add(keyValue, values);
                    mustClause.Add(new { terms = keyValueSelectedValues });
                }
            }

            return mustClause;
        }

        private dynamic BuildFilterTerms(string facetFilters, UserSelectedContext catalogInfo, string category = "", string department = "")
        {
            List<dynamic> mustClause = new List<dynamic>();
            string facetSeparator = "___";
            string[] facets = facetFilters.Split(new string[] { facetSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] values = s.Substring(s.IndexOf(":") + 1).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValues[0].Equals("specialfilters") == false) // pull specialfilters out of ES query
                {
                    string keyValue = ElasticSearchAggregationsMap[keyValues[0]];
                    string selectedValues = String.Join("\",\"", values);
                    ExpandoObject keyValueSelectedValues = new ExpandoObject();
                    (keyValueSelectedValues as IDictionary<string, object>).Add(keyValue, values);
                    mustClause.Add(new { terms = keyValueSelectedValues });
                }
            }

            //Build filter for proprietary items
            if (!string.IsNullOrEmpty(catalogInfo.CustomerId))
                mustClause.Add(new { query_string = new { query = string.Format("isproprietary:false OR (isproprietary:true AND proprietarycustomers: {0})", catalogInfo.CustomerId) } });
            else
                mustClause.Add(new { match = new { isproprietary = false } }); //No CustomerId (Guest), filter out all proprietary items

            if (!string.IsNullOrEmpty(department) && department.Equals("0") == false)
            {
                mustClause.Add(new { match = new { @parentcategoryid = department } });
            }

            if (!String.IsNullOrEmpty(category))
                mustClause.Add(BuildCategoryFilter(category));

            return mustClause;
        }

        private dynamic BuildFilterTermsWithMustNot(string facetFilters, UserSelectedContext catalogInfo, string category = "", string department = "")
        {
            List<dynamic> mustClause = new List<dynamic>();
            string facetSeparator = "___";
            string[] facets = facetFilters.Split(new string[] { facetSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValues[0].Equals("specialfilters") == false) // pull specialfilters out of ES query
                {
                    string[] values = s.Substring(s.IndexOf(":") + 1).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    string keyValue = ElasticSearchAggregationsMap[keyValues[0]];
                    string selectedValues = String.Join("\",\"", values);
                    ExpandoObject keyValueSelectedValues = new ExpandoObject();
                    (keyValueSelectedValues as IDictionary<string, object>).Add(keyValue, values);
                    mustClause.Add(new { terms = keyValueSelectedValues });
                }
            }

            //Build filter for proprietary items
            if (!string.IsNullOrEmpty(catalogInfo.CustomerId))
                mustClause.Add(new { query_string = new { query = string.Format("isproprietary:false OR (isproprietary:true AND proprietarycustomers: {0})", catalogInfo.CustomerId) } });
            else
                mustClause.Add(new { match = new { isproprietary = false } }); //No CustomerId (Guest), filter out all proprietary items

            if (!string.IsNullOrEmpty(department))
            {
                mustClause.Add(new { match = new { @department = department } });
            }

            if (!String.IsNullOrEmpty(category))
                mustClause.Add(BuildCategoryFilter(category));

            List<dynamic> fieldFilterTerms = BuildStatusFilter();

            ExpandoObject filterTerms = new ExpandoObject();
            (filterTerms as IDictionary<string, object>).Add("bool", new { must = mustClause, must_not = fieldFilterTerms });

            return filterTerms;
        }

        /// <summary>
        /// Searches through the given facetFilters and pulls a copy of just the pricefilters
        /// </summary>
        /// <param name="facetFilters"></param>
        /// <returns></returns>
        public List<string> SeekSpecialFilters(string facetFilters)
        {
            List<string> prcFilters = new List<string>();
            string facetSeparator = "___";
            string[] facets = facetFilters.Split(new string[] { facetSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] values = s.Substring(s.IndexOf(":") + 1).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValues[0].Equals("specialfilters"))
                {
                    prcFilters.AddRange(values);
                }
            }
            return prcFilters;
        }

        private List<dynamic> BuildItemBoostFunctions(string searchTerms = null)
        {
            List<dynamic> boosts = new List<dynamic>();


            if (!string.IsNullOrEmpty(searchTerms))
            {
                //boosts.Add(new {
                //    filter = new {
                //        query = new {
                //            @bool = new {
                //                must = new List<dynamic>() { 
                //                    new { 
                //                        match = new { 
                //                            name_ngram_analyzed = new { 
                //                                query = searchTerms.ToLower(), @operator = "and", minimum_should_match = "75%" 
                //                            } 
                //                        } 
                //                    } 
                //                }
                //            }
                //        }
                //    },
                //    boost_factor = 1600
                //});
                boosts.Add(new {
                    filter = new { query = new { @bool = new { should = new List<dynamic>() { new { match_phrase_prefix = new { name_not_analyzed = searchTerms.ToLower() } } } } } },
                    boost_factor = 1500
                });
                boosts.Add(new {
                    filter = new { query = new { @bool = new { should = new List<dynamic>() { new { match_phrase = new { name = searchTerms } } } } } },
                    boost_factor = 1400
                });
                boosts.Add(new {
                    filter = new { query = new { @bool = new { should = new List<dynamic>() { new { match = new { name = searchTerms } } } } } },
                    boost_factor = 1300
                });
                if (!searchTerms.Contains("OR upc:*")) //This is a search for a itemnumber or upc, don't add this boost
                {
                    boosts.Add(new {
                        filter = new { query = new { query_string = new { fields = new List<string>() { "name" }, use_dis_max = true, query = string.Join(" AND ", searchTerms.Split(' ').ToArray()) } } },
                        boost_factor = 1200
                    });
                }
            }


            // preferred item boosts
            //boosts.Add(new { filter = new { term = new { preferreditemcode = "A" } }, 
            //    boost_factor = 300 });
            //boosts.Add(new { filter = new { term = new { preferreditemcode = "B" } }, 
            //    boost_factor = 200 });
            //boosts.Add(new { filter = new { term = new { preferreditemcode = "C" } }, 
            //    boost_factor = 100 });


            return boosts;
        }

        private dynamic BuildSort(string sortField, string sortDir)
        {
            // In BuildSort, cases desc is always added as a secondary sort to shuffle items with higher inventory to the front,
            // when the primary sort is equal
            if (!string.IsNullOrEmpty(sortField))
            {
                ExpandoObject sortObject = new ExpandoObject();
                (sortObject as IDictionary<string, object>).Add(sortField, string.IsNullOrEmpty(sortDir) ? "asc" : sortDir);
                (sortObject as IDictionary<string, object>).Add("cases.keyword", "desc");
                return sortObject;
            }
            else
            {
                ExpandoObject sortObject = new ExpandoObject();
                (sortObject as IDictionary<string, object>).Add("_score", "desc");
                (sortObject as IDictionary<string, object>).Add("cases.keyword", "desc");
                return sortObject;
            }
        }

        /// <summary>
        /// filter out items with unwanted statuses
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        private List<dynamic> BuildStatusFilter()
        {
            string[] valuesToFilter = Configuration.ElasticSearchItemExcludeValues.Split(',');
            List<dynamic> fieldFilterTerms = new List<dynamic>();

            foreach (string s in valuesToFilter)
            {
                fieldFilterTerms.Add(new { match = new { status1_not_analyzed = s } });
            }

            return fieldFilterTerms;
        }

        public CategoriesReturn GetCategories(int from, int size, string catagoryType)
        {
            var index = "";
            switch (catagoryType.ToLower())
            {
                case "unfi":
                index = Constants.ES_UNFI_INDEX_CATEGORIES;
                break;
                default:
                index = Constants.ES_INDEX_CATEGORIES;
                break;
            }

            var response = _eshelper.ElasticClient.Search<Category>(s => s
                .From(from)
                .Size(GetCategoryPagingSize(size))
                .Index(index)
                .Type(Constants.ES_TYPE_CATEGORY)
                );

            var prefixesToExclude = Configuration.CategoryPrefixesToExclude.Split(',').ToList();

            // Have to do this because it won't infer from the ID up one level in the structure. Need to revisit.
            foreach (var r in response.Hits)
            {
                r.Source.Id = r.Id;
            }

            List<Category> filteredResults = (from s in response.Documents
                                              where !(from p in prefixesToExclude select p).Contains(s.Id)
                                              && (s.SubCategories != null && !s.Id.Equals(0))
                                              orderby s.Id
                                              select s).ToList<Category>();

            CategoriesReturn results = new CategoriesReturn { Categories = filteredResults };

            return results;
        }

        private int GetCategoryPagingSize(int size)
        {
            if (size <= 0)
                return Configuration.DefaultCategoryReturnSize;
            return size;
        }

        public ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel)
        {
            int size = GetProductPagingSize(searchModel.Size);

            List<dynamic> filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo);

            dynamic categorySearchExpression = BuildBoolMultiMatchQuery(searchModel, filterTerms, new List<string>() { "brand_control_label" }, brandControlLabel);

            return GetProductsFromElasticSearch(catalogInfo.BranchId.ToLower(), true, "", categorySearchExpression);
        }

        public ProductsReturn GetHouseProductNumbersByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel)
        {
            searchModel.Size = 1;
            searchModel.From = 0;

            List<dynamic> filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo);

            dynamic categorySearchExpression = BuildBoolMultiMatchQueryNoFields(searchModel, filterTerms, new List<string>() { "brand_control_label" }, brandControlLabel);

            searchModel.Size = GetCountProductFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);

            categorySearchExpression = BuildBoolMultiMatchQueryNoFields(searchModel, filterTerms, new List<string>() { "brand_control_label" }, brandControlLabel);

            return GetProductNumbersFromElasticSearch(catalogInfo.BranchId.ToLower(), true, "", categorySearchExpression);
        }

        public Product GetProductById(string branch, string id)
        {
            branch = branch.ToLower();

            var getRequest = new GetRequest(branch, "product", id);
            IGetResponse<DynamicResponse> res = _eshelper.ElasticClient.Get<DynamicResponse>(getRequest);

            if (res == null || !res.IsValid || !res.Found)
                return null;

            return LoadProductFromElasticSearchProductFromHighLevelCall(false, res);
        }

        private int GetProductPagingSize(int size)
        {
            if (size <= 0)
                return Configuration.DefaultProductReturnSize;
            return size;
        }

        private string GetTemperatureZoneDescription(string code)
        {
            if (code.Equals(Constants.TEMP_ZONE_DRY_CODE, StringComparison.CurrentCultureIgnoreCase))
            {
                return Constants.TEMP_ZONE_DRY_DESCRIPTION;
            }
            else if (code.Equals(Constants.TEMP_ZONE_FROZEN_CODE, StringComparison.CurrentCultureIgnoreCase))
            {
                return Constants.TEMP_ZONE_FROZEN_DESCRIPTION;
            }
            else if (code.Equals(Constants.TEMP_ZONE_REFRIGERATED_CODE, StringComparison.CurrentCultureIgnoreCase))
            {
                return Constants.TEMP_ZONE_REFRIGERATED_DESCRIPTION;
            }
            return "?";
        }

        public ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel)
        {
            SetWorkingCatalog(catalogInfo.BranchId);

            int size = 0;
            if (searchModel.Size > 0)
            {
                size = GetProductPagingSize(searchModel.Size);
            }

            int from = 0;
            if (searchModel.From > 0)
            {
                from = searchModel.From;
            }

            //List<string> childCategories = 
            //    GetCategories(0, Configuration.DefaultCategoryReturnSize).Categories.Where(c => c.Id.Equals(category, StringComparison.CurrentCultureIgnoreCase)).SelectMany(s => s.SubCategories.Select(i => i.Id)).ToList();

            ExpandoObject filterTerms = BuildFilterTermsWithMustNot(searchModel.Facets, catalogInfo, category: category);

            //string categorySearch = (childCategories.Count == 0 ? category : String.Join(" OR ", childCategories.ToArray()));

            dynamic categorySearchExpression = BuildBoolFunctionScoreQuery(from, size, searchModel.SField, searchModel.SDir,
                filterTerms);

            var query = Newtonsoft.Json.JsonConvert.SerializeObject(categorySearchExpression);

            List<string> specialFilters = SeekSpecialFilters(searchModel.Facets);

            ProductsReturn ret;

            if (searchModel.Size == 0)
            {
                from = 0;
                size = 1;

                categorySearchExpression = BuildBoolFunctionScoreQueryNoFields(from, size, searchModel.SField, searchModel.SDir,
                    filterTerms);

                query = Newtonsoft.Json.JsonConvert.SerializeObject(categorySearchExpression);

                size = GetCountProductFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);

                categorySearchExpression = BuildBoolFunctionScoreQuery(from, size, searchModel.SField, searchModel.SDir,
                    filterTerms);

                query = Newtonsoft.Json.JsonConvert.SerializeObject(categorySearchExpression);

                ret = GetProductsFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);
            }
            else
            {
                ret = GetProductsFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);
            }

            return ret;
        }

        public ProductsReturn GetProductNumbersByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel)
        {
            SetWorkingCatalog(catalogInfo.BranchId);

            int size = 0;
            if (searchModel.Size > 0)
            {
                size = GetProductPagingSize(searchModel.Size);
            }

            int from = 0;
            if (searchModel.From > 0)
            {
                from = searchModel.From;
            }

            ExpandoObject filterTerms = BuildFilterTermsWithMustNot(searchModel.Facets, catalogInfo, category: category);

            from = 0;
            size = 1;

            dynamic categorySearchExpression = BuildBoolFunctionScoreQueryNoFields(from, size, searchModel.SField, searchModel.SDir,
                filterTerms);

            var query = Newtonsoft.Json.JsonConvert.SerializeObject(categorySearchExpression);

            size = GetCountProductFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);

            categorySearchExpression = BuildBoolFunctionScoreQueryNoFields(from, size, searchModel.SField, searchModel.SDir,
                filterTerms);

            query = Newtonsoft.Json.JsonConvert.SerializeObject(categorySearchExpression);

            return GetProductNumbersFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);
        }

        public ProductsReturn GetProductsByIds(string branch, List<string> ids)
        {
            var productList = String.Join(" OR ", ids);
            var query = @"{
						""from"" : 0, ""size"" : 5000,
						""query"":{
						""query_string"" : {
						""fields"" : [""itemnumber"",""mfritemnumber""],
							""query"" : """ + productList + @""",
						""use_dis_max"" : true
							}
						}}";

            return GetProductsFromElasticSearch(branch, false, query);
        }

        public ProductsReturn GetProductsByItemNumbers(string branch, List<string> ids, SearchInputModel searchModel)
        {
            var q = new {
                from = searchModel.From,
                size = searchModel.Size,
                query = new {
                    @bool = new {
                        filter = new {
                            terms = new {
                                itemnumber = ids
                            }
                        },
                        must = BuildFacetsFilter(searchModel.Facets)
                    },
                },
                sort = BuildSort(searchModel.SField, searchModel.SDir),
                aggregations = ElasticSearchAggregations
            };

            string query = Newtonsoft.Json.JsonConvert.SerializeObject(q);

            ProductsReturn returnValue = GetProductsFromElasticSearch(branch, false, query);

            return returnValue;
        }

        public ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel)
        {
            SetWorkingCatalog(searchModel.CatalogType);

            List<dynamic> filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo, department: searchModel.Dept);
            string termSearch = search;
            List<string> fieldsToSearch = null;
            dynamic termSearchExpression = null;

            System.Text.RegularExpressions.Regex matchOnlyDigits = new System.Text.RegularExpressions.Regex(@"^\d+$");

            if (search != null && matchOnlyDigits.IsMatch(search))
            {
                fieldsToSearch = Configuration.ElasticSearchDigitSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQueryForDigits(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }
            else
            {
                fieldsToSearch = Configuration.ElasticSearchTermSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQuery(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }

            termSearchExpression = BuildBoolMultiMatchQuery(searchModel, filterTerms, fieldsToSearch, termSearch);

            var query = Newtonsoft.Json.JsonConvert.SerializeObject(termSearchExpression);

            string branch = catalogInfo.BranchId.ToLower();

            return GetProductsFromElasticSearch(branch, true, "", termSearchExpression);
        }

        public ProductsReturn GetProductNumbersBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel)
        {
            List<dynamic> filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo, department: searchModel.Dept);
            string termSearch = search;
            List<string> fieldsToSearch = null;
            dynamic termSearchExpression = null;

            System.Text.RegularExpressions.Regex matchOnlyDigits = new System.Text.RegularExpressions.Regex(@"^\d+$");

            if (search != null && matchOnlyDigits.IsMatch(search))
            {
                fieldsToSearch = Configuration.ElasticSearchDigitSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQueryForDigits(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }
            else
            {
                fieldsToSearch = Configuration.ElasticSearchTermSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQuery(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }

            termSearchExpression = BuildBoolMultiMatchQueryNoFields(searchModel, filterTerms, fieldsToSearch, termSearch);

            var query = Newtonsoft.Json.JsonConvert.SerializeObject(termSearchExpression);

            string branch = catalogInfo.BranchId.ToLower();

            return GetProductNumbersFromElasticSearch(branch, true, "", termSearchExpression);
        }

        private ProductsReturn GetProductsFromElasticSearch(string branch, bool listonly, string searchBody, object searchBodyD = null)
        {
            string requestToJSON = "";
            if (searchBodyD == null)
            {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBody);
            }
            else
            {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBodyD);
            }

            ElasticsearchResponse<DynamicResponse> res = null;

            if (searchBodyD == null)
                res = _eshelper.ElasticClient.LowLevel.Search<DynamicResponse>(branch.ToLower(), "product", searchBody);
            else
                res = _eshelper.ElasticClient.LowLevel.Search<DynamicResponse>(branch.ToLower(), "product", searchBodyD);

            List<Product> products = new List<Product>();
            foreach (var oProd in res.Body["hits"]["hits"])
            {
                Product p = LoadProductFromElasticSearchProduct(listonly, oProd);
                products.Add(p);
            }
            ExpandoObject facets = LoadFacetsFromElasticSearchResponse(res);
            int totalCount = Convert.ToInt32(res.Body["hits"]["total"].Value);

            return new ProductsReturn() { Products = products, Facets = facets, TotalCount = totalCount, Count = products.Count };
        }

        private ProductsReturn GetProductNumbersFromElasticSearch(string branch, bool listonly, string searchBody, object searchBodyD = null)
        {
            string requestToJSON = "";
            if (searchBodyD == null)
            {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBody);
            }
            else
            {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBodyD);
            }

            ElasticsearchResponse<DynamicResponse> res = null;

            if (searchBodyD == null)
                res = _eshelper.ElasticClient.LowLevel.Search<DynamicResponse>(branch.ToLower(), "product", searchBody);
            else
                res = _eshelper.ElasticClient.LowLevel.Search<DynamicResponse>(branch.ToLower(), "product", searchBodyD);

            List<Product> products = new List<Product>();
            foreach (var oProd in res.Body["hits"]["hits"])
            {
                products.Add(new Product() { ItemNumber = oProd._id, CatalogId = oProd._index });
            }
            ExpandoObject facets = LoadFacetsFromElasticSearchResponse(res);
            int totalCount = Convert.ToInt32(res.Body["hits"]["total"].Value);

            return new ProductsReturn() { Products = products, Facets = facets, TotalCount = totalCount, Count = products.Count };
        }

        private int GetCountProductFromElasticSearch(string branch, bool listonly, string searchBody, object searchBodyD = null)
        {
            string requestToJSON = "";
            if (searchBodyD == null)
            {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBody);
            }
            else
            {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBodyD);
            }

            ElasticsearchResponse<DynamicResponse> res = null;

            if (searchBodyD == null)
                res = _eshelper.ElasticClient.LowLevel.Search<DynamicResponse>(branch.ToLower(), "product", searchBody);
            else
                res = _eshelper.ElasticClient.LowLevel.Search<DynamicResponse>(branch.ToLower(), "product", searchBodyD);

            int totalCount = Convert.ToInt32(res.Body["hits"]["total"].Value);

            return totalCount;
        }

        public int GetHitsForSearchInIndex(UserSelectedContext catalogInfo, string searchTerm, SearchInputModel searchModel)
        {
            SetWorkingCatalog(searchModel.CatalogType);

            int size = GetProductPagingSize(searchModel.Size);
            //ExpandoObject filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo, department: searchModel.Dept);

            //List<dynamic> newFilterTerms = new List<dynamic>();
            List<dynamic> newFilterTerms = BuildProprietaryItemFilter(catalogInfo, searchModel.Dept);
            if (searchModel.Facets != null && searchModel.Facets.Length > 0)
            {
                newFilterTerms.Add(BuildFacetsFilter(searchModel.Facets));
            }

            string termSearch = searchTerm;
            List<string> fieldsToSearch = null;
            dynamic termSearchExpression = null;

            System.Text.RegularExpressions.Regex matchOnlyDigits = new System.Text.RegularExpressions.Regex(@"^\d+$");

            // results in a search string like '1234 OR upc:*1234 OR gtin:*1234 OR itemnumber:*1234'
            //if (matchOnlyDigits.IsMatch(searchTerm))
            //{
            //    List<string> digitSearchTerms = (Configuration.ElasticSearchDigitSearchFields.Select(x => string.Concat(x + ":*" + searchTerm + "*"))).ToList();
            //    digitSearchTerms.Insert(0, searchTerm);
            //    termSearch = String.Join(" OR ", digitSearchTerms);
            //}
            if (searchTerm != null && matchOnlyDigits.IsMatch(searchTerm))
            {
                fieldsToSearch = Configuration.ElasticSearchDigitSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQueryForDigits(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }
            else
            {
                fieldsToSearch = Configuration.ElasticSearchTermSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQuery(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }

            termSearchExpression = BuildBoolMultiMatchQuery(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            //dynamic termSearchExpression = BuildFunctionScoreQuery(searchModel, filterTerms, fieldsToSearch, termSearch);
            var query = Newtonsoft.Json.JsonConvert.SerializeObject(termSearchExpression);

            string branch = catalogInfo.BranchId.ToLower();

            try {
                ElasticsearchResponse<DynamicResponse> res = _eshelper.ElasticClient.LowLevel.Search<DynamicResponse>(branch.ToLower(), "product", termSearchExpression);
                if (res.Body["hits"]["total"] != null) {
                    if (res.Success) {
                        return Convert.ToInt32(res.Body["hits"]["total"]
                                                  .Value);
                    } else {
                        throw new Exception(string.Format("Error executing ES Query: {0} : {1}", res.ServerError.Error, res.ServerError.Status));
                    }
                } else {
                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private delegate TResult Func<in T, out TResult>();

        private ExpandoObject LoadFacetsFromElasticSearchResponse(ElasticsearchResponse<DynamicResponse> res)
        {
            ExpandoObject facets = new ExpandoObject();

            if (res.Body.Contains("aggregations"))
            {
                BuildFacetsObjectFromResponse(res, facets);

                CorrelateCategoriesToParentCatories(facets);

                AddSpecialFiltersToFacets(facets);
            }

            return facets;
        }

        public void AddSpecialFiltersToFacets(ExpandoObject facets
                                              , string countDeviated = null
                                              , string countRecentOrdered = null)
        {
            List<ExpandoObject> specialFilters = new List<ExpandoObject>();
            AddSpecalFilter(specialFilters,
                            Constants.SPECIALFILTER_DEVIATEDPRICES,
                            Constants.SPECIALFILTER_DEVIATEDPRICES_DESCRIPTION,
                            countDeviated);
            AddSpecalFilter(specialFilters,
                            Constants.SPECIALFILTER_PREVIOUSORDERED,
                            Constants.SPECIALFILTER_PREVIOUSORDERED_DESCRIPTION,
                            countRecentOrdered);
            (facets as IDictionary<string, object>).Add(Constants.SPECIALFILTERS_FACET, specialFilters);
        }

        private void AddSpecalFilter(List<ExpandoObject> specialFilters, string name, string desc, string count)
        {
            dynamic filter = new ExpandoObject();
            filter.count = (count != null) ? count : Constants.SPECIALFILTERS_UNDETERMINEDCOUNT;
            filter.name = name;
            filter.desc = desc;
            specialFilters.Add(filter);
        }

        public void RecalculateFacets(ProductsReturn ret, List<string> specialFilters)
        {
            IDictionary<string, object> dict = ret.Facets as IDictionary<string, object>;
            foreach (var oFacet in dict)
            {
                string name = oFacet.Key;
                if (name.Equals("parentcategories", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcParentCategories(oFacet.Value as List<ExpandoObject>, ret);
                }
                else if (name.Equals("brands", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcFacet(oFacet.Value as List<ExpandoObject>, ret, "brand");
                }
                else if (name.Equals("categories", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcFacet(oFacet.Value as List<ExpandoObject>, ret, "categories");
                }
                else if (name.Equals("mfrname", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcFacet(oFacet.Value as List<ExpandoObject>, ret, "mfrname");
                }
                else if (name.Equals("dietary", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcDietary(oFacet.Value as List<ExpandoObject>, ret);
                }
                else if (name.Equals("nonstock", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcFacet(oFacet.Value as List<ExpandoObject>, ret, "nonstock");
                }
                else if (name.Equals("itemspecs", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcItemspecs(oFacet.Value as List<ExpandoObject>, ret);
                }
                else if (name.Equals("allergens", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcAllergens(oFacet.Value as List<ExpandoObject>, ret);
                }
                else if (name.Equals("specialfilters", StringComparison.CurrentCultureIgnoreCase))
                {
                    RecalcSpecialFilters(oFacet.Value as List<ExpandoObject>, ret, specialFilters);
                }
                else
                {
                    ResetFacet(oFacet.Value as List<ExpandoObject>);
                }
            }
        }

        private void RecalcParentCategories(List<ExpandoObject> list, ProductsReturn ret)
        {
            foreach (var pFacet in list)
            {
                IDictionary<string, object> dict = pFacet as IDictionary<string, object>;
                dict["count"] = ret.Products.Where(p => p.ItemClass == dict["name"].ToString()).Count();
                RecalcFacet(dict["categories"] as List<ExpandoObject>, ret, "categories");
            }
        }

        private void RecalcFacet(List<ExpandoObject> list, ProductsReturn ret, string field)
        {
            foreach (var pFacet in list)
            {
                IDictionary<string, object> dict = pFacet as IDictionary<string, object>;
                Func<Product, bool> where = p => p.BrandExtendedDescription == dict["name"].ToString();
                if (field.Equals("brand", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.BrandExtendedDescription == dict["name"].ToString();
                }
                else if (field.Equals("categories", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.CategoryName == dict["name"].ToString();
                }
                else if (field.Equals("mfrname", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.ManufacturerName == dict["name"].ToString();
                }
                else if (field.Equals("nonstock", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.NonStock == dict["name"].ToString();
                }
                dict["count"] = ret.Products.Where(where).Count();
            }
        }

        private void RecalcDietary(List<ExpandoObject> list, ProductsReturn ret)
        {
            foreach (var pFacet in list)
            {
                IDictionary<string, object> dict = pFacet as IDictionary<string, object>;
                Func<Product, bool> where = null;
                if (dict["name"].ToString().Equals("vegan", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.Nutritional.Diets.Contains("vegan");
                }
                else if (dict["name"].ToString().Equals("kosher", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.Nutritional.Diets.Contains("kosher");
                }
                else if (dict["name"].ToString().Equals("organic", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.Nutritional.Diets.Contains("organic");
                }
                else if (dict["name"].ToString().Equals("halal", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.Nutritional.Diets.Contains("halal");
                }
                dict["count"] = ret.Products
                                   .Where(where).Count();
            }
        }

        private void RecalcItemspecs(List<ExpandoObject> list, ProductsReturn ret)
        {
            foreach (var pFacet in list)
            {
                IDictionary<string, object> dict = pFacet as IDictionary<string, object>;
                Func<Product, bool> where = null;
                if (dict["name"].ToString().Equals("sellsheet", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.SellSheet == "Y";
                }
                else if (dict["name"].ToString().Equals("itembeingreplaced", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.ReplacedItem != "000000";
                }
                else if (dict["name"].ToString().Equals("replacementitem", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = p => p.ReplacementItem != "000000";
                }
                dict["count"] = (where != null) ? ret.Products.Where(where).Count() : 0;
            }
        }

        private void RecalcAllergens(List<ExpandoObject> list, ProductsReturn ret)
        {
            foreach (var pFacet in list)
            {
                IDictionary<string, object> dict = pFacet as IDictionary<string, object>;
                Func<Product, bool> where = p => p.Nutritional != null &&
                                                 p.Nutritional.Allergens != null &&
                                                 p.Nutritional.Allergens.freefrom != null &&
                                                 p.Nutritional.Allergens.freefrom
                                                                        .Contains(dict["name"].ToString());
                dict["count"] = ret.Products.Where(where).Count();
            }
        }

        private void RecalcSpecialFilters(List<ExpandoObject> list, ProductsReturn ret, List<string> specialFilters)
        {
            foreach (var pFacet in list)
            {
                IDictionary<string, object> dict = pFacet as IDictionary<string, object>;
                if (dict["name"].ToString()
                    .Equals(Constants.SPECIALFILTER_DEVIATEDPRICES, StringComparison.CurrentCultureIgnoreCase) &&
                    specialFilters.Contains(Constants.SPECIALFILTER_DEVIATEDPRICES))
                {
                    dict["count"] = ret.TotalCount;
                }
                else if (dict["name"].ToString()
                    .Equals(Constants.SPECIALFILTER_PREVIOUSORDERED, StringComparison.CurrentCultureIgnoreCase) &&
                    specialFilters.Contains(Constants.SPECIALFILTER_PREVIOUSORDERED))
                {
                    dict["count"] = ret.TotalCount;
                }
                else
                {
                    dict["count"] = 0;
                }
            }
        }

        private void ResetFacet(List<ExpandoObject> list)
        {
            foreach (var pFacet in list)
            {
                IDictionary<string, object> sdict = pFacet as IDictionary<string, object>;
                sdict["count"] = 0;
            }
        }

        private void CorrelateCategoriesToParentCatories(ExpandoObject facets)
        {
            List<ExpandoObject> parentcategories = (List<ExpandoObject>)(facets as IDictionary<string, object>)["parentcategories"];
            List<ExpandoObject> categories = (List<ExpandoObject>)(facets as IDictionary<string, object>)["categories"];
            if (parentcategories != null && categories != null)
            {
                foreach (IDictionary<string, object> parent in parentcategories)
                {
                    string pcode = parent["code"].ToString();
                    List<ExpandoObject> cats = new List<ExpandoObject>();
                    foreach (IDictionary<string, object> cat in categories)
                    {
                        string code = cat["code"].ToString();
                        AssignCategoryToParentCategoryBasedOnCatalog(pcode, cats, cat, code);
                    }
                    parent.Add("categories", cats);
                }
            }
        }

        private void AssignCategoryToParentCategoryBasedOnCatalog(string pcode, List<ExpandoObject> cats, IDictionary<string, object> cat, string code)
        {
            if (_catalog.StartsWith("unfi", StringComparison.CurrentCultureIgnoreCase)
                && code.StartsWith(pcode.Substring(0, 1)))
            {
                cats.Add((ExpandoObject)cat);
            }
            else if (_catalog.StartsWith("unfi", StringComparison.CurrentCultureIgnoreCase) == false
                     && code.StartsWith(pcode))
            {
                cats.Add((ExpandoObject)cat);
            }
        }

        private void BuildFacetsObjectFromResponse(ElasticsearchResponse<DynamicResponse> res, ExpandoObject facets)
        {
            foreach (var oFacet in res.Body["aggregations"])
            {
                var facet = new List<ExpandoObject>();
                foreach (var oFacetValue in oFacet.Value["buckets"])
                {
                    BuildBaseFacets(oFacet, facet, oFacetValue);
                }
                (facets as IDictionary<string, object>).Add(oFacet.Path, facet);
            }
        }

        private void BuildBaseFacets(dynamic oFacet, List<ExpandoObject> facet, dynamic oFacetValue)
        {
            var facetValue = new ExpandoObject() as IDictionary<string, object>;
            string key = oFacetValue["key"].ToString().ToLower();
            int count = oFacetValue["doc_count"];
            facetValue.Add(new KeyValuePair<string, object>("name", key));
            facetValue.Add(new KeyValuePair<string, object>("count", count));
            if (oFacet.Path == "categories")
            {
                BuildCategoryFacet(oFacetValue, facetValue);
            }
            else if (oFacet.Path == "parentcategories")
            {
                BuildParentCategoryFacet(oFacetValue, facetValue);
            }
            else if (oFacet.Path == "brands")
            {
                BuildBrandsFacet(oFacetValue, facetValue);
            }
            else if (oFacet.Path == "temp_zone")
            {
                BuildTempZoneFacet(oFacetValue, facetValue);
            }
            facet.Add(facetValue as ExpandoObject);
        }

        private void BuildTempZoneFacet(dynamic oFacetValue, IDictionary<string, object> facetValue)
        {
            facetValue.Add(new KeyValuePair<string, object>("description", GetTemperatureZoneDescription(oFacetValue["key"].Value)));
        }

        private void BuildBrandsFacet(dynamic oFacetValue, IDictionary<string, object> facetValue)
        {
            if (oFacetValue["brand_meta"]["buckets"].Count > 0)
            {
                facetValue.Add(new KeyValuePair<string, object>("brand_control_label", oFacetValue["brand_meta"]["buckets"][0]["key"].Value.ToString()));
            }
            else
            {
                facetValue.Add(new KeyValuePair<string, object>("brand_control_label", null));
            }
        }

        private void BuildParentCategoryFacet(dynamic oFacetValue, IDictionary<string, object> facetValue)
        {
            if (oFacetValue["parentcategory_code"] != null &&
                oFacetValue["parentcategory_code"]["buckets"] != null &&
                (oFacetValue["parentcategory_code"]["buckets"]).Count > 0)
            {
                facetValue.Add(new KeyValuePair<string, object>("code", oFacetValue["parentcategory_code"]["buckets"][0]["key"].Value.ToString()));
            }
        }

        private void BuildCategoryFacet(dynamic oFacetValue, IDictionary<string, object> facetValue)
        {
            facetValue.Add(new KeyValuePair<string, object>("categoryname", oFacetValue["category_meta"]["buckets"][0]["key"].Value.ToString()));
            if (oFacetValue["category_code"] != null &&
               oFacetValue["category_code"]["buckets"] != null &&
               (oFacetValue["category_code"]["buckets"]).Count > 0)
            {
                facetValue.Add(new KeyValuePair<string, object>("code", oFacetValue["category_code"]["buckets"][0]["key"].Value.ToString()));
            }
        }

        public Product LoadProductFromElasticSearchProduct(bool listonly, dynamic oProd)
        {
            Product p = new Product();
            GetBaseProductProperties(oProd, p);
            GetBaseNutritionalProperties(oProd, p);
            GetBaseUnfiProperties(oProd, p);
            p.IsValid = true;
            //            if(true)
            if (listonly == false)
            {
                GetFullProductProperties(oProd, p);
                if (p.CatalogId.ToLower().StartsWith("unfi"))
                {
                    GetFullUnfiProperties(oProd, p);
                }

                if (oProd._source.nutritional != null)
                {
                    GetFullNutritionalProperties(oProd, p);
                }
            }

            return p;
        }

        private void GetFullNutritionalProperties(dynamic oProd, Product p)
        {
            Nutritional nutritional = p.Nutritional;
            nutritional.BrandOwner = oProd._source.nutritional.brandowner;
            nutritional.CountryOfOrigin = oProd._source.nutritional.countryoforigin;
            nutritional.GrossWeight = oProd._source.nutritional.grossweight;
            nutritional.HandlingInstructions = oProd._source.nutritional.handlinginstruction;
            nutritional.Ingredients = oProd._source.nutritional.ingredients;
            nutritional.MarketingMessage = oProd._source.nutritional.marketingmessage;
            nutritional.MoreInformation = oProd._source.nutritional.moreinformation;
            nutritional.ServingSizeUOM = oProd._source.nutritional.servingsizeuom;
            nutritional.ServingSugestion = oProd._source.nutritional.servingsuggestions;
            nutritional.Shelf = oProd._source.nutritional.shelf;
            nutritional.StorageTemp = oProd._source.nutritional.storagetemp;
            nutritional.UnitMeasure = oProd._source.nutritional.unitmeasure;
            nutritional.UnitsPerCase = oProd._source.nutritional.unitspercase;
            nutritional.Volume = oProd._source.nutritional.volume;
            nutritional.Height = oProd._source.nutritional.height;
            nutritional.Length = oProd._source.nutritional.length;
            nutritional.Width = oProd._source.nutritional.width;
            nutritional.DietInfo = new List<Diet>();
            nutritional.Allergens = new Allergen();
            nutritional.Diets = new List<string>();
            if (oProd._source.nutritional.allergen.Value != null)
            {
                GetFullNutritionalAllergenProperties(oProd, nutritional);
            }

            nutritional.NutritionInfo = new List<Nutrition>();
            if (oProd._source.nutritional.nutrition.Value != null)
            {
                GetFullNutritionalNutritionInfo(oProd, nutritional);
            }
            if (oProd._source.nutritional.diet.Value != null)
            {
                foreach (var diet in oProd._source.nutritional.diet)
                {
                    Diet d = new Diet() { DietType = diet.diettype, Value = diet.value };
                    nutritional.Diets.Add(diet.diettype.Value);
                    nutritional.DietInfo.Add(d);
                }
            }
        }

        private void GetFullNutritionalNutritionInfo(dynamic oProd, Nutritional nutritional)
        {
            foreach (var nutrition in oProd._source.nutritional.nutrition)
            {
                Nutrition n = new Nutrition() {
                    DailyValue = nutrition.dailyvalue,
                    MeasurementTypeId = nutrition.measurementtypeid,
                    MeasurementValue = nutrition.measurementvalue,
                    NutrientType = nutrition.nutrienttype,
                    NutrientTypeCode = nutrition.nutrienttypecode
                };
                nutritional.NutritionInfo.Add(n);
            }
        }

        private void GetFullNutritionalAllergenProperties(dynamic oProd, Nutritional nutritional)
        {
            if (oProd._source.nutritional.allergen.freefrom != null)
            {
                nutritional.Allergens.freefrom = new List<string>();
                foreach (var ff in oProd._source.nutritional.allergen.freefrom)
                    nutritional.Allergens.freefrom.Add(((string)ff.Value).ToLower());
            }

            if (oProd._source.nutritional.allergen.contains != null)
            {
                nutritional.Allergens.contains = new List<string>();
                foreach (var ff in oProd._source.nutritional.allergen.contains)
                    nutritional.Allergens.contains.Add(ff.Value);
            }

            if (oProd._source.nutritional.allergen.maycontain != null)
            {
                nutritional.Allergens.maycontain = new List<string>();
                foreach (var ff in oProd._source.nutritional.allergen.maycontain)
                    nutritional.Allergens.maycontain.Add(ff.Value);
            }
        }

        private void GetFullUnfiProperties(dynamic oProd, Product p)
        {
            //make vendor into description
            p.Description = oProd._source.vendor1;
            p.IsSpecialtyCatalog = true;

            UNFI unfi = p.Unfi;

            unfi.CaseHeight = oProd._source.cheight.ToString();
            unfi.CaseLength = oProd._source.clength.ToString();
            unfi.CaseWidth = oProd._source.cwidth.ToString();
            unfi.Weight = oProd._source.averageweight.ToString();
            unfi.UnitOfSale = oProd._source.unitofsale.ToString();
            unfi.CatalogDept = oProd._source.catalogdept.ToString();
            unfi.ShipMinExpire = oProd._source.shipminexpire.ToString();
            unfi.MinOrder = oProd._source.minorder.ToString();
            unfi.CaseQuantity = oProd._source.casequantity.ToString();
            unfi.PutUp = oProd._source.putup.ToString();
            unfi.ContUnit = oProd._source.contunit.ToString();
            unfi.TCSCode = oProd._source.tcscode.ToString();
            unfi.CaseUPC = oProd._source.caseupc.ToString();
            unfi.PackageLength = oProd._source.plength.ToString();
            unfi.PackageHeight = oProd._source.pheight.ToString();
            unfi.PackageWidth = oProd._source.pwidth.ToString();
            unfi.Status = oProd._source.status.ToString();
            unfi.PackagePrice = oProd._source.packageprice.ToString();
            unfi.CasePrice = oProd._source.caseprice.ToString();
            unfi.Flag1 = oProd._source.flag1.ToString();
            unfi.Flag2 = oProd._source.flag2.ToString();
            unfi.Flag3 = oProd._source.flag3.ToString();
            unfi.Flag4 = oProd._source.flag4.ToString();
            unfi.OnHandQty = oProd._source.onhandqty.ToString();
            unfi.Vendor = oProd._source.vendor1.ToString();
            unfi.StockedInBranches = oProd._source.stockedinbranches.ToString();

            p.Unfi = unfi;
        }

        private void GetFullProductProperties(dynamic oProd, Product p)
        {
            p.ManufacturerNumber = oProd._source.mfrnumber;
            p.BrandControlLabel = oProd._source.brand_control_label;
            p.ExtendedDescription = string.Empty;
            p.SubCategoryCode = oProd._source.categoryid;
            p.VendorItemNumber = oProd._source.vendor1;
            try
            {
                p.CaseCube = oProd._source.icube;
            }
            catch// (Exception e)
            {
                p.CaseCube = "";
            }
        }

        private void GetBaseUnfiProperties(dynamic oProd, Product p)
        {
            if (p.CatalogId.ToLower().StartsWith("unfi"))
            {
                //make vendor into description
                p.Description = oProd._source.vendor1;
                p.Pack = oProd._source.casequantity.ToString();
                p.Size = oProd._source.contsize.ToString() + oProd._source.contunit;
                p.IsSpecialtyCatalog = true;
                p.Cases = oProd._source.onhandqty.ToString();
                p.SpecialtyItemCost = (decimal)p.CasePriceNumeric;
                p.CasePrice = oProd._source.caseprice.ToString();
                if (oProd._source.tcscode != null)
                    p.SubCategoryCode = oProd._source.tcscode.ToString();
                if (oProd._source.packageprice != null)
                    p.PackagePrice = oProd._source.packageprice.ToString();
                if (oProd._source.caseprice != null)
                    p.CasePrice = oProd._source.caseprice.ToString();
                UNFI unfi = new UNFI();

                if (oProd._source.cheight != null)
                    unfi.CaseHeight = oProd._source.cheight.ToString();
                if (oProd._source.packageprice != null)
                    unfi.PackagePrice = oProd._source.packageprice.ToString();
                if (oProd._source.caseprice != null)
                    unfi.CasePrice = oProd._source.caseprice.ToString();
                p.Unfi = unfi;
            }
        }

        private void GetBaseNutritionalProperties(dynamic oProd, Product p)
        {
            if (oProd._source.nutritional != null)
            {
                Nutritional nutritional = new Nutritional();
                nutritional.Diets = new List<string>();
                if (oProd._source.nutritional.diet != null)
                {
                    var diets = oProd._source.nutritional.diet;
                    foreach (var diet in diets)
                    {
                        String type = diet["diettype"];
                        nutritional.Diets.Add(type);
                    }
                }
                nutritional.ServingSize = oProd._source.nutritional.servingsize;
                nutritional.ServingsPerPack = oProd._source.nutritional.servingsperpack;
                p.Nutritional = nutritional;
            }
        }

        private void GetBaseProductProperties(dynamic oProd, Product p)
        {
            p.CatalogId = oProd._index;
            p.ManufacturerName = oProd._source.mfrname;
            p.ItemNumber = oProd._id;
            String isKosher = oProd._source.kosher;
            p.Kosher = string.IsNullOrEmpty(isKosher) ? "Unknown" : isKosher;
            p.Brand = oProd._source.brand;
            p.BrandExtendedDescription = oProd._source.brand_description;
            if (oProd._source.cases != null) {
                p.Cases = oProd._source.cases.ToString();
            }
            p.Description = oProd._source.description;
            p.CategoryCode = oProd._source.categoryid;
            p.ReplacedItem = oProd._source.replaceditem;
            p.ReplacementItem = oProd._source.replacementitem;
            p.UPC = oProd._source.upc;
            p.Name = oProd._source.name;
            p.CategoryName = oProd._source.categoryname;
            p.ItemClass = oProd._source.parentcategoryname;
            p.Pack = oProd._source.pack;
            p.Size = oProd._source.size;
            p.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                     p.Name,
                                     p.ItemNumber,
                                     p.BrandExtendedDescription,
                                     p.ItemClass,
                                     p.PackSize);
            p.CaseOnly = oProd._source.caseonly == "Y";
            p.TempZone = oProd._source.temp_zone;
            p.IsProprietary = oProd._source.isproprietary;
            p.ProprietaryCustomers = oProd._source.proprietarycustomers;
            p.CatchWeight = oProd._source.catchweight;
            p.AverageWeight = oProd._source.averageweight;
            p.CasePriceNumeric = oProd._source.caseprice.Value != null ? oProd._source.caseprice : 0.00;
            p.CasePrice = p.CasePriceNumeric.ToString();
            p.PackagePriceNumeric = oProd._source.packageprice.Value != null ? oProd._source.packageprice : 0.00;
            p.PackagePrice = p.PackagePriceNumeric.ToString();
            p.SellSheet = oProd._source.sellsheet;
            p.ChildNutrition = oProd._source.childnutrition;
            p.NonStock = oProd._source.nonstock;
            p.MarketingBrand = oProd._source.marketing_brand;
            p.MarketingDescription = oProd._source.marketing_description;
            p.MarketingManufacturer = oProd._source.marketing_manufacturer;
            p.MarketingName = oProd._source.marketing_name;
            p.Status1 = oProd._source.status1;
        }

        private void SetWorkingCatalog(string catalogId)
        {
            if (catalogId != null)
            {
                _catalog = catalogId;
            }
            else
            {
                _catalog = "bek";
            }
        }

        public Product LoadProductFromElasticSearchProductFromHighLevelCall(bool listonly, dynamic oProd)
        {
            Product p = new Product();
            GetBaseProductPropertiesFromHighLevelCall(oProd, p);
            GetBaseNutritionalPropertiesFromHighLevelCall(oProd, p);
            GetBaseUnfiPropertiesFromHighLevelCall(oProd, p);
            p.IsValid = true;
            //            if(true)
            if (listonly == false)
            {
                GetFullProductPropertiesFromHighLevelCall(oProd, p);
                if (p.CatalogId.ToLower().StartsWith("unfi"))
                {
                    GetFullUnfiPropertiesFromHighLevelCall(oProd, p);
                }

                if (oProd.Source.nutritional != null)
                {
                    GetFullNutritionalPropertiesFromHighLevelCall(oProd, p);
                }
            }

            return p;
        }

        private void GetFullNutritionalPropertiesFromHighLevelCall(dynamic oProd, Product p)
        {
            Nutritional nutritional = p.Nutritional;
            nutritional.BrandOwner = oProd.Source.nutritional.brandowner;
            nutritional.CountryOfOrigin = oProd.Source.nutritional.countryoforigin;
            nutritional.GrossWeight = oProd.Source.nutritional.grossweight;
            nutritional.HandlingInstructions = oProd.Source.nutritional.handlinginstruction;
            nutritional.Ingredients = oProd.Source.nutritional.ingredients;
            nutritional.MarketingMessage = oProd.Source.nutritional.marketingmessage;
            nutritional.MoreInformation = oProd.Source.nutritional.moreinformation;
            nutritional.ServingSizeUOM = oProd.Source.nutritional.servingsizeuom;
            nutritional.ServingSugestion = oProd.Source.nutritional.servingsuggestions;
            nutritional.Shelf = oProd.Source.nutritional.shelf;
            nutritional.StorageTemp = oProd.Source.nutritional.storagetemp;
            nutritional.UnitMeasure = oProd.Source.nutritional.unitmeasure;
            nutritional.UnitsPerCase = oProd.Source.nutritional.unitspercase;
            nutritional.Volume = oProd.Source.nutritional.volume;
            nutritional.Height = oProd.Source.nutritional.height;
            nutritional.Length = oProd.Source.nutritional.length;
            nutritional.Width = oProd.Source.nutritional.width;
            nutritional.DietInfo = new List<Diet>();
            nutritional.Allergens = new Allergen();
            nutritional.Diets = new List<string>();
            if (oProd.Source.nutritional.allergen != null)
            {
                GetFullNutritionalAllergenPropertiesFromHighLevelCall(oProd, nutritional);
            }

            nutritional.NutritionInfo = new List<Nutrition>();
            if (oProd.Source.nutritional.nutrition != null)
            {
                GetFullNutritionalNutritionInfoFromHighLevelCall(oProd, nutritional);
            }
            if (oProd.Source.nutritional.diet != null)
            {
                foreach (var diet in oProd.Source.nutritional.diet)
                {
                    Diet d = new Diet() { DietType = diet.diettype, Value = diet.value };
                    nutritional.Diets.Add(diet.diettype.Value);
                    nutritional.DietInfo.Add(d);
                }
            }
        }

        private void GetFullNutritionalNutritionInfoFromHighLevelCall(dynamic oProd, Nutritional nutritional)
        {
            foreach (var nutrition in oProd.Source.nutritional.nutrition)
            {
                Nutrition n = new Nutrition()
                {
                    DailyValue = nutrition.dailyvalue,
                    MeasurementTypeId = nutrition.measurementtypeid,
                    MeasurementValue = nutrition.measurementvalue,
                    NutrientType = nutrition.nutrienttype,
                    NutrientTypeCode = nutrition.nutrienttypecode
                };
                nutritional.NutritionInfo.Add(n);
            }
        }

        private void GetFullNutritionalAllergenPropertiesFromHighLevelCall(dynamic oProd, Nutritional nutritional)
        {
            if (oProd.Source.nutritional.allergen.freefrom != null)
            {
                nutritional.Allergens.freefrom = new List<string>();
                foreach (var ff in oProd.Source.nutritional.allergen.freefrom)
                    nutritional.Allergens.freefrom.Add(((string)ff.Value).ToLower());
            }

            if (oProd.Source.nutritional.allergen.contains != null)
            {
                nutritional.Allergens.contains = new List<string>();
                foreach (var ff in oProd.Source.nutritional.allergen.contains)
                    nutritional.Allergens.contains.Add(ff.Value);
            }

            if (oProd.Source.nutritional.allergen.maycontain != null)
            {
                nutritional.Allergens.maycontain = new List<string>();
                foreach (var ff in oProd.Source.nutritional.allergen.maycontain)
                    nutritional.Allergens.maycontain.Add(ff.Value);
            }
        }

        private void GetFullUnfiPropertiesFromHighLevelCall(dynamic oProd, Product p)
        {
            //make vendor into description
            p.Description = oProd.Source.vendor1;
            p.IsSpecialtyCatalog = true;

            UNFI unfi = p.Unfi;

            unfi.CaseHeight = oProd.Source.cheight.ToString();
            unfi.CaseLength = oProd.Source.clength.ToString();
            unfi.CaseWidth = oProd.Source.cwidth.ToString();
            unfi.Weight = oProd.Source.averageweight.ToString();
            unfi.UnitOfSale = oProd.Source.unitofsale.ToString();
            unfi.CatalogDept = oProd.Source.catalogdept.ToString();
            unfi.ShipMinExpire = oProd.Source.shipminexpire.ToString();
            unfi.MinOrder = oProd.Source.minorder.ToString();
            unfi.CaseQuantity = oProd.Source.casequantity.ToString();
            unfi.PutUp = oProd.Source.putup.ToString();
            unfi.ContUnit = oProd.Source.contunit.ToString();
            unfi.TCSCode = oProd.Source.tcscode.ToString();
            unfi.CaseUPC = oProd.Source.caseupc.ToString();
            unfi.PackageLength = oProd.Source.plength.ToString();
            unfi.PackageHeight = oProd.Source.pheight.ToString();
            unfi.PackageWidth = oProd.Source.pwidth.ToString();
            unfi.Status = oProd.Source.status.ToString();
            unfi.PackagePrice = oProd.Source.packageprice.ToString();
            unfi.CasePrice = oProd.Source.caseprice.ToString();
            unfi.Flag1 = oProd.Source.flag1.ToString();
            unfi.Flag2 = oProd.Source.flag2.ToString();
            unfi.Flag3 = oProd.Source.flag3.ToString();
            unfi.Flag4 = oProd.Source.flag4.ToString();
            unfi.OnHandQty = oProd.Source.onhandqty.ToString();
            unfi.Vendor = oProd.Source.vendor1.ToString();
            unfi.StockedInBranches = oProd.Source.stockedinbranches.ToString();

            p.Unfi = unfi;
        }

        private void GetFullProductPropertiesFromHighLevelCall(dynamic oProd, Product p)
        {
            p.ManufacturerNumber = oProd.Source.mfrnumber;
            p.BrandControlLabel = oProd.Source.brand_control_label;
            p.ExtendedDescription = string.Empty;
            p.SubCategoryCode = oProd.Source.categoryid;
            p.VendorItemNumber = oProd.Source.vendor1;
            try
            {
                p.CaseCube = oProd.Source.icube;
            }
            catch// (Exception e)
            {
                p.CaseCube = "";
            }
        }

        private void GetBaseUnfiPropertiesFromHighLevelCall(dynamic oProd, Product p)
        {
            if (p.CatalogId.ToLower().StartsWith("unfi"))
            {
                //make vendor into description
                p.Description = oProd.Source.vendor1;
                p.Pack = oProd.Source.casequantity.ToString();
                p.Size = oProd.Source.contsize.ToString() + oProd.Source.contunit;
                p.IsSpecialtyCatalog = true;
                p.Cases = oProd.Source.onhandqty.ToString();
                p.SpecialtyItemCost = (decimal)p.CasePriceNumeric;
                p.CasePrice = oProd.Source.caseprice.ToString();
                if (oProd.Source.tcscode != null)
                    p.SubCategoryCode = oProd.Source.tcscode.ToString();
                if (oProd.Source.packageprice != null)
                    p.PackagePrice = oProd.Source.packageprice.ToString();
                if (oProd.Source.caseprice != null)
                    p.CasePrice = oProd.Source.caseprice.ToString();
                UNFI unfi = new UNFI();

                if (oProd.Source.cheight != null)
                    unfi.CaseHeight = oProd.Source.cheight.ToString();
                if (oProd.Source.packageprice != null)
                    unfi.PackagePrice = oProd.Source.packageprice.ToString();
                if (oProd.Source.caseprice != null)
                    unfi.CasePrice = oProd.Source.caseprice.ToString();
                p.Unfi = unfi;
            }
        }

        private void GetBaseNutritionalPropertiesFromHighLevelCall(dynamic oProd, Product p)
        {
            if (oProd.Source.nutritional != null)
            {
                Nutritional nutritional = new Nutritional();
                nutritional.Diets = new List<string>();
                if (oProd.Source.nutritional.diet != null)
                {
                    var diets = oProd.Source.nutritional.diet;
                    foreach (var diet in diets)
                    {
                        String type = diet["diettype"];
                        nutritional.Diets.Add(type);
                    }
                }
                nutritional.ServingSize = oProd.Source.nutritional.servingsize;
                nutritional.ServingsPerPack = oProd.Source.nutritional.servingsperpack;
                p.Nutritional = nutritional;
            }
        }

        private void GetBaseProductPropertiesFromHighLevelCall(dynamic oProd, Product p)
        {
            p.CatalogId = oProd.Index;
            p.ManufacturerName = oProd.Source.mfrname;
            p.ItemNumber = oProd.Id;
            String isKosher = oProd.Source.kosher;
            p.Kosher = string.IsNullOrEmpty(isKosher) ? "Unknown" : isKosher;
            p.Brand = oProd.Source.brand;
            p.BrandExtendedDescription = oProd.Source.brand_description;
            if (oProd.Source.cases != null)
            {
                p.Cases = oProd.Source.cases.ToString();
            }
            p.Description = oProd.Source.description;
            p.CategoryCode = oProd.Source.categoryid;
            p.ReplacedItem = oProd.Source.replaceditem;
            p.ReplacementItem = oProd.Source.replacementitem;
            p.UPC = oProd.Source.upc;
            p.Name = oProd.Source.name;
            p.CategoryName = oProd.Source.categoryname;
            p.ItemClass = oProd.Source.parentcategoryname;
            p.Pack = oProd.Source.pack;
            p.Size = oProd.Source.size;
            p.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                     p.Name,
                                     p.ItemNumber,
                                     p.BrandExtendedDescription,
                                     p.ItemClass,
                                     p.PackSize);
            p.CaseOnly = oProd.Source.caseonly == "Y";
            p.TempZone = oProd.Source.temp_zone;
            p.IsProprietary = oProd.Source.isproprietary;
            p.ProprietaryCustomers = oProd.Source.proprietarycustomers;
            p.CatchWeight = oProd.Source.catchweight;
            p.AverageWeight = oProd.Source.averageweight;
            p.CasePriceNumeric = oProd.Source.caseprice != null ? oProd.Source.caseprice : 0.00;
            p.CasePrice = p.CasePriceNumeric.ToString();
            p.PackagePriceNumeric = oProd.Source.packageprice != null ? oProd.Source.packageprice : 0.00;
            p.PackagePrice = p.PackagePriceNumeric.ToString();
            p.SellSheet = oProd.Source.sellsheet;
            p.ChildNutrition = oProd.Source.childnutrition;
            p.NonStock = oProd.Source.nonstock;
            p.MarketingBrand = oProd.Source.marketing_brand;
            p.MarketingDescription = oProd.Source.marketing_description;
            p.MarketingManufacturer = oProd.Source.marketing_manufacturer;
            p.MarketingName = oProd.Source.marketing_name;
            p.Status1 = oProd.Source.status1;
        }

        #endregion

        #region properties
        public ExpandoObject ElasticSearchAggregations {
            get {
                if (!String.IsNullOrEmpty(Configuration.ElasticSearchAggregations))
                {
                    ExpandoObject aggregationsFromConfig = new ExpandoObject();

                    foreach (string aggregation in Configuration.ElasticSearchAggregations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] aggregationParams = aggregation.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (aggregationParams.Length != 2)
                            throw new ApplicationException("Incorrect aggreation configuration");

                        if (aggregationParams[0] == "categories")
                        {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0],
                                new {
                                    terms = new { field = aggregationParams[1], size = 500 },
                                    aggregations = new {
                                        category_meta = new { terms = new { field = "categoryname_not_analyzed", size = 500 } },
                                        category_code = new {
                                            terms = new {
                                                field =
                                        ((_catalog.StartsWith("unfi", StringComparison.CurrentCultureIgnoreCase)) ? "tcscode.keyword" : "categoryid.keyword"),
                                                size = 500
                                            }
                                        }
                                    }
                                });
                        }
                        else if (aggregationParams[0] == "parentcategories")
                        {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0],
                                new {
                                    terms = new { field = aggregationParams[1], size = 500 },
                                    aggregations = new {
                                        parentcategory_code = new {
                                            terms = new {
                                                field =
                                        ((_catalog.StartsWith("unfi", StringComparison.CurrentCultureIgnoreCase)) ? "catalogdept.keyword" : "parentcategoryid.keyword"),
                                                size = 500
                                            }
                                        }
                                    }
                                });
                        }
                        else if (aggregationParams[0] == "brands")
                        {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0], new { terms = new { field = aggregationParams[1], size = 500 }, aggregations = new { brand_meta = new { terms = new { field = "brand_control_label.keyword", size = 500 } } } });
                        }
                        else
                        {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0], new { terms = new { field = aggregationParams[1], size = 500 } });
                        }
                    }
                    return aggregationsFromConfig;
                }
                return new ExpandoObject();
            }
        }

        public Dictionary<string, string> ElasticSearchAggregationsMap {
            get {
                Dictionary<string, string> val = new Dictionary<string, string>();
                foreach (string aggregation in Configuration.ElasticSearchAggregations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] aggregationParams = aggregation.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (aggregationParams.Length != 2)
                        throw new ApplicationException("Incorrect aggreation configuration");

                    val.Add(aggregationParams[0], aggregationParams[1]);
                }
                return val;
            }
        }
        #endregion
    } // end class
} // end namespace
