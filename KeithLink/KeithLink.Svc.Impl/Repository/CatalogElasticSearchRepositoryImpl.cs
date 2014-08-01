using KeithLink.Svc.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using System.Dynamic;
using KeithLink.Svc.Core.Catalog;

namespace KeithLink.Svc.Impl.Repository
{
    public class CatalogElasticSearchRepositoryImpl : ICatalogRepository
    {
        private ElasticsearchClient client = GetElasticsearchClient(Configuration.ElasticSearchURL);

        public CatalogElasticSearchRepositoryImpl()
        {
            
        }

        public ProductsReturn GetProductsByCategory(string branch, string category, int from, int size, string facetFilters)
        {
            size = GetProductPagingSize(size);

            category = category.ToLower();
            List<string> childCategories = new List<string>();

            CategoriesReturn ret = GetCategories(0, Configuration.DefaultCategoryReturnSize);
            foreach (var c in ret.Categories)
            {
                if (category == c.Id.ToLower())
                {
                    foreach (var subC in c.SubCategories)
                    {
                        childCategories.Add(subC.Id);
                    }
                }
            }

            string categorySearch = (childCategories.Count == 0 ? category : String.Join(" OR ", childCategories.ToArray()));

            var categoryFilter = @"{
                ""from"" : " + from + @", ""size"" : " + size + @",
                ""query"":{
                     ""query_string"" : {
                         ""fields"" : [""categoryid""],
                          ""query"" : """ + categorySearch + @""",
                                               ""use_dis_max"" : true
                        }
                      }" + ElasticSearchAggregations + @"
            }";

            return GetProductsFromElasticSearch(branch, categoryFilter);
        }

        private static void LoadFacetsFromElasticSearchResponse(ElasticsearchResponse<DynamicDictionary> res, ExpandoObject facets)
        {

            foreach (var oFacet in res.Response["aggregations"])
            {
                var facet = new ExpandoObject() as IDictionary<string, object>;
                foreach (var oFacetValue in oFacet.Value["buckets"])
                {
                    facet.Add(oFacetValue["key"].ToString(), oFacetValue["doc_count"]);
                }

                (facets as IDictionary<string, object>).Add(oFacet.Key, new List<ExpandoObject>() { facet as ExpandoObject });
            }
        }

        public CategoriesReturn GetCategories(int from, int size)
        {
            size = GetCategoryPagingSize(size);

            var categoryFilter = @"{
                ""from"" : " + from + @", ""size"" : " + size + @"
                }";

            ElasticsearchResponse<DynamicDictionary> res = client.Search("categories", "category", categoryFilter);
            List<Category> cats = new List<Category>();

            foreach (var oCat in res.Response["hits"]["hits"])
            {
                if (oCat._source.subcategories != null)
                {
                    Category cat = new Category() { Id = oCat._id, Name = oCat._source.name, Description = oCat._source.name };
                    List<Category> subCats = new List<Category>();
                    foreach (var oSubCat in oCat._source.subcategories)
                    {
                        Category subCat = new Category() { Id = oSubCat.categoryid, Name = oSubCat.name, Description = oSubCat.name };
                        subCats.Add(subCat);
                    }

                    cat.SubCategories = subCats.ToArray();
                    cats.Add(cat);
                }
            }

