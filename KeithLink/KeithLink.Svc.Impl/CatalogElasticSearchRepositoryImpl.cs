using KeithLink.Svc.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using System.Dynamic;

namespace KeithLink.Svc.Impl
{
    public class CatalogElasticSearchRepositoryImpl : ICatalogRepository
    {
        private ElasticsearchClient client = GetElasticsearchClient(Configuration.ElasticSearchURL);

        public CatalogElasticSearchRepositoryImpl()
        {
            
        }

        public ProductsReturn GetProductsByCategory(string branch, string category, int from, int size)
        {
            size = GetProductPagingSize(size);

            category = category.ToLower();
            List<string> childCategories = new List<string>();

            CategoriesReturn ret = GetCategories(from, size);
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

            List<Product> products = new List<Product>();
            string categorySearch = (childCategories.Count == 0 ? category : String.Join(" OR ", childCategories.ToArray()));

            var categoryFilter = @"{
                ""from"" : " + from + @", ""size"" : " + size + @",
                ""query"":{
                     ""query_string"" : {
                         ""fields"" : [""categoryid""],
                          ""query"" : """ + categorySearch + @""",
                                               ""use_dis_max"" : true
                        }
                      }
                    }";

            ElasticsearchResponse<DynamicDictionary> res = client.Search(branch, "product", categoryFilter);

            foreach (var oProd in res.Response["hits"]["hits"])
            {
                Product p = LoadProductFromElasticSearchProduct(oProd);
                products.Add(p);
            }

            return new ProductsReturn() { Products = products };
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

        public ProductsReturn GetProductsBySearch(string branch, string search, int from, int size)
        {
            size = GetProductPagingSize(size);
            branch = branch.ToLower();

            var searchBody = @"{
                ""from"" : " + from + @", ""size"" : " + size + @",
                ""query"":{
                     ""query_string"" : {
                          ""query"" : """ + search + @""",
                                               ""use_dis_max"" : true
                        }
                      }
                    }";

            ElasticsearchResponse<DynamicDictionary> res = client.Search(branch, "product", searchBody);

            List<Product> products = new List<Product>();

            foreach (var prod in res.Response["hits"]["hits"])
            {
                Product p = LoadProductFromElasticSearchProduct(prod);
                products.Add(p);
            }

            return new ProductsReturn() { Products = products };
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
    }
}
