using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceServer.Core.Catalog;
using KeithLink.Svc.Core.ETL;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using KeithLink.Svc.Impl.Models.ETL;
using KeithLink.Svc.Core;
using CommerceServer.Core.Profiles;
using System.Text.RegularExpressions;
using KeithLink.Svc.Impl.Models;
using System.Collections.Concurrent;
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.ETL
{
    public class CatalogLogicImpl: ICatalogLogic
    {
        private const string Language = "en-US";

        ILogger log = LogManager.GetLogger(typeof(CatalogLogicImpl));


        private readonly ICatalogInternalRepository catalogRepository;
        private readonly IStagingRepository stagingRepository;
        private readonly IElasticSearchRepository elasticSearchRepository;
        
        public CatalogLogicImpl(ICatalogInternalRepository catalogRepository, IStagingRepository stagingRepository, IElasticSearchRepository elasticSearchRepository)
        {
            this.catalogRepository = catalogRepository;
            this.stagingRepository = stagingRepository;
            this.elasticSearchRepository = elasticSearchRepository;
        }

        public void ProcessStagedData()
        {
            try
            {
                log.Debug("Start Processing Staged Catalog Data");
                
                var catTask = Task.Factory.StartNew(() => ImportCatalog());
                var profileTask = Task.Factory.StartNew(() => ImportProfiles());
                var esItemTask = Task.Factory.StartNew(() => ImportItemsToElasticSearch());
                var esCatTask = Task.Factory.StartNew(() => ImportCategoriesToElasticSearch());

                Task.WaitAll(catTask, profileTask, esItemTask, esCatTask);

                log.Debug("Staged Data Processed");

            }
            catch (Exception ex) 
            {
                log.Error(ex);
                throw ex;
            }
        }

        public void ImportCatalog()
        {
            //Create root level catalog object
            MSCommerceCatalogCollection2 catalog = new MSCommerceCatalogCollection2();
            catalog.version = "3.0"; //Required for the import to work

            //Create the BaseCatalog
            catalog.Catalog = BuildCatalogs(); 
            
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.Unicode);
            var serializer = new XmlSerializer(typeof(MSCommerceCatalogCollection2));
            
            serializer.Serialize(streamWriter, catalog);
            memoryStream.Position = 0;
            var catalogNames = string.Join(",", catalog.Catalog.Select(c => c.name).ToList().ToArray());
            
            catalogRepository.ImportXML(new CatalogImportOptions() { Mode = ImportMode.Incremental, TransactionMode = TransactionMode.NonTransactional, CatalogsToImport = catalogNames }, memoryStream);            
        }

        public void ImportProfiles()
        {   
        }

        public void ImportItemsToElasticSearch()
        {
            var dataTable = stagingRepository.ReadFullItemForElasticSearch();
            var products = new BlockingCollection<ElasticSearchItemUpdate>();
            Parallel.ForEach(dataTable.AsEnumerable(), row =>
            {
                products.Add(PopulateElasticSearchItem(row)
                );
            });

            int totalProcessed = 0;
			
            while (totalProcessed < products.Count)
            {
                var batch = products.Skip(totalProcessed).Take(Configuration.ElasticSearchBatchSize).ToList();

                //var sb = new StringBuilder();

                //foreach (var prod in batch)
                //    sb.Append(prod.ToJson());

                elasticSearchRepository.Create(string.Concat(batch.Select(i => i.ToJson())));

                totalProcessed += Configuration.ElasticSearchBatchSize;
            }

        }

        public void ImportCategoriesToElasticSearch()
        {
            var parentCategories = stagingRepository.ReadParentCategories();
            var childCategories = stagingRepository.ReadSubCategories();
            var categories = new BlockingCollection<ElasticSearchCategoryUpdate>();

            //Parent Categories
            Parallel.ForEach(parentCategories.AsEnumerable(), row =>
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
            });

            //Sub Categories
            Parallel.ForEach(childCategories.AsEnumerable(), row =>
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
            });

            elasticSearchRepository.Create(string.Concat(categories.Select(c => c.ToJson())));
        }
               
        
        #region Helper Methods

        private MSCommerceCatalogCollection2Catalog[] BuildCatalogs()
        {
            var catalogs = new List<MSCommerceCatalogCollection2Catalog>();
            var dataTable = stagingRepository.ReadAllBranches();

            foreach (DataRow row in dataTable.Rows)
            {
                var newCatalog = new MSCommerceCatalogCollection2Catalog() { name = row.GetString("BranchId"), productUID = "ProductId", startDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), endDate = DateTime.Now.AddYears(500).ToString("yyyy-MM-ddTHH:mm:ss"), languages = Language, DefaultLanguage = Language, ReportingLanguage = Language };
                newCatalog.DisplayName = CreateDisplayName(row.GetString("Description"));

                newCatalog.Category = GenerateCategories();
                newCatalog.Product = GenerateProducts(row.GetString("BranchId"));
                catalogs.Add(newCatalog);
            }

            return catalogs.ToArray();
        }
                
        private MSCommerceCatalogCollection2CatalogProduct[] GenerateProducts(string branchId)
        {
            var products = new List<MSCommerceCatalogCollection2CatalogProduct>();
            var itemTable = stagingRepository.ReadItems(branchId);

            foreach (DataRow row in itemTable.Rows)
            {
                var newProd = new MSCommerceCatalogCollection2CatalogProduct() { ProductId = row.GetString("ItemId"), Definition = "Item" };
                newProd.DisplayName = new DisplayName[1] { new DisplayName() { language = "en-US", Value = row.GetString("Name") } };
                newProd.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = row.GetString("CategoryId"), rank = "0" } };
                newProd.MfrName = row.GetString("MfrName");
                newProd.MfrNumber = row.GetString("MfrNumber");
                newProd.Pack = row.GetString("Pack");
                newProd.Size = row.GetString("Size");
                newProd.UPC = row.GetString("UPC");
                newProd.Description = Regex.Replace(row.GetString("Description"), @"[^0-9a-zA-Z /\~!@#$%^&*()_]+?", string.Empty);
                newProd.Brand = row.GetString("Brand");
                products.Add(newProd);
            }

            return products.ToArray();
        }

        private MSCommerceCatalogCollection2CatalogCategory[] GenerateCategories()
        {
            List<MSCommerceCatalogCollection2CatalogCategory> categories = new List<MSCommerceCatalogCollection2CatalogCategory>();
            var dataTable = stagingRepository.ReadParentCategories();
            var childTable = stagingRepository.ReadSubCategories();

            foreach (DataRow cat in dataTable.Rows)
            {
                var newSubCat = new MSCommerceCatalogCollection2CatalogCategory() { name = cat.GetString("CategoryId"), Definition = "Category" };
                newSubCat.DisplayName = CreateDisplayName(cat.GetString("CategoryName"));
                categories.Add(newSubCat);
            }

            foreach (DataRow subCat in childTable.Rows)
            {
                var newSubCat = new MSCommerceCatalogCollection2CatalogCategory() { name = subCat.GetString("CategoryId"), Definition = "Category" };
                newSubCat.DisplayName = CreateDisplayName(subCat.GetString("CategoryName"));
                newSubCat.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = string.Format("{0}000", subCat.GetString("CategoryId", true).Substring(0, 2)) } };
                categories.Add(newSubCat);
            }

            return categories.ToArray();
        }

        private static DisplayName[] CreateDisplayName(string value)
        {
            return new DisplayName[1] { new DisplayName() { language = Language, Value = value } };
        }

        private ElasticSearchItemUpdate PopulateElasticSearchItem(DataRow row)
        {
            return new ElasticSearchItemUpdate()
            {
                index = new RootData()
                {
                    _id = row.GetString("ItemId"),
                    _index = row.GetString("BranchId").ToLower(),
                    data = new AdditionalData()
                    {
                        brand = row.GetString("Brand"),
                        buyer = row.GetString("Buyer"),
                        cases = row.GetString("Cases"),
                        categoryid = row.GetString("CategoryId"),
                        categoryname = row.GetString("CategoryName"),
                        catmgr = row.GetString("CatMgr"),
                        description = Regex.Replace(row.GetString("Description"), @"[^0-9a-zA-Z /\~!@#$%^&*()_]+?", string.Empty),
                        icseonly = row.GetString("ICSEOnly"),
                        itemclass = row.GetString("Class"),
                        itemtype = row.GetString("ItemType"),
                        kosher = row.GetString("Kosher"),
                        mfrname = row.GetString("MfrName"),
                        mfrnumber = row.GetString("MfrNumber"),
                        name = row.GetString("Name"),
                        pack = row.GetString("Pack"),
                        package = row.GetString("Package"),
                        parentcategoryid = row.GetString("ParentCategoryId"),
                        parentcategoryname = row.GetString("ParentCategoryName"),
                        perferreditemcode = row.GetString("PreferredItemCode"),
                        size = row.GetString("Size"),
                        specialorderitem = row.GetString("SpecialOrderItem"),
                        status1 = row.GetString("Status1"),
                        status2 = row.GetString("Status2"),
                        upc = row.GetString("UPC"),
                        vendor1 = row.GetString("Vendor1"),
                        vendor2 = row.GetString("Vendor2"),
                        branchid = row.GetString("BranchId"),
                        replaceditem = row.GetString("ReplacedItem"),
                        replacementitem = row.GetString("ReplacementItem"),
                        cndoc = row.GetString("CNDoc")
                    }

                }
            };
        }

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