            return new CategoriesReturn() { Categories = cats };
        }

        public ProductsReturn GetProductsBySearch(string branch, string search, int from, int size, string facetFilters)
        {
            size = GetProductPagingSize(size);
            branch = branch.ToLower();
            string[] facets = facetFilters.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in facets)
            {
                string[] keyValues = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] values = s.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            }

            var searchBody = @"{
                ""from"" : " + from + @", ""size"" : " + size + @",
                ""query"":{
                     ""query_string"" : {
                          ""fields"" : [""name"", ""description"", ""categoryname""],
                          ""query"" : """ + search + @""",
                                               ""use_dis_max"" : true
                        }
                      }" + ElasticSearchAggregations + @"
            }";

            return GetProductsFromElasticSearch(branch, searchBody);
        }

        private ProductsReturn GetProductsFromElasticSearch(string branch, string searchBody)
        {
            ElasticsearchResponse<DynamicDictionary> res = client.Search(branch, "product", searchBody);

            List<Product> products = new List<Product>();
            ExpandoObject facets = new ExpandoObject();
            foreach (var oProd in res.Response["hits"]["hits"])
            {
                Product p = LoadProductFromElasticSearchProduct(oProd);
                products.Add(p);
            }
            LoadFacetsFromElasticSearchResponse(res, facets);
            int totalCount = Convert.ToInt32(res.Response["hits"]["total"].Value);

            return new ProductsReturn() { Products = products, Facets = (new List<ExpandoObject>() { facets }), TotalCount = totalCount, Count = products.Count };
        }

        public Product GetProductById(string branch, string id)
        {
            branch = branch.ToLower();

            ElasticsearchResponse<DynamicDictionary> res = client.Get(branch, "product", id);

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
            // TODO: pack, package, preferreditemcode, itemtype, status1, status2, icseonly, specialorderitem, vendor1, vendor2, itemclass, catmgr, buyer, branchid, replacementitem, replaceid, cndoc
            Gs1 gs1 = new Gs1();
            if (oProd._source.gs1 != null)
            {
                gs1.BrandOwner = oProd._source.gs1.brandowner;
                gs1.CountryOfOrigin = oProd._source.gs1.countryoforigin;
                gs1.GrossWeight = oProd._source.gs1.grossweight;
                gs1.HandlingInstructions = oProd._source.gs1.handlinginstructions;
                gs1.Ingredients = oProd._source.gs1.ingredients;
                gs1.MarketingMessage = oProd._source.gs1.marketingmessage;
                gs1.MoreInformation = oProd._source.gs1.moreinformation;
                gs1.ServingSize = oProd._source.gs1.servingsize;
                gs1.ServingSizeUOM = oProd._source.gs1.servingsizeuom;
                gs1.ServingsPerPack = oProd._source.gs1.servingsperpack;
                gs1.ServingSugestion = oProd._source.gs1.servingsuggestions;
                gs1.Shelf = oProd._source.gs1.shelf;
                gs1.StorageTemp = oProd._source.gs1.storagetemp;
                gs1.UnitMeasure = oProd._source.gs1.unitmeasure;
                gs1.UnitsPerCase = oProd._source.gs1.unitspercase;
                gs1.Volume = oProd._source.gs1.volume;
                gs1.Height = oProd._source.gs1.height;
                gs1.Length = oProd._source.gs1.length;
                gs1.Width = oProd._source.gs1.width;
                gs1.Allergens = new List<Allergen>();
                gs1.NutritionInfo = new List<Nutrition>();
                gs1.DietInfo = new List<Diet>();
                if (oProd._source.gs1.allergen != null)
                {
                    foreach (var allergen in oProd._source.gs1.allergen)
                    {
                        Allergen a = new Allergen() { AllergenType = allergen.allergentype, Level = allergen.level };
                        gs1.Allergens.Add(a);
                    }
                }
                if (oProd._source.gs1.nutrition != null)
                {
                    foreach (var nutrition in oProd._source.gs1.nutrition)
                    {
                        Nutrition n = new Nutrition()
                        {
                            DailyValue = nutrition.dailyvalue,
                            MeasurementTypeId = nutrition.measurementtypeid,
                            MeasurementValue = nutrition.measurementvalue,
                            NutrientType = nutrition.nutrienttype,
                            NutrientTypeCode = nutrition.nutrienttypecode
                        };
                        gs1.NutritionInfo.Add(n);
                    }
                }
                if (oProd._source.gs1.diet != null)
                {
                    foreach (var diet in oProd._source.gs1.diet)
                    {
                        Diet d = new Diet() { DietType = diet.diettype, Value = diet.value };
                        gs1.DietInfo.Add(d);
                    }
                }
            }
            p.Gs1 = gs1;
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
            if (size < 0)
                return Configuration.DefaultCategoryReturnSize;
            return size;
        }

        private int GetProductPagingSize(int size)
        {
            if (size < 0)
                return Configuration.DefaultProductReturnSize;
            return size;
        }

        private string _elasticSearchAggreagations = @",
                ""aggregations"" : {
                    ""categories"" : {
                    ""terms"" : {
                        ""field"" : ""categoryid""
                    }
                    }, ""brands"" : {
                    ""terms"" : {
                        ""field"" : ""brand""
                    }
                    }
                }";

        public string ElasticSearchAggregations {
            get
            {
                if (!String.IsNullOrEmpty(Configuration.ElasticSearchAggregations))
                {
                    List<string> aggregationsFromConfig = new List<string>();
                    StringBuilder s = new StringBuilder();
                    s.Append(",\"aggregations\" : {{\r\n {0} \r\n}}");
                    foreach (string aggregation in Configuration.ElasticSearchAggregations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] aggregationParams = aggregation.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (aggregationParams.Length != 2)
                            throw new ApplicationException("Incorrect aggreation configuration");

                        aggregationsFromConfig.Add("\r\n\"" + aggregationParams[0] + "\" : {\r\n    \"terms\" : { \"field\": \"" + aggregationParams[1] + "\" }}");
                    }

                    string formatString = s.ToString();
                    string aggregationsString = String.Join(",", aggregationsFromConfig.ToArray());
                    return string.Format(formatString, aggregationsString);
                }
                return _elasticSearchAggreagations;
            }
            set
            {
                _elasticSearchAggreagations = value;
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
    }
}
