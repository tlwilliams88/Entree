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
using KeithLink.Svc.Core.Interface.InternalCatalog;
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
		private readonly string ItemSpec_NonStock = "NonStock";
		private readonly string ItemSpec_ReplacementItem = "ReplacementItem";
		private readonly string ItemSpec_Replaced = "ItemBeingReplaced";
		private readonly string ItemSpec_CNDoc = "CNDoc";
		
		private readonly string ProductMapping = @"{
			  ""product"" : {
				   ""properties"" : {
					   ""categoryname"" : {
							  ""type"" : ""string"",
							  ""index"" : ""not_analyzed""
						},
						""brand"" : {
							""type"" : ""string"",
							""index"" : ""not_analyzed""
						},
         				""name"" : {
           					""type"" : ""string"",
							""index"" : ""not_analyzed""
         				}
				   }
				}
			}";


		private readonly ICatalogInternalRepository catalogRepository;
        private readonly IStagingRepository stagingRepository;
        private readonly IElasticSearchRepository elasticSearchRepository;
		private readonly IEventLogRepository eventLog;
        
        public CatalogLogicImpl(ICatalogInternalRepository catalogRepository, IStagingRepository stagingRepository, IElasticSearchRepository elasticSearchRepository, IEventLogRepository eventLog)
        {
            this.catalogRepository = catalogRepository;
            this.stagingRepository = stagingRepository;
            this.elasticSearchRepository = elasticSearchRepository;
			this.eventLog = eventLog;
        }

        public void ProcessStagedData()
        {
            try
            {
				var catTask = Task.Factory.StartNew(() => ImportCatalog());
				var profileTask = Task.Factory.StartNew(() => ImportProfiles());
				var esItemTask = Task.Factory.StartNew(() => ImportItemsToElasticSearch());
				var esCatTask = Task.Factory.StartNew(() => ImportCategoriesToElasticSearch());

				Task.WaitAll(catTask, profileTask, esItemTask, esCatTask);


            }
            catch (Exception ex) 
            {
				//log
				eventLog.WriteErrorLog("Catalog Import Error", ex);
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
            
            catalogRepository.ImportXML(new CatalogImportOptions() { Mode = ImportMode.Full, TransactionMode = TransactionMode.NonTransactional, CatalogsToImport = catalogNames }, memoryStream);            
        }

        public void ImportProfiles()
        {   
        }

        public void ImportItemsToElasticSearch()
        {
			var branches = stagingRepository.ReadAllBranches();

			Parallel.ForEach(branches.AsEnumerable(), row =>
			{
				if (!elasticSearchRepository.CheckIfIndexExist(row.GetString("BranchId").ToLower()))
				{
					elasticSearchRepository.CreateEmptyIndex(row.GetString("BranchId").ToLower());
					elasticSearchRepository.MapProductProperties(row.GetString("BranchId").ToLower(), ProductMapping);
				}
			});

            var dataTable = stagingRepository.ReadFullItemForElasticSearch();
            var products = new BlockingCollection<ElasticSearchItemUpdate>();

            var gsData = stagingRepository.ReadGSDataForItems();

            var itemNutritions = BuildNutritionDictionary(gsData);
            var itemDiet = BuildDietDictionary(gsData);
            var itemAllergens = BuildAllergenDictionary(gsData);

            Parallel.ForEach(dataTable.AsEnumerable(), row =>
            {
                products.Add(PopulateElasticSearchItem(row, itemNutritions, itemDiet, itemAllergens));
            });

            int totalProcessed = 0;
			
            while (totalProcessed < products.Count)
            {
                var batch = products.Skip(totalProcessed).Take(Configuration.ElasticSearchBatchSize).ToList();

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
			var prefixesToExclude = Configuration.CategoryPrefixesToExclude.Split(',').ToList();

            foreach (DataRow row in itemTable.Rows)
            {
				if(prefixesToExclude.Contains(row.GetString("CategoryId").Substring(0,2)))
					continue;

                var newProd = new MSCommerceCatalogCollection2CatalogProduct() { ProductId = row.GetString("ItemId"), Definition = "Item" };
                newProd.DisplayName = new DisplayName[1] { new DisplayName() { language = "en-US", Value = row.GetString("Name") } };
                newProd.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = row.GetString("CategoryId"), rank = "0" } };
				newProd.listprice = "0";
                products.Add(newProd);
            }

            return products.ToArray();
        }

        private MSCommerceCatalogCollection2CatalogCategory[] GenerateCategories()
        {
            List<MSCommerceCatalogCollection2CatalogCategory> categories = new List<MSCommerceCatalogCollection2CatalogCategory>();

            var prefixesToExclude = Configuration.CategoryPrefixesToExclude.Split(',').ToList();
            

            
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

				if (prefixesToExclude.Contains(subCat.GetString("CategoryId").Substring(0, 2)))
					continue;

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

        private ElasticSearchItemUpdate PopulateElasticSearchItem(DataRow row, Dictionary<string, List<ItemNutrition>> nutrition, Dictionary<string, List<Diet>> diets, Dictionary<string, Allergen> allergens)
        {
            var item =  new ElasticSearchItemUpdate()
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
                        icube = row.GetString("Cube"),
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
                        cndoc = row.GetString("CNDoc"),
                        itemnumber = row.GetString("ItemId"),
						nonstock = row.GetString("NonStock"),
                        gs1 = new GS1Data()
                        {
                            brandowner = row.GetString("BrandOwner"),
                            countryoforigin = row.GetString("CountryOfOrigin"),
                            grossweight = row.GetString("GrossWeight"),
                            handlinginstruction = row.GetString("HandlingInstruction"),
                            height = row.GetString("Height"),
                            ingredients = row.GetString("Ingredients"),
                            length = row.GetString("Length"),
                            marketingmessage = row.GetString("MarketingMessage"),
                            moreinformation = row.GetString("MoreInformation"),
                            servingsize = row.GetString("ServingSize"),
                            servingsizeuom = row.GetString("ServingSizeUOM"),
                            servingsperpack = row.GetString("ServingsPerPack"),
                            servingsuggestion = row.GetString("ServingSuggestion"),
                            shelf = row.GetString("Shelf"),
                            storagetemp = row.GetString("StorageTemp"),
                            unitmeasure = row.GetString("UnitMeasure"),
                            unitspercase = row.GetString("UnitsPerCase"),
                            volume = row.GetString("Volume"),
                            width = row.GetString("Width"),
                            nutrition = nutrition.ContainsKey(row.GetString("UPC")) ? nutrition[row.GetString("UPC")] : null,
                            diet = diets.ContainsKey(row.GetString("UPC")) ? diets[row.GetString("UPC")] : null,
                            allergen = allergens.ContainsKey(row.GetString("UPC")) ? allergens[row.GetString("UPC")] : null,
                        }
                    }

                }
            };
			item.index.data.itemspecification = new List<string>();

			

			if (item.index.data.replacementitem != "000000")
				item.index.data.itemspecification.Add(ItemSpec_ReplacementItem);
			if (item.index.data.replaceditem != "000000")
				item.index.data.itemspecification.Add(ItemSpec_Replaced);
			if (item.index.data.cndoc.Equals("y", StringComparison.CurrentCultureIgnoreCase))
				item.index.data.itemspecification.Add(ItemSpec_CNDoc);
			//if(row.GetString("NonStock").Equals("y", StringComparison.CurrentCultureIgnoreCase))
			//	item.index.data.itemspecification.Add(ItemSpec_NonStock);
			

            return item;
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

        private Dictionary<string, List<ItemNutrition>> BuildNutritionDictionary(DataSet gsData)
        {
            var itemNutritions = new Dictionary<string, List<ItemNutrition>>();

            foreach (DataRow row in gsData.Tables[0].Rows)
            {
                if (itemNutritions.ContainsKey(row.GetString("gtin")))
                    itemNutritions[row.GetString("gtin")].Add(MapItemNutrition(row));
                else
                    itemNutritions.Add(row.GetString("gtin"), new List<ItemNutrition>() { MapItemNutrition(row) });
            }

            return itemNutritions;
        }

        private Dictionary<string, List<Diet>> BuildDietDictionary(DataSet gsData)
        {
            var itemDiets = new Dictionary<string, List<Diet>>();

            foreach (DataRow row in gsData.Tables[1].Rows)
            {
				if (row.GetString("Value").Equals("y", StringComparison.CurrentCultureIgnoreCase))
					if (itemDiets.ContainsKey(row.GetString("gtin")))
						itemDiets[row.GetString("gtin")].Add(MapDiet(row));
					else
						itemDiets.Add(row.GetString("gtin"), new List<Diet>() { MapDiet(row) });
            }

            return itemDiets;
        }

        private Dictionary<string, Allergen> BuildAllergenDictionary(DataSet gsData)
        {
            var itemAllergen = new Dictionary<string, Allergen>();

            foreach (DataRow row in gsData.Tables[2].Rows)
            {
				if (itemAllergen.ContainsKey(row.GetString("gtin")))
				{
					var existing = itemAllergen[row.GetString("gtin")];
					MapAllergen(existing, row);
				}
				else
				{
					var newItem = new Allergen();
					MapAllergen(newItem, row);
					itemAllergen.Add(row.GetString("gtin"), newItem);
				}
            }

            return itemAllergen;
        }

		private static void MapAllergen(Allergen allergen, DataRow row)
		{
			switch (row.GetString("LevelOfContainment"))
			{
				case "FREE_FROM":
					allergen.freefrom.Add(row.GetString("AllergenTypeDesc"));
					break;
				case "CONTAINS":
					allergen.contains.Add(row.GetString("AllergenTypeDesc"));
					break;
				case "MAY_CONTAIN":
					allergen.maycontain.Add(row.GetString("AllergenTypeDesc"));
					break;
			}
		}

		//private static Allergen MappAllergen(DataRow row)
		//{
		//	return new Allergen() { allergentype = row.GetString("AllergenTypeDesc"), level = row.GetString("LevelOfContainment") };
		//}


        private static Diet MapDiet(DataRow row)
        {
            return new Diet() { diettype = row.GetString("DietType") };
        }

        private ItemNutrition MapItemNutrition(DataRow subRow)
        {
            return new ItemNutrition()
            {
                dailyvalue = subRow.GetString("DailyValue"),
                measurementtypeid = subRow.GetString("MeasurmentTypeId"),
                measurementvalue = subRow.GetString("MeasurementValue"),
                nutrienttype = subRow.GetString("NutrientTypeDesc"),
                nutrienttypecode = subRow.GetString("NutrientTypeCode")
            };
        }

        #endregion






        
    }
}
