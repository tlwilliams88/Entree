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

namespace KeithLink.Svc.Impl.Repository.SiteCatalog {
    public class ElasticSearchCatalogRepositoryImpl : ICatalogRepository {
        #region attributes
        private Helpers.ElasticSearch _eshelper;
        private ElasticsearchClient _client;
        #endregion

        #region constructor
        public ElasticSearchCatalogRepositoryImpl() {
            _eshelper = new Helpers.ElasticSearch();
            _client = GetElasticsearchClient(Configuration.ElasticSearchURL);
        }
        #endregion

        #region methods
        private dynamic BuildBoolFunctionScoreQuery(int from, int size, string sortField, string sortDir, ExpandoObject query) {
            return new
            {
                from = from,
                size = size,
                query = new
                {
                    function_score = new
                    {
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

        private dynamic BuildBoolMultiMatchQuery(SearchInputModel searchModel, List<dynamic> filterTerms, List<string> fieldsToSearch, string searchExpression) {
            List<dynamic> statusFields = BuildStatusFilter();

            List<dynamic> musts = new List<dynamic>();
            musts.Add( new {
                multi_match = new {
                    query = searchExpression,
                    @type = "most_fields",
                    fields = fieldsToSearch,
                    @operator = "and"
                },
            } );

            musts.Add( filterTerms );

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

        private dynamic BuildBoolMultiMatchQueryForDigits(SearchInputModel searchModel, List<dynamic> filterTerms, List<string> fieldsToSearch, string searchExpression) {
            List<dynamic> statusFields = BuildStatusFilter();
            string wildcardText = string.Format("*{0}*", searchExpression);
            List<dynamic> shoulds = new List<dynamic>();
            shoulds.Add(new {
                match = new {
                    itemnumber = new {
                        query = searchExpression,
                        boost = 5
                    }
                }
            });
            shoulds.Add(new {
                wildcard = new {
                    itemnumber = new {
                        value = wildcardText,
                        boost = 5
                    }
                }
            });
            shoulds.Add(new {
                match = new {
                    mfrnumber = new {
                        query = searchExpression,
                        boost = 3
                    }
                }
            });
            shoulds.Add(new {
                wildcard = new {
                    mfrnumber = new {
                        value = wildcardText,
                        boost = 3
                    }
                }
            });
            shoulds.Add(new {
                match = new {
                    gtin = new {
                        query = searchExpression
                    }
                }
            });
            shoulds.Add(new {
                wildcard = new {
                    gtin = new {
                        value = wildcardText
                    }
                }
            });
            shoulds.Add(new {
                match = new {
                    upc = new {
                        query = searchExpression
                    }
                }
            });
            shoulds.Add(new {
                wildcard = new {
                    upc = new {
                        value = wildcardText
                    }
                }
            });
            shoulds.AddRange(filterTerms);

            return new {
                from = searchModel.From,
                size = searchModel.Size,
                query = new {
                    @bool = new {
                        should = shoulds,
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
        private static dynamic BuildCategoryFilter(string category) {
            return new { multi_match =
                    new { query = category, fields = 
                        new List<string>() { "categoryname_not_analyzed", "parentcategoryname_not_analyzed", "categoryid", "parentcategoryid" } } };
        }

        private dynamic BuildDigitMatchQuery(string fieldName, string searchText, int? fieldBoost) {
            dynamic query = null;

            if (fieldBoost.HasValue) {
                query = new {
                    match = new {
                        fieldName = new {
                            query = searchText,
                            boost = fieldBoost
                        }
                    }
                };
            } else {
                query = new {
                    match = new {
                        fieldName = new {
                            query = searchText
                        }
                    }
                };
            }

            return query;
        }

        private List<dynamic> BuildDigitQuery(List<string> searchFields, string searchTerms) {
            const char SPLIT_CHAR = '^';
            const Int16 INDEX_FIELDNAME = 0;
            const Int16 INDEX_BOOST = 1;
            const Int16 INDEX_NOTFOUND = -1;

            List<dynamic> searchClauses = new List<dynamic>();

            foreach (string searchField in searchFields) {
                if (searchField.IndexOf(SPLIT_CHAR) == INDEX_NOTFOUND) {
                    searchClauses.Add(BuildDigitMatchQuery(searchField, searchTerms, null));
                    searchClauses.Add(BuildDigitWildcardQuery(searchField, searchTerms, null));
                } else {
                    string[] fieldBoost = searchField.Split(SPLIT_CHAR);
                    int boost = int.Parse(fieldBoost[INDEX_BOOST]);

                    searchClauses.Add(BuildDigitMatchQuery(fieldBoost[INDEX_FIELDNAME], searchTerms, boost));
                    searchClauses.Add(BuildDigitWildcardQuery(fieldBoost[INDEX_FIELDNAME], searchTerms, boost));
                }
            }

            return searchClauses;
        }

        private dynamic BuildDigitWildcardQuery(string fieldName, string searchText, int? fieldBoost) {
            dynamic query = null;
            string text = string.Format("*{0}*", searchText);

            if (fieldBoost.HasValue) {
                query = new {
                    wildcard = new {
                        fieldName = new {
                            value = text,
                            boost = fieldBoost
                        }
                    }
                };
            } else {
                query = new {
                    wildcard = new {
                        value = new {
                            query = text
                        }
                    }
                };
            }

            return query;
        }

        private dynamic BuildProprietaryItemFilter( UserSelectedContext catalogInfo, string department ) {
            List<dynamic> proprietaryItems = new List<dynamic>();

            //Build filter for proprietary items
			if(!string.IsNullOrEmpty(catalogInfo.CustomerId))
				proprietaryItems.Add(new { query_string = new { query = string.Format("isproprietary:false OR (isproprietary:true AND proprietarycustomers: {0})", catalogInfo.CustomerId) } });
			else
				proprietaryItems.Add(new { match = new { isproprietary = false } }); //No CustomerId (Guest), filter out all proprietary items

            //if (!string.IsNullOrEmpty(department)) {
            //    proprietaryItems.Add(new { match = new { @department = department } });
            //}

            return proprietaryItems;
        }

        private List<dynamic> BuildFacetsFilter( string facetFilters ) {
            List<dynamic> mustClause = new List<dynamic>();
            string facetSeparator = "___";
            string[] facets = facetFilters.Split(new string[] { facetSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] values = s.Substring(s.IndexOf(":") + 1).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                string keyValue = ElasticSearchAggregationsMap[keyValues[0]];
                string selectedValues = String.Join("\",\"", values);
                ExpandoObject keyValueSelectedValues = new ExpandoObject();
                (keyValueSelectedValues as IDictionary<string, object>).Add(keyValue, values);
                mustClause.Add(new { terms = keyValueSelectedValues });
            }

            return mustClause;
        }

        private dynamic BuildFilterTerms(string facetFilters, UserSelectedContext catalogInfo, string category = "", string department = "") {
            List<dynamic> mustClause = new List<dynamic>();
            string facetSeparator = "___";
            string[] facets = facetFilters.Split(new string[] { facetSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] values = s.Substring(s.IndexOf(":") + 1).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                string keyValue = ElasticSearchAggregationsMap[keyValues[0]];
                string selectedValues = String.Join("\",\"", values);
                ExpandoObject keyValueSelectedValues = new ExpandoObject();
                (keyValueSelectedValues as IDictionary<string, object>).Add(keyValue, values);
                mustClause.Add(new { terms = keyValueSelectedValues });
            }

			//Build filter for proprietary items
			if(!string.IsNullOrEmpty(catalogInfo.CustomerId))
				mustClause.Add(new { query_string = new { query = string.Format("isproprietary:false OR (isproprietary:true AND proprietarycustomers: {0})", catalogInfo.CustomerId) } });
			else
				mustClause.Add(new { match = new { isproprietary = false } }); //No CustomerId (Guest), filter out all proprietary items

            if (!string.IsNullOrEmpty(department)) {
                mustClause.Add(new { match = new { @department = department } });
            }

            if (!String.IsNullOrEmpty(category))
                mustClause.Add(BuildCategoryFilter(category));

            return mustClause;
        }

        private dynamic BuildFilterTermsWithMustNot( string facetFilters, UserSelectedContext catalogInfo, string category = "", string department = "" ) {
            List<dynamic> mustClause = new List<dynamic>();
            string facetSeparator = "___";
            string[] facets = facetFilters.Split( new string[] { facetSeparator }, StringSplitOptions.RemoveEmptyEntries );
            foreach (string s in facets) {
                string[] keyValues = s.Split( new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries );
                string[] values = s.Substring( s.IndexOf( ":" ) + 1 ).Split( new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries );
                string keyValue = ElasticSearchAggregationsMap[keyValues[0]];
                string selectedValues = String.Join( "\",\"", values );
                ExpandoObject keyValueSelectedValues = new ExpandoObject();
                (keyValueSelectedValues as IDictionary<string, object>).Add( keyValue, values );
                mustClause.Add( new { terms = keyValueSelectedValues } );
            }

            //Build filter for proprietary items
            if (!string.IsNullOrEmpty( catalogInfo.CustomerId ))
                mustClause.Add( new { query_string = new { query = string.Format( "isproprietary:false OR (isproprietary:true AND proprietarycustomers: {0})", catalogInfo.CustomerId ) } } );
            else
                mustClause.Add( new { match = new { isproprietary = false } } ); //No CustomerId (Guest), filter out all proprietary items

            if (!string.IsNullOrEmpty( department )) {
                mustClause.Add( new { match = new { @department = department } } );
            }

            if (!String.IsNullOrEmpty( category ))
                mustClause.Add( BuildCategoryFilter( category ) );

            List<dynamic> fieldFilterTerms = BuildStatusFilter();

            ExpandoObject filterTerms = new ExpandoObject();
            (filterTerms as IDictionary<string, object>).Add( "bool", new { must = mustClause, must_not = fieldFilterTerms } );

            return filterTerms;
        }

        private dynamic BuildFunctionScoreQuery(SearchInputModel searchModel, ExpandoObject filterTerms, List<string> fieldsToSearch, string searchExpression) {
            List<dynamic> shouldQueries = new List<dynamic>();
            shouldQueries.Add(
                new {
                    query_string = new {
                        fields = fieldsToSearch,
                        query = searchExpression,
                        use_dis_max = true
                    }
                }
            );

            shouldQueries.Add(
                new {
                    match = new {
                        name_ngram_analyzed = new {
                            query = searchExpression,
                            @operator = "and",
                            minimum_should_match = "75%"
                        }
                    }
                }
            );
            return new {
                from = searchModel.From,
                size = searchModel.Size,
                query = new {
                    function_score = new {
                        query = new {
                            filtered = new {
                                query = new {
                                    @bool = new {
                                        should = shouldQueries
                                    }
                                },
                                filter = new { query = filterTerms }
                            }
                        },
                        functions = BuildItemBoostFunctions(searchExpression),
                        score_mode = "sum",
                        boost_mode = "replace"
                    }
                },
                sort = BuildSort(searchModel.SField, searchModel.SDir),
                aggregations = ElasticSearchAggregations
            };
        }
        
        private static List<dynamic> BuildItemBoostFunctions(string searchTerms = null) {
            List<dynamic> boosts = new List<dynamic>();


			if(!string.IsNullOrEmpty(searchTerms))
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
				boosts.Add(new
				{
					filter = new { query = new { @bool = new { should = new List<dynamic>() { new { match_phrase_prefix = new { name_not_analyzed = searchTerms.ToLower() } } } } } },
					boost_factor = 1500
				});
				boosts.Add(new
				{
					filter = new { query = new { @bool = new { should = new List<dynamic>() { new { match_phrase = new { name = searchTerms } } } } } },
					boost_factor = 1400
				});
				boosts.Add(new
				{
					filter = new { query = new { @bool = new { should = new List<dynamic>() { new { match = new { name = searchTerms } } } } } },
					boost_factor = 1300
				});
				if (!searchTerms.Contains("OR upc:*")) //This is a search for a itemnumber or upc, don't add this boost
				{
					boosts.Add(new
					{
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
        
        private static dynamic BuildSort(string sortField, string sortDir) {
            if (!string.IsNullOrEmpty(sortField)) {
                ExpandoObject sortObject = new ExpandoObject();
                (sortObject as IDictionary<string, object>).Add(sortField, string.IsNullOrEmpty(sortDir) ? "asc" : sortDir);
                return sortObject;
            } else {
                return new {_score = "desc" };
            }
        }
        
        /// <summary>
        /// filter out items with unwanted statuses
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        private static List<dynamic> BuildStatusFilter() {
            string[] valuesToFilter = Configuration.ElasticSearchItemExcludeValues.Split(',');
            List<dynamic> fieldFilterTerms = new List<dynamic>();
            
            foreach (string s in valuesToFilter)
            {
                fieldFilterTerms.Add(new { match = new { status1_not_analyzed = s } });
            }

            return fieldFilterTerms;
        }

        public CategoriesReturn GetCategories(int from, int size, string catagoryType) {
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
            var response = _eshelper.Client.Search<Category>(s => s
                .From(from)
                .Size(GetCategoryPagingSize(size))
                .Type(Constants.ES_TYPE_CATEGORY)
                .Index(index)
                );

            var prefixesToExclude = Configuration.CategoryPrefixesToExclude.Split(',').ToList();

            foreach (string s in prefixesToExclude)
            {
                response.Documents.Where(b => !b.Id.Substring(0, 2).Equals(s));
            }
            
            // Have to do this because it won't infer from the ID up one level in the structure. Need to revisit.
            foreach (var r in response.Hits) {
                r.Source.Id = r.Id;
            }

            CategoriesReturn results = new CategoriesReturn { Categories = response.Documents.Where(s => s.SubCategories != null 
                && !(s.Id.Substring(0, 2).Equals("AA") || s.Id.Substring(0, 2).Equals("ZZ") || s.Id.Substring(0,2).Equals("TW"))).ToList<Category>() };

            return results;
        }

        private int GetCategoryPagingSize(int size) {
            if (size <= 0)
                return Configuration.DefaultCategoryReturnSize;
            return size;
        }
        
        private static ElasticsearchClient GetElasticsearchClient(string elasticSearchUrl) {
            var node = new Uri(elasticSearchUrl);
            var config = new Elasticsearch.Net.Connection.ConnectionConfiguration(node);
            var client = new ElasticsearchClient(config);
            return client;
        }
        
        public ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel) {
            int size = GetProductPagingSize(searchModel.Size);
            
            List<dynamic> filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo);

            dynamic categorySearchExpression = BuildBoolMultiMatchQuery(searchModel, filterTerms, new List<string>() { "brand_control_label" }, brandControlLabel);

            return GetProductsFromElasticSearch(catalogInfo.BranchId.ToLower(), true, "", categorySearchExpression);
        }

        public Product GetProductById(string branch, string id) {
            branch = branch.ToLower();

            ElasticsearchResponse<DynamicDictionary> res = _client.Get(branch, "product", id);

			if (res.Response == null)
				return null;

            return LoadProductFromElasticSearchProduct(false, res.Response);
        }
        
        private int GetProductPagingSize(int size) {
            if (size <= 0)
                return Configuration.DefaultProductReturnSize;
            return size;
        }
        private string _catalog = null;
        public ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel) {
            _catalog = catalogInfo.BranchId;
            int size = 0;
            if (searchModel.Size > 0)
            {
                size = GetProductPagingSize(searchModel.Size);
            }

            //List<string> childCategories = 
            //    GetCategories(0, Configuration.DefaultCategoryReturnSize).Categories.Where(c => c.Id.Equals(category, StringComparison.CurrentCultureIgnoreCase)).SelectMany(s => s.SubCategories.Select(i => i.Id)).ToList();

            ExpandoObject filterTerms = BuildFilterTermsWithMustNot(searchModel.Facets, catalogInfo, category: category);

            //string categorySearch = (childCategories.Count == 0 ? category : String.Join(" OR ", childCategories.ToArray()));

            dynamic categorySearchExpression = BuildBoolFunctionScoreQuery( searchModel.From, size, searchModel.SField, searchModel.SDir, 
                filterTerms);

            var query = Newtonsoft.Json.JsonConvert.SerializeObject(categorySearchExpression);

            ProductsReturn ret = GetProductsFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);

            if (searchModel.Size == 0)
            {
                size = ret.TotalCount;

                categorySearchExpression = BuildBoolFunctionScoreQuery(searchModel.From, size, searchModel.SField, searchModel.SDir,
                    filterTerms);

                query = Newtonsoft.Json.JsonConvert.SerializeObject(categorySearchExpression);

                ret = GetProductsFromElasticSearch(catalogInfo.BranchId, true, "", categorySearchExpression);
            }

            return ret;
        }

        public ProductsReturn GetProductsByIds(string branch, List<string> ids) {
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

        public ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel) {
            int size = GetProductPagingSize(searchModel.Size);
            List<dynamic> filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo, department: searchModel.Dept);
                        string termSearch = search;
            List<string> fieldsToSearch = null;
            dynamic termSearchExpression = null;

            System.Text.RegularExpressions.Regex matchOnlyDigits = new System.Text.RegularExpressions.Regex(@"^\d+$");

            if (matchOnlyDigits.IsMatch(search)) {
                fieldsToSearch = Configuration.ElasticSearchDigitSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQueryForDigits(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            } else {
                fieldsToSearch = Configuration.ElasticSearchTermSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQuery(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }

            termSearchExpression = BuildBoolMultiMatchQuery(searchModel, filterTerms, fieldsToSearch, termSearch);

			var query = Newtonsoft.Json.JsonConvert.SerializeObject(termSearchExpression);

            string branch = catalogInfo.BranchId.ToLower();
             
            return GetProductsFromElasticSearch(branch, true, "", termSearchExpression);
        }
        
        private ProductsReturn GetProductsFromElasticSearch(string branch, bool listonly, string searchBody, object searchBodyD = null) {
            string requestToJSON = "";
            if (searchBodyD == null) {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBody);
            } else {
                requestToJSON = Newtonsoft.Json.JsonConvert.SerializeObject(searchBodyD);
            }

            ElasticsearchResponse<DynamicDictionary> res = null;
            
            if (searchBodyD == null)
                res = _client.Search(branch.ToLower(), "product", searchBody);
            else
				res = _client.Search(branch.ToLower(), "product", searchBodyD);

            List<Product> products = new List<Product>();
            foreach (var oProd in res.Response["hits"]["hits"]) {
                Product p = LoadProductFromElasticSearchProduct(listonly, oProd);
                products.Add(p);
            }
            ExpandoObject facets = LoadFacetsFromElasticSearchResponse(res);
            int totalCount = Convert.ToInt32(res.Response["hits"]["total"].Value);

            return new ProductsReturn() { Products = products, Facets = facets, TotalCount = totalCount, Count = products.Count };
        }

        public int GetHitsForSearchInIndex(UserSelectedContext catalogInfo, string searchTerm, SearchInputModel searchModel)
        {
            _catalog = searchModel.CatalogType;
            int size = GetProductPagingSize(searchModel.Size);
            //ExpandoObject filterTerms = BuildFilterTerms(searchModel.Facets, catalogInfo, department: searchModel.Dept);

            //List<dynamic> newFilterTerms = new List<dynamic>();
            List<dynamic> newFilterTerms = BuildProprietaryItemFilter(catalogInfo, searchModel.Dept);
            if (searchModel.Facets != null && searchModel.Facets.Length > 0) {
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
            if (matchOnlyDigits.IsMatch(searchTerm)) {
                fieldsToSearch = Configuration.ElasticSearchDigitSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQueryForDigits(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            } else {
                fieldsToSearch = Configuration.ElasticSearchTermSearchFields;
                //termSearchExpression = BuildBoolMultiMatchQuery(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            }

            termSearchExpression = BuildBoolMultiMatchQuery(searchModel, newFilterTerms, fieldsToSearch, termSearch);
            //dynamic termSearchExpression = BuildFunctionScoreQuery(searchModel, filterTerms, fieldsToSearch, termSearch);
            var query = Newtonsoft.Json.JsonConvert.SerializeObject(termSearchExpression);

            string branch = catalogInfo.BranchId.ToLower();

            try {
                var res = _client.Search(branch.ToLower(), "product", termSearchExpression);
                if (res.Response["hits"]["total"] != null)
                    return Convert.ToInt32(res.Response["hits"]["total"].Value);
                else
                    return 0;
            } catch (Exception) {
                
                throw;
            }
        }

        //private delegate TResult Func<in T, out TResult>();

        private ExpandoObject LoadFacetsFromElasticSearchResponse(ElasticsearchResponse<DynamicDictionary> res) {
            ExpandoObject facets = new ExpandoObject();

            if (res.Response.Contains("aggregations")) {
                foreach (var oFacet in res.Response["aggregations"]) {
                    var facet = new List<ExpandoObject>();
                    foreach (var oFacetValue in oFacet.Value["buckets"]) {
                        var facetValue = new ExpandoObject() as IDictionary<string, object>;
                        facetValue.Add(new KeyValuePair<string, object>("name", oFacetValue["key"].ToString()));
                        facetValue.Add(new KeyValuePair<string, object>("count", oFacetValue["doc_count"]));
                        if (oFacet.Key == "categories") {
                            facetValue.Add(new KeyValuePair<string, object>("categoryname",  oFacetValue["category_meta"]["buckets"][0]["key"].ToString()));
                            if(oFacetValue["category_code"] != null && 
                               oFacetValue["category_code"]["buckets"] != null && 
                               (oFacetValue["category_code"]["buckets"]).Count > 0)
                            {
                                facetValue.Add(new KeyValuePair<string, object>("code", oFacetValue["category_code"]["buckets"][0]["key"].ToString()));
                            }
                        }
                        else if (oFacet.Key == "parentcategories")
                        {
                            if (oFacetValue["parentcategory_code"] != null &&
                                oFacetValue["parentcategory_code"]["buckets"] != null &&
                                (oFacetValue["parentcategory_code"]["buckets"]).Count > 0)
                            {
                                facetValue.Add(new KeyValuePair<string, object>("code", oFacetValue["parentcategory_code"]["buckets"][0]["key"].ToString()));
                            }
                        }
                        else if (oFacet.Key == "brands") {
                            if (oFacetValue["brand_meta"]["buckets"].Count > 0) {
                                facetValue.Add(new KeyValuePair<string, object>("brand_control_label", oFacetValue["brand_meta"]["buckets"][0]["key"].ToString()));
                            } else {
                                facetValue.Add(new KeyValuePair<string, object>("brand_control_label", null));
                            }
                        }
                        facet.Add(facetValue as ExpandoObject);
                    }
                    (facets as IDictionary<string, object>).Add(oFacet.Key, facet);
                }

                // corrolate parentcategories and categories
                var parentcategories = (facets as IDictionary<string, object>)["parentcategories"];
                var categories = (facets as IDictionary<string, object>)["categories"];
                if (parentcategories != null && categories != null)
                {
                    var newparentlist = new List<ExpandoObject>();
                    foreach (IDictionary<string, object> parent in (parentcategories as List<ExpandoObject>))
                    {
                        string pcode = parent["code"].ToString();
                        List<ExpandoObject> cats = new List<ExpandoObject>();
                        foreach (IDictionary<string, object> cat in (categories as List<ExpandoObject>))
                        {
                            string code = cat["code"].ToString();
                            if (_catalog.StartsWith("unfi", StringComparison.CurrentCultureIgnoreCase) && code.StartsWith(pcode.Substring(0, 1)))
                            {
                                cats.Add((ExpandoObject)cat);
                            }
                            else if (code.StartsWith(pcode.Substring(0, 2)))
                            {
                                cats.Add((ExpandoObject)cat);
                            }
                        }
                        parent.Add("categories", cats);
                    }
                }

            }

            return facets;
        }
        
        private Product LoadProductFromElasticSearchProduct(bool listonly, dynamic oProd) {
            Product p = new Product();
            p.ManufacturerName = oProd._source.mfrname;
            p.ItemNumber = oProd._id;
            p.Kosher = string.IsNullOrEmpty(oProd._source.kosher) ? "Unknown" : oProd._source.kosher;
            p.Brand = oProd._source.brand;
            p.BrandExtendedDescription = oProd._source.brand_description;
            p.Description = oProd._source.description;
            p.CategoryCode = oProd._source.parentcategoryid;
            p.UPC = oProd._source.upc;
            p.Name = oProd._source.name;
            p.CategoryName = oProd._source.categoryname;
            p.ItemClass = oProd._source.parentcategoryname;
            p.Pack = oProd._source.pack;
            p.Size = oProd._source.size;
            p.CaseOnly = oProd._source.caseonly == "Y";
            p.TempZone = oProd._source.temp_zone;
            p.CasePriceNumeric = oProd._source.caseprice != null ? oProd._source.caseprice : 0.00;
            p.CasePrice = p.CasePriceNumeric.ToString();
            p.PackagePriceNumeric = oProd._source.packageprice != null ? oProd._source.packageprice : 0.00;
            p.PackagePrice = p.PackagePriceNumeric.ToString();
            p.CatalogId = oProd._index;
            if (p.CatalogId.ToLower().StartsWith("unfi"))
            {
                //make vendor into description
                p.Description = oProd._source.vendor1;
                p.Pack = oProd._source.casequantity.ToString();
                p.Size = oProd._source.contsize.ToString() + oProd._source.contunit;
                p.IsSpecialtyCatalog = true;
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
            if (listonly == false)
            {
                p.ManufacturerNumber = oProd._source.mfrnumber;
                p.BrandControlLabel = oProd._source.brand_control_label;
                p.Cases = oProd._source.cases;
                p.ExtendedDescription = string.Empty;
                p.SubCategoryCode = oProd._source.categoryid;
                p.ReplacedItem = oProd._source.replaceditem;
                p.ReplacementItem = oProd._source.replacementitem;
                p.ChildNutrition = oProd._source.childnutrition;
                p.SellSheet = oProd._source.sellsheet;
                p.VendorItemNumber = oProd._source.vendor1;
                try
                {
                    p.CaseCube = oProd._source.icube;
                }
                catch (Exception e)
                {
                    p.CaseCube = "";
                }
                p.NonStock = oProd._source.nonstock;
                p.CatchWeight = oProd._source.catchweight;
                p.IsProprietary = oProd._source.isproprietary;
                p.AverageWeight = oProd._source.averageweight;
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

                    UNFI unfi = new UNFI();

                    if (oProd._source.cheight != null)
                        unfi.CaseHeight = oProd._source.cheight.ToString();
                    if (oProd._source.clength != null)
                        unfi.CaseLength = oProd._source.clength.ToString();
                    if (oProd._source.clength != null)
                        unfi.CaseWidth = oProd._source.cwidth.ToString();
                    if (oProd._source.averageweight != null)
                        unfi.Weight = oProd._source.averageweight.ToString();
                    if (oProd._source.unitofsale != null)
                        unfi.UnitOfSale = oProd._source.unitofsale.ToString();
                    if (oProd._source.shipminexpire != null)
                        unfi.CatalogDept = oProd._source.catalogdept.ToString();
                    if (oProd._source.clength != null)
                        unfi.ShipMinExpire = oProd._source.shipminexpire.ToString();
                    if (oProd._source.minorder != null)
                        unfi.MinOrder = oProd._source.minorder.ToString();
                    if (oProd._source.casequantity != null)
                        unfi.CaseQuantity = oProd._source.casequantity.ToString();
                    if (oProd._source.putup != null)
                        unfi.PutUp = oProd._source.putup.ToString();
                    if (oProd._source.contunit != null)
                        unfi.ContUnit = oProd._source.contunit.ToString();
                    if (oProd._source.tcscode != null)
                        unfi.TCSCode = oProd._source.tcscode.ToString();
                    if (oProd._source.caseupc != null)
                        unfi.CaseUPC = oProd._source.caseupc.ToString();
                    if (oProd._source.plength != null)
                        unfi.PackageLength = oProd._source.plength.ToString();
                    if (oProd._source.pheight != null)
                        unfi.PackageHeight = oProd._source.pheight.ToString();
                    if (oProd._source.pwidth != null)
                        unfi.PackageWidth = oProd._source.pwidth.ToString();
                    if (oProd._source.status != null)
                        unfi.Status = oProd._source.status.ToString();
                    if (oProd._source.packageprice != null)
                        unfi.PackagePrice = oProd._source.packageprice.ToString();
                    if (oProd._source.caseprice != null)
                        unfi.CasePrice = oProd._source.caseprice.ToString();
                    if (oProd._source.flag1 != null)
                        unfi.Flag1 = oProd._source.flag1.ToString();
                    if (oProd._source.flag2 != null)
                        unfi.Flag2 = oProd._source.flag2.ToString();
                    if (oProd._source.flag3 != null)
                        unfi.Flag3 = oProd._source.flag3.ToString();
                    if (oProd._source.flag4 != null)
                        unfi.Flag4 = oProd._source.flag4.ToString();
                    if (oProd._source.onhandqty != null)
                        unfi.OnHandQty = oProd._source.onhandqty.ToString();
                    if (oProd._source.vendor1 != null)
                        unfi.Vendor = oProd._source.vendor1.ToString();
                    if (oProd._source.stockedinbranches != null)
                        unfi.StockedInBranches = oProd._source.stockedinbranches.ToString();

                    p.Unfi = unfi;
                }

                if (oProd._source.nutritional != null)
                {
                    Nutritional nutritional = new Nutritional();
                    nutritional.BrandOwner = oProd._source.nutritional.brandowner;
                    nutritional.CountryOfOrigin = oProd._source.nutritional.countryoforigin;
                    nutritional.GrossWeight = oProd._source.nutritional.grossweight;
                    nutritional.HandlingInstructions = oProd._source.nutritional.handlinginstruction;
                    nutritional.Ingredients = oProd._source.nutritional.ingredients;
                    nutritional.MarketingMessage = oProd._source.nutritional.marketingmessage;
                    nutritional.MoreInformation = oProd._source.nutritional.moreinformation;
                    nutritional.ServingSize = oProd._source.nutritional.servingsize;
                    nutritional.ServingSizeUOM = oProd._source.nutritional.servingsizeuom;
                    nutritional.ServingsPerPack = oProd._source.nutritional.servingsperpack;
                    nutritional.ServingSugestion = oProd._source.nutritional.servingsuggestions;
                    nutritional.Shelf = oProd._source.nutritional.shelf;
                    nutritional.StorageTemp = oProd._source.nutritional.storagetemp;
                    nutritional.UnitMeasure = oProd._source.nutritional.unitmeasure;
                    nutritional.UnitsPerCase = oProd._source.nutritional.unitspercase;
                    nutritional.Volume = oProd._source.nutritional.volume;
                    nutritional.Height = oProd._source.nutritional.height;
                    nutritional.Length = oProd._source.nutritional.length;
                    nutritional.Width = oProd._source.nutritional.width;
                    nutritional.Allergens = new Allergen();
                    nutritional.NutritionInfo = new List<Nutrition>();
                    nutritional.DietInfo = new List<Diet>();
                    if (oProd._source.nutritional.allergen != null)
                    {
                        if (oProd._source.nutritional.allergen.freefrom != null)
                        {
                            nutritional.Allergens.freefrom = new List<string>();
                            foreach (var ff in oProd._source.nutritional.allergen.freefrom)
                                nutritional.Allergens.freefrom.Add(ff);
                        }

                        if (oProd._source.nutritional.allergen.contains != null)
                        {
                            nutritional.Allergens.contains = new List<string>();
                            foreach (var ff in oProd._source.nutritional.allergen.contains)
                                nutritional.Allergens.contains.Add(ff);
                        }

                        if (oProd._source.nutritional.allergen.maycontain != null)
                        {
                            nutritional.Allergens.maycontain = new List<string>();
                            foreach (var ff in oProd._source.nutritional.allergen.maycontain)
                                nutritional.Allergens.maycontain.Add(ff);
                        }

                        //foreach (var allergen in oProd._source.gs1.allergen)
                        //{
                        //	Allergen a = new Allergen() { AllergenType = allergen.allergentype, Level = allergen.level };
                        //	gs1.Allergens.Add(a);
                        //}
                    }

                    if (oProd._source.nutritional.nutrition != null)
                    {
                        foreach (var nutrition in oProd._source.nutritional.nutrition)
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
                    if (oProd._source.nutritional.diet != null)
                    {
                        foreach (var diet in oProd._source.nutritional.diet)
                        {
                            Diet d = new Diet() { DietType = diet.diettype, Value = diet.value };
                            nutritional.DietInfo.Add(d);
                        }
                    }
                    p.Nutritional = nutritional;
                }
            }

            return p;
        }
        #endregion

        #region properties
        public ExpandoObject ElasticSearchAggregations {
            get {
                if (!String.IsNullOrEmpty(Configuration.ElasticSearchAggregations)) {
                    ExpandoObject aggregationsFromConfig = new ExpandoObject();

                    foreach (string aggregation in Configuration.ElasticSearchAggregations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)) {
                        string[] aggregationParams = aggregation.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (aggregationParams.Length != 2)
                            throw new ApplicationException("Incorrect aggreation configuration");

                        if (aggregationParams[0] == "categories") {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0], 
                                new
                                {
                                    terms = new { field = aggregationParams[1], size = 500 },
                                    aggregations = new
                                    {
                                        category_meta = new { terms = new { field = "categoryname_not_analyzed", size = 500 } },
                                        category_code = new { terms = new { field = 
                                        ((_catalog.StartsWith("unfi", StringComparison.CurrentCultureIgnoreCase))? "tcscode" : "categoryid"), size = 500 } }
                                    }
                                });
                        }
                        else if (aggregationParams[0] == "parentcategories")
                        {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0],
                                new
                                {
                                    terms = new { field = aggregationParams[1], size = 500 },
                                    aggregations = new
                                    {
                                        parentcategory_code = new { terms = new { field =
                                        ((_catalog.StartsWith("unfi", StringComparison.CurrentCultureIgnoreCase)) ? "catalogdept" : "parentcategoryid"), size = 500 } }
                                    }
                                });
                        }
                        else if (aggregationParams[0] == "brands") {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0], new { terms = new { field = aggregationParams[1], size = 500 }, aggregations = new { brand_meta = new { terms = new { field = "brand_control_label", size = 500 } } } });
                        } else {
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
                foreach (string aggregation in Configuration.ElasticSearchAggregations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)) {
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
