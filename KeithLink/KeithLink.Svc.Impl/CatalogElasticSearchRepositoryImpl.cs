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
        public CatalogElasticSearchRepositoryImpl()
        {
            
        }

        public ProductsReturn GetProductsByCategory(string branch, string category, string elasticSearchEndpoint)
        {
            var client = GetElasticsearchClient(elasticSearchEndpoint);

            List<Product> products = new List<Product>();
            category = category.ToLower();

            var categoryFilter = @"{
                ""query"":{
                     ""query_string"" : {
                         ""fields"" : [""categoryid""],
                          ""query"" : """ + category + @""",
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

        public CategoriesReturn GetCategories(string elasticSearchEndpoint)
        {
            var client = GetElasticsearchClient(elasticSearchEndpoint);

            ElasticsearchResponse<DynamicDictionary> res = client.Search("categories", "category", "");
            List<Category> cats = new List<Category>();

            foreach (var oCat in res.Response["hits"]["hits"])
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

            return new CategoriesReturn() { Categories = cats };
        }

        private static ElasticsearchClient GetElasticsearchClient(string elasticSearchEndpoint)
        {
            var node = new Uri(elasticSearchEndpoint);
            var config = new Elasticsearch.Net.Connection.ConnectionConfiguration(node);
            var client = new ElasticsearchClient(config);
            return client;
        }

        public CategoriesReturn GetCategories()
        {
            throw new NotImplementedException();
        }


        public ProductsReturn GetProductsBySearch(string branch, string search, string elasticSearchEndpoint)
        {
            var client = GetElasticsearchClient(elasticSearchEndpoint);
            branch = branch.ToLower();

            var searchBody = @"{
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

        public Product GetProductById(string branch, string id, string elasticSearchEndpoint)
        {
            var client = GetElasticsearchClient(elasticSearchEndpoint);
            branch = branch.ToLower();

            ElasticsearchResponse<DynamicDictionary> res = client.Get(branch, "product", id);

            return LoadProductFromElasticSearchProduct(res.Response);
        }
    }
}
