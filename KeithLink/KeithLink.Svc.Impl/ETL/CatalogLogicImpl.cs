// Commerce Server
using CommerceServer.Core.Catalog;
using CommerceServer.Core.Profiles;

// KeithLink
using KeithLink.Common.Core;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Impl.Models;
using KeithLink.Svc.Impl.Models.ETL;
using KeithLink.Svc.Impl.Models.ElasticSearch.Item;
using KeithLink.Svc.Core.Models.Lists;

// Core
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KeithLink.Svc.Impl.ETL
{
    public class CatalogLogicImpl: KeithLink.Svc.Core.ETL.ICatalogLogic
    {
        #region " attributes "
        private const string Language = "en-US";
		private readonly string ItemSpec_NonStock = "NonStock";
		private readonly string ItemSpec_ReplacementItem = "ReplacementItem";
		private readonly string ItemSpec_Replaced = "ItemBeingReplaced";
		private readonly string ItemSpec_CNDoc = "CNDoc";
        private readonly string ItemSpec_CNDoc_FriendlyName = "childnutrition";
        private readonly string ItemSpec_SellSheet = "FDAProductFlag";
        private readonly string ItemSpec_SellSheet_FriendlyName = "sellsheet";
		
		private readonly string ProductMapping = @"{
			  ""product"" : {
				   ""properties"" : {
					   ""categoryname_not_analyzed"" : {
							  ""type"" : ""string"",
							  ""index"" : ""not_analyzed""
						},
					   ""parentcategoryname_not_analyzed"" : {
							  ""type"" : ""string"",
							  ""index"" : ""not_analyzed""
						},
						""brand_not_analyzed"" : {
							""type"" : ""string"",
							""index"" : ""not_analyzed""
						},
                        ""brand_description_not_analyzed"" : {
                            ""type"" : ""string"",
                            ""index"" : ""not_analyzed""
                        },
         				""name_not_analyzed"" : {
           					""type"" : ""string"",
							""index"" : ""not_analyzed""
         				},
						""mfrname_not_analyzed"" : {
							""type"" : ""string"",
							""index"" : ""not_analyzed""
						},
         				""preferreditemcode"" : {
           					""type"" : ""string"",
							""index"" : ""not_analyzed""
         				},
                        ""status1_not_analyzed"" : {
           					""type"" : ""string"",
							""index"" : ""not_analyzed""
         				},
                        ""nutritional"" : {
                            ""properties"" : {
                                ""diet"" : {
                                    ""properties"" : {
                                        ""diettype"" : {
                                            ""type"" : ""string"",
                                            ""index"" : ""not_analyzed""
                                        }
                                    }
                                }
                            }
                        }
				   }
				}
			}";


		private readonly ICatalogInternalRepository catalogRepository;
        private readonly IStagingRepository stagingRepository;
        private readonly IElasticSearchRepository elasticSearchRepository;
		private readonly IEventLogRepository eventLog;
        private readonly IUserProfileLogic userProfileLogic;

		private readonly IInternalListLogic listLogic;
        private readonly IInternalMessagingLogic messageLogic;
        
        #endregion

        #region " Methods / Functions "
        public CatalogLogicImpl(ICatalogInternalRepository catalogRepository,
            IStagingRepository stagingRepository, IElasticSearchRepository elasticSearchRepository,
			IEventLogRepository eventLog, IUserProfileLogic userProfile, IInternalListLogic listLogic, IInternalMessagingLogic messageLogic)
        {
            this.catalogRepository = catalogRepository;
            this.stagingRepository = stagingRepository;
            this.elasticSearchRepository = elasticSearchRepository;
			this.eventLog = eventLog;
            this.userProfileLogic = userProfile;
			this.listLogic = listLogic;
            this.messageLogic = messageLogic;
        }

        //run all catalog, es, and pre-populated list tasks in a non-distributed manner
        public void ProcessCatalogDataSerial()
        {
            try
            {
                // Note: Catalog processing stays here
                eventLog.WriteInformationLog("ETL Import Process Starting:  Import Catalog");
                ImportCatalog();

                // TODO: Elasticsearch processing needs to move to it's own logic class
                eventLog.WriteInformationLog("ETL Import Process Starting:  Import Items to Elastic Search");
                ImportItemsToElasticSearch();

                // TODO: Need to move category processing to an Elasticsearch logic class
                eventLog.WriteInformationLog("ETL Import Process Starting:  Import Categories to Elastic Search");
                ImportCategoriesToElasticSearch();

                // TODO: Need to move brands to an Elasticsearch logic class
                eventLog.WriteInformationLog("ETL Import Process Starting:  Import House Brands to ElasticSearch");
                ImportHouseBrandsToElasticSearch();

                // TODO: Need to evaluate where this should go to
                eventLog.WriteInformationLog("ETL Import Process Starting:  Import Pre-Populated Lists");
                ImportPrePopulatedLists();

                eventLog.WriteInformationLog("ETL Import Process Complete:  CatalogLogicImpl Tasks");
            }
            catch (Exception ex)
            {
                //log
                eventLog.WriteErrorLog("Error with ETL Import -- CatalogLogicImpl", ex);
            }
            
        }

        public void ProcessCatalogData()
        {
            try
            {
				var catTask = Task.Factory.StartNew(() => ImportCatalog());

				Task.WaitAll(catTask);

            }
            catch (Exception ex) 
            {
				//log
				eventLog.WriteErrorLog("Catalog Import Error", ex);
            }
        }

        public void ProcessElasticSearchData()
        {
            try
            {
                var esItemTask = Task.Factory.StartNew(() => ImportItemsToElasticSearch());
                var esCatTask = Task.Factory.StartNew(() => ImportCategoriesToElasticSearch());
                var esBrandTask = Task.Factory.StartNew(() => ImportHouseBrandsToElasticSearch());

                Task.WaitAll(esItemTask, esCatTask, esBrandTask);
            }
            catch (Exception ex)
            {
                //log
                eventLog.WriteErrorLog("Catalog Import Error", ex);
            }
        }

        public void ProcessContractAndWorksheetData()
        {
            try
            {
                var contractTask = Task.Factory.StartNew(() => ImportPrePopulatedLists());

                Task.WaitAll(contractTask);

            }
            catch (Exception ex)
            {
                //log
                eventLog.WriteErrorLog("Contract or Worksheet Import Error", ex);
            }
        }

        private void ImportCatalog()
        {
			//For performance debugging purposes
			var startTime = DateTime.Now;

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

			eventLog.WriteInformationLog(string.Format("ImportCatalog Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));

        }

        private void ImportProfiles()
        {   
        }

        private void ImportPrePopulatedLists()
        {

            //For performance debugging purposes
            var startTime = DateTime.Now;

			try
			{
				stagingRepository.ProcessContractItems();
				stagingRepository.ProcessWorksheetItems();
			}
			catch (Exception ex)
			{
				eventLog.WriteErrorLog("Error importing pre-populated lists", ex);
			}

			//var users = stagingRepository.ReadCSUsers();
			//var processedCustomers = new List<string>();

			//foreach (DataRow userRow in users.Rows)
			//{
			//	try
			//	{
			//		Guid userId = userRow.GetGuid("u_user_id");
			//		KeithLink.Svc.Core.Models.Profile.UserProfile userProfile = (KeithLink.Svc.Core.Models.Profile.UserProfile)userProfileLogic.GetUserProfile(userId, false).UserProfiles[0];

			//		if (userProfileLogic.IsInternalAddress(userProfile.EmailAddress))
			//			continue;

			//		List<KeithLink.Svc.Core.Models.Profile.Customer> customers = userProfileLogic.GetNonPagedCustomersForUser(userProfile);

			//		foreach (KeithLink.Svc.Core.Models.Profile.Customer customerRow in customers)
			//		{
			//			//These list are shared across all users in the same customer, 
			//			//so if the list has already been created for the customer, there is nothing to do
			//			if (processedCustomers.Contains(customerRow.CustomerNumber))
			//				break;
			//			processedCustomers.Add(customerRow.CustomerNumber);

			//			CreateOrUpdateContractLists(userProfile, customerRow);
			//			CreateOrUpdateWorksheetLists(userProfile, customerRow);

			//		}
			//	}
			//	catch (Exception ex)
			//	{
			//		eventLog.WriteErrorLog("Error importing pre-populated lists", ex);
			//	}

			//}

            eventLog.WriteInformationLog(string.Format("ImportPrePopulatedLists Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        private void ImportItemsToElasticSearch()
        {
			//For performance debugging purposes
			var startTime = DateTime.Now;

			var branches = stagingRepository.ReadAllBranches();

			//Parallel.ForEach(branches.AsEnumerable(), row =>
            foreach(DataRow row in branches.Rows)
			{
				if (!elasticSearchRepository.CheckIfIndexExist(row.GetString("BranchId").ToLower()))
				{
					elasticSearchRepository.CreateEmptyIndex(row.GetString("BranchId").ToLower());
					elasticSearchRepository.MapProductProperties(row.GetString("BranchId").ToLower(), ProductMapping);
				}
				else
					elasticSearchRepository.RefreshSynonyms(row.GetString("BranchId").ToLower());
			}

            var dataTable = stagingRepository.ReadFullItemForElasticSearch();
            var products = new BlockingCollection<ItemUpdate>();

            var gsData = stagingRepository.ReadGSDataForItems();

            var itemNutritions = BuildNutritionDictionary(gsData);
            var itemDiet = BuildDietDictionary(gsData);
            var itemAllergens = BuildAllergenDictionary(gsData);
			var proprietaryItems = BuildProprietaryItemDictionary(stagingRepository.ReadProprietaryItems());

            //Parallel.ForEach(dataTable.AsEnumerable(), row =>
            foreach(DataRow row in dataTable.Rows)
            {
                products.Add(PopulateElasticSearchItem(row, itemNutritions, itemDiet, itemAllergens, proprietaryItems));
            };

            int totalProcessed = 0;
			
            while (totalProcessed < products.Count)
            {
                var batch = products.Skip(totalProcessed).Take(Configuration.ElasticSearchBatchSize).ToList();

                elasticSearchRepository.Create(string.Concat(batch.Select(i => i.ToJson())));

                totalProcessed += Configuration.ElasticSearchBatchSize;
            }

			eventLog.WriteInformationLog(string.Format("ImportItemsToElasticSearch Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        private void ImportCategoriesToElasticSearch()
        {
			//For performance debugging purposes
			var startTime = DateTime.Now;

            var parentCategories = stagingRepository.ReadParentCategories();
            var childCategories = stagingRepository.ReadSubCategories();
            var categories = new BlockingCollection<ElasticSearchCategoryUpdate>();

            //Parent Categories
            //Parallel.ForEach(parentCategories.AsEnumerable(), row =>
            foreach(DataRow row in parentCategories.Rows)
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
            foreach(DataRow row in childCategories.Rows)
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

            elasticSearchRepository.Create(string.Concat(categories.Select(c => c.ToJson())));

			eventLog.WriteInformationLog(string.Format("ImportCategoriesToElasticSearch Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        private void ImportHouseBrandsToElasticSearch()
        {
			//For performance debugging purposes
			var startTime = DateTime.Now;

            var brandsDataTable = stagingRepository.ReadBrandControlLabels();
            var brands = new BlockingCollection<Models.ElasticSearch.BrandControlLabels.BrandUpdate>();

            //Parallel.ForEach(brandsDataTable.AsEnumerable(), row =>reach
            foreach(DataRow row in brandsDataTable.Rows)
                {
                    brands.Add(new Models.ElasticSearch.BrandControlLabels.BrandUpdate() 
                    {
                        index = new Models.ElasticSearch.BrandControlLabels.RootData()
                        {
                            _id = row.GetString("ControlLabel"),
                            data = new Models.ElasticSearch.BrandControlLabels.BrandData()
                            {
                                BrandControlLabel = row.GetString("ControlLabel"),
                                ExtendedDescription = row.GetString("ExtendedDescription")
                            }
                        }
                    });
                }

            elasticSearchRepository.Create(string.Concat(brands.Select(c => c.ToJson())));

			eventLog.WriteInformationLog(string.Format("ImportHouseBrandsToElasticSearch Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        #endregion

        #region Helper Methods

        #region Catalog and ES Helpers
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

        private ItemUpdate PopulateElasticSearchItem(DataRow row, Dictionary<string, List<ItemNutrition>> nutrition, 
                                                     Dictionary<string, List<Diet>> diets, Dictionary<string, Allergen> allergens, 
                                                     Dictionary<string, List<string>> proprietaryItems) {
            NutritionalInformation nutInfo = new NutritionalInformation();
            nutInfo.BrandOwner = row.GetString("BrandOwner");
            nutInfo.CountryOfOrigin = row.GetString("CountryOfOrigin");
            nutInfo.GrossWeight = row.GetString("GrossWeight");
            nutInfo.HandlingInstruction = row.GetString("HandlingInstruction");
            nutInfo.Height = row.GetString("Height");
            nutInfo.Ingredients = row.GetString("Ingredients");
            nutInfo.Length = row.GetString("Length");
            nutInfo.MarketingMessage = row.GetString("MarketingMessage");
            nutInfo.MoreInformation = row.GetString("MoreInformation");
            nutInfo.ServingSize = row.GetString("ServingSize");
            nutInfo.ServingSizeUom = row.GetString("ServingSizeUOM");
            nutInfo.ServingsPerPack = row.GetString("ServingsPerPack");
            nutInfo.ServingSuggestion = row.GetString("ServingSuggestion");
            nutInfo.Shelf = row.GetString("Shelf");
            nutInfo.StorageTemp = row.GetString("StorageTemp");
            nutInfo.UnitMeasure = row.GetString("UnitMeasure");
            nutInfo.UnitsPerCase = row.GetString("UnitsPerCase");
            nutInfo.Volume = row.GetString("Volume");
            nutInfo.Width = row.GetString("Width");
            nutInfo.Nutrition = nutrition.ContainsKey(row.GetString("UPC")) ? nutrition[row.GetString("UPC")] : null;
            nutInfo.Diet = diets.ContainsKey(row.GetString("UPC")) ? diets[row.GetString("UPC")] : null;
            nutInfo.Allergen = allergens.ContainsKey(row.GetString("UPC")) ? allergens[row.GetString("UPC")] : null;

            AdditionalData data = new AdditionalData();
            data.Brand = row.GetString("Brand");
            data.BrandNotAnalyzed = row.GetString("Brand");
            data.BrandDescription = row.GetString("BrandDescription");
            data.BrandDescriptionNotAnalyzed = row.GetString("BrandDescription");
            data.BrandControlLabel = row.GetString("MaxSmrt");
            data.Buyer = row.GetString("Buyer");
            data.Cases = row.GetString("Cases");
            data.CategoryId = row.GetString("CategoryId");
            data.CategoryName = row.GetString("CategoryName");
            data.CategoryNameNotAnalyzed = row.GetString("CategoryName");
            data.CatMgr = row.GetString("CatMgr");
            data.Description = Regex.Replace(row.GetString("Description"), @"[^0-9a-zA-Z /\~!@#$%^&*()_]+?", string.Empty);
            data.CaseOnly = row.GetString("ICSEOnly");
            data.ItemClass = row.GetString("Class");
            data.ItemType = row.GetString("ItemType");
            data.Kosher = row.GetString("Kosher");
            data.MfrName = row.GetString("MfrName");
            data.MfrNameNotAnalyzed = row.GetString("MfrName");
            data.MfrNumber = row.GetString("MfrNumber");
            data.Name = row.GetString("Name");
			data.NameNotAnalyzed = row.GetString("Name").ToLower();
            data.Pack = row.GetString("Pack");
            data.Package = row.GetString("Package");
            data.ParentCategoryId = row.GetString("ParentCategoryId");
            data.ParentCategoryName = row.GetString("ParentCategoryName");
            data.ParentCategoryNameNotAnalyzed = row.GetString("ParentCategoryName");
            data.PreferredItemCode = row.GetString("PreferredItemCode");
            data.Size = row.GetString("Size");
            data.SpecialOrderItem = row.GetString("SpecialOrderItem");
            data.Status1 = row.GetString("Status1");
            data.Status1NotAnalyzed = row.GetString("Status1");
            data.Status2 = row.GetString("Status2");
            data.Upc = row.GetString("UPC");
            data.Vendor1 = row.GetString("Vendor1");
            data.Vendor2 = row.GetString("Vendor2");
            data.BranchId = row.GetString("BranchId");
            data.ReplacedItem = row.GetString("ReplacedItem");
            data.ReplacementItem = row.GetString("ReplacementItem");
            data.ChildNutrition = row.GetString(ItemSpec_CNDoc);
            data.SellSheet = row.GetString(ItemSpec_SellSheet);
            data.ItemNumber = row.GetString("ItemId");
			data.NonStock = row.GetString("NonStock");
            data.TempZone = row.GetString("TempZone");
            data.CatchWeight = row.GetString("HowPrice") == "3";
			//data.IsProprietary = proprietaryItems.ContainsKey(row.GetString("ItemId"));
            data.IsProprietary = row.GetString("ItemType").Equals("P") ? true : false;
			data.ProprietaryCustomers = BuildPropritaryCustomerList(row.GetString("ItemId"), proprietaryItems);
            data.AverageWeight = (row.GetDouble("FPNetWt") > 0 ? row.GetDouble("FPNetWt") / 100 : (row.GetDouble("GrossWt") > 0 ? row.GetDouble("GrossWt") / 100 : 0));
            data.Nutritional = nutInfo;

            RootData index = new RootData();
            index._id = row.GetString("ItemId");
            index._index = row.GetString("BranchId").ToLower();
            index.data = data;

            ItemUpdate item = new ItemUpdate();
            item.index = index;

            item.index.data.ItemSpecification = new List<string>();

			if (item.index.data.ReplacementItem != "000000")
				item.index.data.ItemSpecification.Add(ItemSpec_ReplacementItem);
			if (item.index.data.ReplacedItem != "000000")
				item.index.data.ItemSpecification.Add(ItemSpec_Replaced);
			if (item.index.data.ChildNutrition.Equals("y", StringComparison.CurrentCultureIgnoreCase))
				item.index.data.ItemSpecification.Add(ItemSpec_CNDoc_FriendlyName);
			//if(row.GetString("NonStock").Equals("y", StringComparison.CurrentCultureIgnoreCase))
			//	item.index.data.itemspecification.Add(ItemSpec_NonStock);
            if (item.index.data.SellSheet.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                item.index.data.ItemSpecification.Add(ItemSpec_SellSheet_FriendlyName);

            return item;
        }
        
        private string BuildPropritaryCustomerList(string itemNumber, Dictionary<string, List<string>> proprietaryItems)
		{
			if (!proprietaryItems.ContainsKey(itemNumber))
				return null;

			var sbCustomerList = new StringBuilder();

			foreach (var customer in proprietaryItems[itemNumber])
				sbCustomerList.AppendFormat("{0} ", customer);

			return sbCustomerList.ToString();

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

		private Dictionary<string, List<string>> BuildProprietaryItemDictionary(DataTable propItemData)
		{
			var proprietaryItems = new Dictionary<string, List<string>>();

			foreach (DataRow row in propItemData.Rows)
			{
				if (proprietaryItems.ContainsKey(row.GetString("ItemNumber")))
				{
					proprietaryItems[row.GetString("ItemNumber")].Add(row.GetString("CustomerNumber"));
				}
				else
					proprietaryItems.Add(row.GetString("ItemNumber"), new List<string>() { row.GetString("CustomerNumber") });
			}

			return proprietaryItems;
		}

		private static void MapAllergen(Allergen allergen, DataRow row)
		{
			switch (row.GetString("LevelOfContainment"))
			{
				case "FREE_FROM":
					allergen.FreeFrom.Add(row.GetString("AllergenTypeDesc"));
					break;
				case "CONTAINS":
					allergen.Contains.Add(row.GetString("AllergenTypeDesc"));
					break;
				case "MAY_CONTAIN":
					allergen.MayContain.Add(row.GetString("AllergenTypeDesc"));
					break;
			}
		}

		//private static Allergen MappAllergen(DataRow row)
		//{
		//	return new Allergen() { allergentype = row.GetString("AllergenTypeDesc"), level = row.GetString("LevelOfContainment") };
		//}

        private static Diet MapDiet(DataRow row)
        {
            return new Diet() { DietType = row.GetString("DietType") };
        }

        private ItemNutrition MapItemNutrition(DataRow subRow)
        {
            return new ItemNutrition()
            {
                DailyValue = subRow.GetString("DailyValue"),
                MeasurementTypeId = subRow.GetString("MeasurmentTypeId"),
                MeasurementValue = subRow.GetString("MeasurementValue"),
                NutrientType = subRow.GetString("NutrientTypeDesc"),
                NutrientTypeCode = subRow.GetString("NutrientTypeCode")
            };
        }

        #endregion  

        #region Contract and Worksheet Helpers

		//private void CreateOrUpdateContractLists(
		//	KeithLink.Svc.Core.Models.Profile.UserProfile userProfile
		//	, KeithLink.Svc.Core.Models.Profile.Customer customerProfile
		//	)
		//{

		//	KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext = CreateUserSelectedContext(customerProfile.CustomerNumber, customerProfile.CustomerBranch);
		//	List<ListModel> lists = listLogic.ReadListByType(userSelectedContext, ListType.Contract);


		//	if (lists.Count == 0 && customerProfile.ContractId != null && !customerProfile.ContractId.Equals(String.Empty))
		//	{
		//		listLogic.CreateList(   
		//			userProfile.UserId 
		//			, userSelectedContext 
		//			, CreateUserList(
		//				customerProfile.ContractId
		//				, GetContractItems(userSelectedContext.CustomerId, userSelectedContext.BranchId, customerProfile.ContractId)
		//				, ListType.Contract
		//				, true)
		//			, ListType.Contract);
		//		return;
		//	}

		//	if (lists.Count == 1 && customerProfile.ContractId != null && !customerProfile.ContractId.Equals(String.Empty))
		//	{
		//		Dictionary<string, ListItemModel> newItemDictionary = CreateListItemDictionary(GetContractItems(userSelectedContext.CustomerId, userSelectedContext.BranchId, customerProfile.ContractId));
		//		Dictionary<string, ListItemModel> existingItemDictionary = CreateListItemDictionary(lists[0].Items);
		//		SortedSet<string> existingItemNumbers = new SortedSet<string>(existingItemDictionary.Keys);
		//		SortedSet<string> newItemNumbers = new SortedSet<string>(newItemDictionary.Keys);

		//		if (!Crypto.CalculateMD5Hash(existingItemNumbers).Equals(Crypto.CalculateMD5Hash(newItemNumbers)))
		//		{
		//			List<ListItemModel> newItems = CompareNewToExistingListItems(lists[0], newItemDictionary, existingItemDictionary);
		//			List<ListItemModel> deletedItems = CompareExistingToNewListItems(newItemDictionary, existingItemDictionary);
                    
		//			if (newItems.Count > 0)
		//			{
		//				long addedItemsListId = GetContractItemChangesListId(userProfile, userSelectedContext, ListType.ContractItemsAdded);
		//				listLogic.AddItems(userProfile, userSelectedContext, addedItemsListId, newItems);
		//			}
                    
		//			if (deletedItems.Count > 0)
		//			{
		//				long deletedItemsListId = GetContractItemChangesListId(userProfile, userSelectedContext, ListType.ContractItemsDeleted);
		//				listLogic.AddItems(userProfile, userSelectedContext, deletedItemsListId, deletedItems);
		//			}

		//			lists[0].Items = GetContractItems(userSelectedContext.CustomerId, userSelectedContext.BranchId, customerProfile.ContractId);
		//			listLogic.UpdateList(lists[0]);

		//			SendContractDiffMessages(customerProfile);

		//		}
		//	}
		//	else if (lists.Count == 1 && (customerProfile.ContractId == null || customerProfile.ContractId.Equals(String.Empty)))
		//	{
		//		listLogic.DeleteList(lists[0].ListId);
		//	}
		//}

		//private void CreateOrUpdateWorksheetLists(
		//	KeithLink.Svc.Core.Models.Profile.UserProfile userProfile
		//	, KeithLink.Svc.Core.Models.Profile.Customer customerProfile)
		//{
		//	KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext = CreateUserSelectedContext(customerProfile.CustomerNumber, customerProfile.CustomerBranch);
		//	List<ListModel> lists = listLogic.ReadListByType(userSelectedContext, ListType.Worksheet);

		//	if (lists.Count == 0)
		//	{
		//		listLogic.CreateList(
		//			userProfile.UserId
		//			, userSelectedContext
		//			, CreateUserList(
		//				String.Empty
		//				, GetWorksheetItems(userSelectedContext.CustomerId, userSelectedContext.BranchId)
		//				, ListType.Worksheet
		//				, true)
		//			, ListType.Worksheet);
		//	}

		//	if (lists.Count == 1)
		//	{
		//		Dictionary<string, ListItemModel> newItemDictionary = CreateListItemDictionary(GetWorksheetItems(userSelectedContext.CustomerId, userSelectedContext.BranchId));
		//		Dictionary<string, ListItemModel> existingItemDictionary = CreateListItemDictionary(lists[0].Items);
		//		SortedSet<string> existingItemNumbers = new SortedSet<string>(existingItemDictionary.Keys);
		//		SortedSet<string> newItemNumbers = new SortedSet<string>(newItemDictionary.Keys);

		//		if (!Crypto.CalculateMD5Hash(existingItemNumbers).Equals(Crypto.CalculateMD5Hash(newItemNumbers)))
		//		{
		//			lists[0].Items = GetWorksheetItems(userSelectedContext.CustomerId, userSelectedContext.BranchId);
		//			listLogic.UpdateList(lists[0]);
		//		}
		//	}
		//}

		//private KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext CreateUserSelectedContext(string customerNumber, string branchId)
		//{
		//	KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext = new KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext();
		//	userSelectedContext.BranchId = branchId;
		//	userSelectedContext.CustomerId = customerNumber;
		//	return userSelectedContext;
		//}

		//private ListModel CreateUserList(string name, List<ListItemModel> items, ListType listType, bool readOnly)
		//{
		//	ListModel list = new ListModel();
		//	list.Name = GetListName(name, listType);
		//	list.Items = items;
		//	list.ReadOnly = readOnly;
		//	return list;
		//}

		//private string GetListName(string name, ListType listType)
		//{
		//	string listName = name;

		//	switch (listType)
		//	{
		//		case ListType.Contract:
		//			listName = "Contract - " + listName;
		//			break;
		//		case ListType.Worksheet:
		//			listName = "History";
		//			break;
		//		case ListType.ContractItemsAdded:
		//			listName = "Contract Items Added";
		//			break;
		//		case ListType.ContractItemsDeleted:
		//			listName = "Contract Items Deleted";
		//			break;
		//	}

		//	return listName;
		//}

		//private List<ListItemModel> GetContractItems(
		//	 string customerNumber,
		//	 string branchId,
		//	 string contractId
		//	 )
		//{
		//	List<ListItemModel> contractItems = stagingRepository
		//					.ReadContractItems(customerNumber, branchId, contractId)
		//					.AsEnumerable()
		//					.Select(itemRow =>
		//						new ListItemModel
		//						{
		//							ItemNumber = itemRow.GetString("ItemNumber"),
		//							Position = itemRow.GetInt("BidLineNumber"),
		//							Category = itemRow.GetString("CategoryDescription"),
		//							Each = itemRow.GetString("BrokenCaseCode").Equals("Y") ? true : false
		//						}).ToList();
		//	return contractItems;
		//}

		//private List<ListItemModel> GetWorksheetItems(
		//	 string customerNumber,
		//	 string branchId
		//	 )
		//{
		//	List<ListItemModel> contractItems = stagingRepository
		//					.ReadWorksheetItems(customerNumber, branchId)
		//					.AsEnumerable()
		//					.Select(itemRow =>
		//						new ListItemModel
		//						{
		//							ItemNumber = itemRow.GetString("ItemNumber"),
		//							Each = itemRow.GetString("BrokenCaseCode").Equals("Y") ? true : false
		//						}).ToList();
		//	return contractItems;
		//}
		        
		//private Dictionary<string, ListItemModel> CreateListItemDictionary(List<ListItemModel> listItems)
		//{
		//	Dictionary<string, ListItemModel> itemDictionary = new Dictionary<string, ListItemModel>();
            
		//	foreach (ListItemModel lim in listItems)
		//	{
		//		if (!itemDictionary.ContainsKey(lim.ItemNumber + lim.Each.ToString()))
		//		{
		//			itemDictionary.Add(lim.ItemNumber + lim.Each.ToString(), lim);
		//		}
		//	}

		//	return itemDictionary;
		//}

		//private List<ListItemModel> CompareNewToExistingListItems(ListModel list, Dictionary<string, ListItemModel> newItemDictionary, Dictionary<string, ListItemModel> existingItemDictionary)
		//{
		//	List<ListItemModel> newItems = new List<ListItemModel>();
		//	foreach (KeyValuePair<string, ListItemModel> kvp in newItemDictionary)
		//	{
		//		if (!existingItemDictionary.ContainsKey(kvp.Key))
		//		{
		//			newItems.Add(kvp.Value);
		//		}
		//	}
		//	return newItems;
		//}

		//private List<ListItemModel> CompareExistingToNewListItems(Dictionary<string, ListItemModel> newItemDictionary, Dictionary<string, ListItemModel> existingItemDictionary)
		//{
		//	List<ListItemModel> existingItems = new List<ListItemModel>();
		//	foreach (KeyValuePair<string, ListItemModel> kvp in existingItemDictionary)
		//	{
		//		if (!newItemDictionary.ContainsKey(kvp.Key))
		//		{
		//			existingItems.Add(kvp.Value);
		//		}
		//	}

		//	return existingItems;
		//}

		//private long GetContractItemChangesListId(
		//	 KeithLink.Svc.Core.Models.Profile.UserProfile userProfile
		//	, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext
		//	, ListType listType)
		//{
		//	long listId = -1;
		//	List<ListModel> addedItemsList = listLogic.ReadListByType(userSelectedContext, listType);

		//	if (addedItemsList.Count == 0)
		//	{
		//		listId = listLogic.CreateList(
		//										userProfile.UserId
		//										, userSelectedContext
		//										, CreateUserList(
		//											""
		//											, null
		//											, listType
		//											, true)
		//										, listType);
		//	}
		//	else
		//	{
		//		listId = addedItemsList[0].ListId;
		//	}

		//	return listId;
		//}

		//private void SendContractDiffMessages(KeithLink.Svc.Core.Models.Profile.Customer customerProfile)
		//{
		//	KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext = CreateUserSelectedContext(customerProfile.CustomerNumber, customerProfile.CustomerBranch);
		//	KeithLink.Svc.Core.Models.Profile.UserFilterModel filter = new KeithLink.Svc.Core.Models.Profile.UserFilterModel();
		//	filter.CustomerId = customerProfile.CustomerId;
		//	KeithLink.Svc.Core.Models.Profile.UserProfileReturn userQuery = userProfileLogic.GetUsers(filter);

		//	List<KeithLink.Svc.Core.Models.Profile.UserProfile> users = userQuery.UserProfiles;

		//	foreach (KeithLink.Svc.Core.Models.Profile.UserProfile up in users)
		//	{
		//		CreateContractDiffMessage(up.UserId, userSelectedContext);
		//	}

		//}

		//private void CreateContractDiffMessage(Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext)
		//{
		//	KeithLink.Svc.Core.Models.Messaging.UserMessageModel messageModel = new KeithLink.Svc.Core.Models.Messaging.UserMessageModel();
		//	messageModel.Body = "Your contract items have changed.  Please check your contract added and removed items lists for details.";
		//	messageModel.Subject = "Your contract items have changed";
		//	messageModel.NotificationType = Core.Enumerations.Messaging.NotificationType.HasNews;
		//	messageLogic.CreateUserMessage(userId, userSelectedContext, messageModel);
		//}

        #endregion
        #endregion

    }
}
