﻿using KeithLink.Svc.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using System.Dynamic;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using Nest;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class ElasticSearchCatalogRepositoryImpl : ICatalogRepository
    {
        #region " attributes "

        private Helpers.ElasticSearch _eshelper;

        private ElasticsearchClient client = GetElasticsearchClient(Configuration.ElasticSearchURL);

        #endregion

        #region " constructor "

        public ElasticSearchCatalogRepositoryImpl()
        {
            _eshelper = new Helpers.ElasticSearch();
        }

        #endregion

        #region " methods / functions "

		public ProductsReturn GetProductsByCategory(string branch, string category, SearchInputModel searchModel)
        {
            int size = GetProductPagingSize(searchModel.Size);

            //List<string> childCategories = 
            //    GetCategories(0, Configuration.DefaultCategoryReturnSize).Categories.Where(c => c.Id.Equals(category, StringComparison.CurrentCultureIgnoreCase)).SelectMany(s => s.SubCategories.Select(i => i.Id)).ToList();

            ExpandoObject filterTerms = BuildFilterTerms(searchModel.Facets, category);

            //string categorySearch = (childCategories.Count == 0 ? category : String.Join(" OR ", childCategories.ToArray()));

            dynamic categorySearchExpression = BuildBoolFunctionScoreQuery(searchModel.From, searchModel.Size, searchModel.SField, searchModel.SDir, 
                filterTerms);

            return GetProductsFromElasticSearch(branch, "", categorySearchExpression);
        }

        public ProductsReturn GetHouseProductsByBranch(string branchId, string brandControlLabel, SearchInputModel searchModel)
        {
            int size = GetProductPagingSize(searchModel.Size);

            ExpandoObject filterTerms = BuildFilterTerms(searchModel.Facets);

            dynamic categorySearchExpression = BuildFunctionScoreQuery(searchModel.From, size, searchModel.SField, searchModel.SDir, filterTerms, new List<string>() { "brand_control_label" }, brandControlLabel);

            return GetProductsFromElasticSearch(branchId.ToLower(), "", categorySearchExpression);
        }

        private dynamic BuildFunctionScoreQuery(int from, int size, string sortField, string sortDir, ExpandoObject filterTerms, List<string> fieldsToSearch, string searchExpression)
        {
            return new {
                from = from,
                size = size,
                query = new {
                    function_score = new {
                        query = new {
                            filtered = new {
                                query = new {
                                    query_string = new {
                                        fields = fieldsToSearch,
                                        query = searchExpression,
                                        use_dis_max = true
                                    }
                                },
                                filter = new { query = filterTerms }
                            }
                        },
                        functions = BuildPreferredItemBoostFunctions(),
                        score_mode = "max",
                        boost_mode = "multiply"
                    }
                },
                sort = BuildSort(sortField, sortDir),
                aggregations = ElasticSearchAggregations
            };
        }
        private dynamic BuildBoolFunctionScoreQuery(int from, int size, string sortField, string sortDir, ExpandoObject query)
        {
            return new
            {
                from = from,
                size = size,
                query = new
                {
                    function_score = new
                    {
                        query,
                        functions = BuildPreferredItemBoostFunctions(),
                        score_mode = "max",
                        boost_mode = "multiply"
                    }
                },
                sort = BuildSort(sortField, sortDir),
                aggregations = ElasticSearchAggregations
            };
        }

        private static List<dynamic> BuildPreferredItemBoostFunctions()
        {
            return new List<dynamic>
                    {
                        new { filter = new { term = new { preferreditemcode = "A" } }, boost_factor = 300 },
                        new { filter = new { term = new { preferreditemcode = "B" } }, boost_factor = 200 },
                        new { filter = new { term = new { preferreditemcode = "C" } }, boost_factor = 100 }
                    };
        }

        private dynamic BuildFilterTerms(string facetFilters, string category="")
        {
            List<dynamic> facetTerms = new List<dynamic>();
            string[] facets = facetFilters.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] values = s.Substring(s.IndexOf(":") + 1).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                string keyValue = ElasticSearchAggregationsMap[keyValues[0]];
                string selectedValues = String.Join("\",\"", values);
                ExpandoObject keyValueSelectedValues = new ExpandoObject();
                (keyValueSelectedValues as IDictionary<string, object>).Add(keyValue, values);
                facetTerms.Add(new { terms = keyValueSelectedValues });
            }

            List<dynamic> fieldFilterTerms = BuildStatusFilter();
            List<dynamic> categoryFilterTerms = BuildCategoryFilter(category);

            ExpandoObject filterTerms = new ExpandoObject();
            (filterTerms as IDictionary<string, object>).Add("bool", new { must = facetTerms, must_not = fieldFilterTerms, should = categoryFilterTerms });

            return filterTerms;

        }

        private static List<dynamic> BuildStatusFilter() //filter out items with unwanted statuses
        {
            string[] valuesToFilter = Configuration.ElasticSearchItemExcludeValues.Split(',');
            List<dynamic> fieldFilterTerms = new List<dynamic>();
            
            foreach (string s in valuesToFilter)
            {
                fieldFilterTerms.Add(new { match = new { status1_not_analyzed = s } });
            }

            return fieldFilterTerms;
        }

        private static List<dynamic> BuildCategoryFilter(string category) //filter out items with unwanted statuses
        {
            List<dynamic> categoryFilterTerms = new List<dynamic>();

            if (String.IsNullOrEmpty(category))
                return categoryFilterTerms;

            categoryFilterTerms.Add(
                new { multi_match =
                    new { query = category, fields = 
                        new List<string>() { "categoryname_not_analyzed", "parentcategoryname_not_analyzed", "categoryid", "parentcategoryid" } } });

            return categoryFilterTerms;
        }

        private static dynamic BuildSort(string sortField, string sortDir)
        {
            if (!string.IsNullOrEmpty(sortField))
            {
                ExpandoObject sortObject = new ExpandoObject();
                (sortObject as IDictionary<string, object>).Add(sortField, string.IsNullOrEmpty(sortDir) ? "asc" : sortDir);
                return sortObject;
            }
            else
            {
                return new {_score = "desc" };
            }
        }

        private static ExpandoObject LoadFacetsFromElasticSearchResponse(ElasticsearchResponse<DynamicDictionary> res)
        {
            ExpandoObject facets = new ExpandoObject();

            if (res.Response.Contains("aggregations"))
            {
                foreach (var oFacet in res.Response["aggregations"])
                {
                    var facet = new List<ExpandoObject>();
                    foreach (var oFacetValue in oFacet.Value["buckets"])
                    {
                        var facetValue = new ExpandoObject() as IDictionary<string, object>;
                        facetValue.Add(new KeyValuePair<string, object>("name", oFacetValue["key"].ToString()));
                        facetValue.Add(new KeyValuePair<string, object>("count", oFacetValue["doc_count"]));
                        if (oFacet.Key == "categories")
                        {
                            facetValue.Add(new KeyValuePair<string, object>("categoryname", oFacetValue["category_meta"]["buckets"][0]["key"].ToString()));
                        }
                        else if (oFacet.Key == "brands")
                        {
                            if (oFacetValue["brand_meta"]["buckets"].Count > 0)
                            {
                                facetValue.Add(new KeyValuePair<string, object>("brand_control_label", oFacetValue["brand_meta"]["buckets"][0]["key"].ToString()));
                            }
                            else
                            {
                                facetValue.Add(new KeyValuePair<string, object>("brand_control_label", null));
                            }
                        }
                        facet.Add(facetValue as ExpandoObject);
                    }

                    (facets as IDictionary<string, object>).Add(oFacet.Key, facet);
                }
            }

            return facets;
        }

        public CategoriesReturn GetCategories(int from, int size)
        {
            var response = _eshelper.Client.Search<Category>(s => s
                .From(from)
                .Size(GetCategoryPagingSize(size))
                .Type(Constants.ES_TYPE_CATEGORY)
                .Index(Constants.ES_INDEX_CATEGORIES)
                );

            // Have to do this because it won't infer from the ID up one level in the structure. Need to revisit.
            foreach (var r in response.Hits)
            {
                r.Source.Id = r.Id;
            }

            CategoriesReturn results = new CategoriesReturn { Categories = response.Documents.Where(s=>s.SubCategories != null).ToList<Category>() };

            return results;
        }

		public ProductsReturn GetProductsBySearch(string branch, string search, SearchInputModel searchModel)
        {
            int size = GetProductPagingSize(searchModel.Size);
            branch = branch.ToLower();
            ExpandoObject filterTerms = BuildFilterTerms(searchModel.Facets);

            string termSearch = search;
            List<string> fieldsToSearch = Configuration.ElasticSearchTermSearchFields;
            System.Text.RegularExpressions.Regex matchOnlyDigits = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (matchOnlyDigits.IsMatch(search))
            { // results in a search string like '1234 OR upc:*1234 OR gtin:*1234 OR itemnumber:*1234'
                List<string> digitSearchTerms = (Configuration.ElasticSearchDigitSearchFields.Select(x => string.Concat(x + ":*" + search + "*"))).ToList();
                digitSearchTerms.Insert(0, search);
                termSearch = String.Join(" OR ", digitSearchTerms);
            }

            dynamic termSearchExpression = BuildFunctionScoreQuery(searchModel.From, size, searchModel.SField, searchModel.SDir, filterTerms, fieldsToSearch, termSearch);

            return GetProductsFromElasticSearch(branch, "", termSearchExpression);
        }

        private ProductsReturn GetProductsFromElasticSearch(string branch, string searchBody, object searchBodyD = null)
        {
            ElasticsearchResponse<DynamicDictionary> res = null;

            if (searchBodyD == null)
                res = client.Search(branch.ToLower(), "product", searchBody);
            else
				res = client.Search(branch.ToLower(), "product", searchBodyD);

            List<Product> products = new List<Product>();
            foreach (var oProd in res.Response["hits"]["hits"])
            {
                Product p = LoadProductFromElasticSearchProduct(oProd);
                products.Add(p);
            }
            ExpandoObject facets = LoadFacetsFromElasticSearchResponse(res);
            int totalCount = Convert.ToInt32(res.Response["hits"]["total"].Value);

            return new ProductsReturn() { Products = products, Facets = facets, TotalCount = totalCount, Count = products.Count };
        }

        public Product GetProductById(string branch, string id)
        {
            branch = branch.ToLower();

            ElasticsearchResponse<DynamicDictionary> res = client.Get(branch, "product", id);

			if (res.Response == null)
				return null;

            return LoadProductFromElasticSearchProduct(res.Response);
        }

        private static Product LoadProductFromElasticSearchProduct(dynamic oProd)
        {
            Product p = new Product();
            p.ManufacturerName = oProd._source.mfrname;
            p.ItemNumber = oProd._id;
            p.Kosher = string.IsNullOrEmpty(oProd._source.kosher) ? "Unknown" : oProd._source.kosher;
            p.ManufacturerNumber = oProd._source.mfrnumber;
            p.Size = oProd._source.size;
            p.Brand = oProd._source.brand;
            p.BrandExtendedDescription = oProd._source.brand_description;
            p.BrandControlLabel = oProd._source.brand_control_label;
            p.UPC = oProd._source.upc;
            p.Description = oProd._source.description;
            p.Cases = oProd._source.cases;
            p.ExtendedDescription = string.Empty;
            p.CategoryId = oProd._source.categoryid;
            p.ReplacedItem = oProd._source.replaceditem;
            p.ReplacementItem = oProd._source.replacementitem;
            p.CNDoc = oProd._source.cndoc;
            p.Name = oProd._source.name;
            p.CategoryName = oProd._source.categoryname;
            p.VendorItemNumber = oProd._source.vendor1;
            p.ItemClass = oProd._source.itemclass;
            p.CaseCube = oProd._source.icube;
			p.NonStock = oProd._source.nonstock;
			p.Pack = oProd._source.pack;
            p.TempZone = oProd._source.temp_zone;
			// TODO: pack, package, preferreditemcode, itemtype, status1, status2, icseonly, specialorderitem, vendor1, vendor2, itemclass, catmgr, buyer, branchid, replacementitem, replaceid, cndoc
            Nutritional nutritional = new Nutritional();
            if (oProd._source.nutritional != null)
            {
                nutritional.BrandOwner = oProd._source.nutritional.brandowner;
                nutritional.CountryOfOrigin = oProd._source.nutritional.countryoforigin;
                nutritional.GrossWeight = oProd._source.nutritional.grossweight;
                nutritional.HandlingInstructions = oProd._source.nutritional.handlinginstructions;
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
            }
            p.Nutritional = nutritional;
            return p;
        }

        private static ElasticsearchClient GetElasticsearchClient(string elasticSearchUrl)
        {
            var node = new Uri(elasticSearchUrl);
            var config = new Elasticsearch.Net.Connection.ConnectionConfiguration(node);
            var client = new ElasticsearchClient(config);
            return client;
        }

        private int GetCategoryPagingSize(int size)
        {
            if (size <= 0)
                return Configuration.DefaultCategoryReturnSize;
            return size;
        }

        private int GetProductPagingSize(int size)
        {
            if (size <= 0)
                return Configuration.DefaultProductReturnSize;
            return size;
        }

        #endregion

        #region " properties "

        public ExpandoObject ElasticSearchAggregations
        {
            get
            {
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
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0], new { terms = new { field = aggregationParams[1], size = 500 }, aggregations = new { category_meta = new { terms = new { field = "categoryname_not_analyzed", size = 500 } } } });
                        }
                        else if (aggregationParams[0] == "brands")
                        {
                            (aggregationsFromConfig as IDictionary<string, object>).Add(aggregationParams[0], new { terms = new { field = aggregationParams[1], size = 500 }, aggregations = new { brand_meta = new { terms = new { field = "brand_control_label", size = 500 } } } });
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

        public Dictionary<string, string> ElasticSearchAggregationsMap
        {
            get
            {
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

        public ProductsReturn GetProductsByIds(string branch, List<string> ids)
		{
			var productList = String.Join(" OR ", ids);
			var query = @"{
						""from"" : 0, ""size"" : 5000,
						""query"":{
						""query_string"" : {
						""fields"" : [""itemnumber""],
							""query"" : """ + productList + @""",
						""use_dis_max"" : true
							}
						}}";

			return GetProductsFromElasticSearch(branch, query);

        }

        #endregion

    } // end class
} // end namespace
