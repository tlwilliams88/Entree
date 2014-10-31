using System;
using System.Collections;
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
using KeithLink.Svc.Impl.Models.ElasticSearch.Item;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Interface.Profile;

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
         				""preferreditemcode"" : {
           					""type"" : ""string"",
							""index"" : ""not_analyzed""
         				},
                        ""status1_not_analyzed"" : {
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
        private readonly IUserProfileLogic userProfileLogic;
		private readonly IInternalListLogic listLogic;
        
        #endregion

        #region " Methods / Functions "
        public CatalogLogicImpl(ICatalogInternalRepository catalogRepository,
            IStagingRepository stagingRepository, IElasticSearchRepository elasticSearchRepository,
			IEventLogRepository eventLog, IUserProfileLogic userProfile, IInternalListLogic listLogic)
        {
            this.catalogRepository = catalogRepository;
            this.stagingRepository = stagingRepository;
            this.elasticSearchRepository = elasticSearchRepository;
			this.eventLog = eventLog;
            this.userProfileLogic = userProfile;
			this.listLogic = listLogic;
        }

        public void ProcessStagedData()
        {
            try
            {
				var catTask = Task.Factory.StartNew(() => ImportCatalog());

				Task.WaitAll(catTask);

				var esItemTask = Task.Factory.StartNew(() => ImportItemsToElasticSearch());
				var esCatTask = Task.Factory.StartNew(() => ImportCategoriesToElasticSearch());
                var esBrandTask = Task.Factory.StartNew(() => ImportHouseBrandsToElasticSearch());

				Task.WaitAll(esItemTask, esCatTask, esBrandTask);

                var contractTask = Task.Factory.StartNew(() => ImportPrePopulatedLists());
                
                Task.WaitAll(contractTask);
				
            }
            catch (Exception ex) 
            {
				//log
				eventLog.WriteErrorLog("Catalog Import Error", ex);
            }
        }

        public void ImportCustomers()
        {

        }

        public void ImportCatalog()
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

        public void ImportProfiles()
        {   
        }

        public void ImportPrePopulatedLists()
        {
            //For performance debugging purposes
            var startTime = DateTime.Now;

            var users = stagingRepository.ReadCSUsers();
            var processedCustomers = new List<string>();

            foreach (DataRow userRow in users.Rows)
            {
                try
                {
                    Guid userId = userRow.GetGuid("u_user_id");
                    KeithLink.Svc.Core.Models.Profile.UserProfile userProfile = (KeithLink.Svc.Core.Models.Profile.UserProfile)userProfileLogic.GetUserProfile(userId).UserProfiles[0];

                    if (userProfileLogic.IsInternalAddress(userProfile.EmailAddress))
                        continue;

                    List<KeithLink.Svc.Core.Models.Profile.Customer> customers = userProfile.UserCustomers;

                    foreach (KeithLink.Svc.Core.Models.Profile.Customer customerRow in customers)
                    {
                        //These list are shared across all users in the same customer, 
                        //so if the list has already been created for the customer, there is nothing to do
                        if (processedCustomers.Contains(customerRow.CustomerNumber))
                            break;
                        processedCustomers.Add(customerRow.CustomerNumber);

                        KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext = CreateUserSelectedContext(customerRow.CustomerNumber, customerRow.CustomerBranch);

                        CreateOrUpdateContractLists(userProfile, userSelectedContext, customerRow.ContractId);
                        CreateOrUpdateWorksheetLists(userProfile, userSelectedContext);

                    }
                }
                catch (Exception ex)
                {
                    eventLog.WriteErrorLog("Catalog Import Error", ex);
                }
                
            }

            eventLog.WriteInformationLog(string.Format("ImportContractLists Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }
        
        public void ImportItemsToElasticSearch()
        {
			//For performance debugging purposes
			var startTime = DateTime.Now;

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
            var products = new BlockingCollection<ItemUpdate>();

            var gsData = stagingRepository.ReadGSDataForItems();

            var itemNutritions = BuildNutritionDictionary(gsData);
            var itemDiet = BuildDietDictionary(gsData);
            var itemAllergens = BuildAllergenDictionary(gsData);
			var proprietaryItems = BuildProprietaryItemDictionary(stagingRepository.ReadProprietaryItems());

            Parallel.ForEach(dataTable.AsEnumerable(), row =>
            {
                products.Add(PopulateElasticSearchItem(row, itemNutritions, itemDiet, itemAllergens, proprietaryItems));
            });

            int totalProcessed = 0;
			
            while (totalProcessed < products.Count)
            {
                var batch = products.Skip(totalProcessed).Take(Configuration.ElasticSearchBatchSize).ToList();

                elasticSearchRepository.Create(string.Concat(batch.Select(i => i.ToJson())));

                totalProcessed += Configuration.ElasticSearchBatchSize;
            }

			eventLog.WriteInformationLog(string.Format("ImportItemsToElasticSearch Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        public void ImportCategoriesToElasticSearch()
        {
			//For performance debugging purposes
			var startTime = DateTime.Now;

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

			eventLog.WriteInformationLog(string.Format("ImportCategoriesToElasticSearch Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        public void ImportHouseBrandsToElasticSearch()
        {
			//For performance debugging purposes
			var startTime = DateTime.Now;

            var brandsDataTable = stagingRepository.ReadBrandControlLabels();
            var brands = new BlockingCollection<Models.ElasticSearch.BrandControlLabels.BrandUpdate>();

            Parallel.ForEach(brandsDataTable.AsEnumerable(), row =>
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
                });

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

        private ItemUpdate PopulateElasticSearchItem(DataRow row, Dictionary<string, List<ItemNutrition>> nutrition, Dictionary<string, List<Diet>> diets, Dictionary<string, Allergen> allergens, Dictionary<string, List<string>> proprietaryItems)
        {
            var item =  new ItemUpdate()
            {
                index = new RootData()
                {
                    _id = row.GetString("ItemId"),
                    _index = row.GetString("BranchId").ToLower(),
                    data = new AdditionalData()
                    {
                        Brand = row.GetString("Brand"),
                        BrandNotAnalyzed = row.GetString("Brand"),
                        BrandDescription = row.GetString("BrandDescription"),
                        BrandDescriptionNotAnalyzed = row.GetString("BrandDescription"),
                        BrandControlLabel = row.GetString("MaxSmrt"),
                        Buyer = row.GetString("Buyer"),
                        Cases = row.GetString("Cases"),
                        CategoryId = row.GetString("CategoryId"),
                        CategoryName = row.GetString("CategoryName"),
                        CategoryNameNotAnalyzed = row.GetString("CategoryName"),
                        CatMgr = row.GetString("CatMgr"),
                        Description = Regex.Replace(row.GetString("Description"), @"[^0-9a-zA-Z /\~!@#$%^&*()_]+?", string.Empty),
                        CaseOnly = row.GetString("ICSEOnly"),
                        ItemClass = row.GetString("Class"),
                        ItemType = row.GetString("ItemType"),
                        Kosher = row.GetString("Kosher"),
                        MfrName = row.GetString("MfrName"),
                        MfrNumber = row.GetString("MfrNumber"),
                        Name = row.GetString("Name"),
                        NameNotAnalyzed = row.GetString("Name"),
                        Pack = row.GetString("Pack"),
                        Package = row.GetString("Package"),
                        ParentCategoryId = row.GetString("ParentCategoryId"),
                        ParentCategoryName = row.GetString("ParentCategoryName"),
                        ParentCategoryNameNotAnalyzed = row.GetString("ParentCategoryName"),
                        PreferredItemCode = row.GetString("PreferredItemCode"),
                        Size = row.GetString("Size"),
                        SpecialOrderItem = row.GetString("SpecialOrderItem"),
                        Status1 = row.GetString("Status1"),
                        Status1NotAnalyzed = row.GetString("Status1"),
                        Status2 = row.GetString("Status2"),
                        Upc = row.GetString("UPC"),
                        Vendor1 = row.GetString("Vendor1"),
                        Vendor2 = row.GetString("Vendor2"),
                        BranchId = row.GetString("BranchId"),
                        ReplacedItem = row.GetString("ReplacedItem"),
                        ReplacementItem = row.GetString("ReplacementItem"),
                        ChildNutrition = row.GetString(ItemSpec_CNDoc),
                        SellSheet = row.GetString(ItemSpec_SellSheet),
                        ItemNumber = row.GetString("ItemId"),
						NonStock = row.GetString("NonStock"),
                        TempZone = row.GetString("TempZone"),
                        CatchWeight = row.GetString("HowPrice") == "3",
						IsProprietary = proprietaryItems.ContainsKey(row.GetString("ItemId")),
						ProprietaryCustomers = BuildPropritaryCustomerList(row.GetString("ItemId"), proprietaryItems),
                        Nutritional = new NutritionalInformation()
                        {
                            BrandOwner = row.GetString("BrandOwner"),
                            CountryOfOrigin = row.GetString("CountryOfOrigin"),
                            GrossWeight = row.GetString("GrossWeight"),
                            HandlingInstruction = row.GetString("HandlingInstruction"),
                            Height = row.GetString("Height"),
                            Ingredients = row.GetString("Ingredients"),
                            Length = row.GetString("Length"),
                            MarketingMessage = row.GetString("MarketingMessage"),
                            MoreInformation = row.GetString("MoreInformation"),
                            ServingSize = row.GetString("ServingSize"),
                            ServingSizeUom = row.GetString("ServingSizeUOM"),
                            ServingsPerPack = row.GetString("ServingsPerPack"),
                            ServingSuggestion = row.GetString("ServingSuggestion"),
                            Shelf = row.GetString("Shelf"),
                            StorageTemp = row.GetString("StorageTemp"),
                            UnitMeasure = row.GetString("UnitMeasure"),
                            UnitsPerCase = row.GetString("UnitsPerCase"),
                            Volume = row.GetString("Volume"),
                            Width = row.GetString("Width"),
                            Nutrition = nutrition.ContainsKey(row.GetString("UPC")) ? nutrition[row.GetString("UPC")] : null,
                            Diet = diets.ContainsKey(row.GetString("UPC")) ? diets[row.GetString("UPC")] : null,
                            Allergen = allergens.ContainsKey(row.GetString("UPC")) ? allergens[row.GetString("UPC")] : null,
                        }
                    }

                }
            };
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

        private void CreateOrUpdateContractLists(
            KeithLink.Svc.Core.Models.Profile.UserProfile userProfile
            , KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext
            , string contractNumber)
        {

            List<ListModel> lists = listLogic.ReadListByType(userProfile, userSelectedContext, Core.Models.EF.ListType.Contract, true);

            if (lists.Count == 0 && contractNumber != null && !contractNumber.Equals(String.Empty))
            {
                listLogic.CreateList(   
                    userProfile.UserId 
                    , userSelectedContext 
                    , CreateUserList(
                        contractNumber
                        , GetContractItems(userSelectedContext.CustomerId, userSelectedContext.BranchId, contractNumber)
                        , Core.Models.EF.ListType.Contract
                        , true)
                    , Core.Models.EF.ListType.Contract);
            }
            
            if (lists.Count == 1 && contractNumber != null && !contractNumber.Equals(String.Empty))
            {
                Dictionary<string, ListItemModel> newItemDictionary = CreateListItemDictionary(GetContractItems(userSelectedContext.CustomerId, userSelectedContext.BranchId, contractNumber));
                Dictionary<string, ListItemModel> existingItemDictionary = CreateListItemDictionary(lists[0].Items);
                SortedSet<string> existingItemNumbers = new SortedSet<string>(existingItemDictionary.Keys);
                SortedSet<string> newItemNumbers = new SortedSet<string>(newItemDictionary.Keys);

				if (!Crypto.CalculateMD5Hash(existingItemNumbers).Equals(Crypto.CalculateMD5Hash(newItemNumbers)))
                {
                    CompareNewToExistingListItems(lists[0], newItemDictionary, existingItemDictionary);
                    CompareExistingToNewListItems(newItemDictionary, existingItemDictionary);
                }
                else
                {
                    CompareListItemUpdatedDates(existingItemDictionary);
                }
            }
            else if (lists.Count == 1 && (contractNumber == null || contractNumber.Equals(String.Empty)))
            {
                listLogic.DeleteList(lists[0].ListId);
            }
        }

        private void CreateOrUpdateWorksheetLists(
            KeithLink.Svc.Core.Models.Profile.UserProfile userProfile
            , KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext)
        {
            List<ListModel> lists = listLogic.ReadListByType(userProfile, userSelectedContext, Core.Models.EF.ListType.Worksheet, true);

            if (lists.Count == 0)
            {
                listLogic.CreateList(
                    userProfile.UserId
                    , userSelectedContext
                    , CreateUserList(
                        String.Empty
                        , GetWorksheetItems(userSelectedContext.CustomerId, userSelectedContext.BranchId)
                        , Core.Models.EF.ListType.Worksheet
                        , true)
                    , Core.Models.EF.ListType.Worksheet);
            }

            if (lists.Count == 1)
            {
                Dictionary<string, ListItemModel> newItemDictionary = CreateListItemDictionary(GetWorksheetItems(userSelectedContext.CustomerId, userSelectedContext.BranchId));
                Dictionary<string, ListItemModel> existingItemDictionary = CreateListItemDictionary(lists[0].Items);
                SortedSet<string> existingItemNumbers = new SortedSet<string>(existingItemDictionary.Keys);
                SortedSet<string> newItemNumbers = new SortedSet<string>(newItemDictionary.Keys);

				if (!CalculateMD5Hash(existingItemNumbers).Equals(GenerateMD5Hash(newItemNumbers)))
                {
                    CompareNewToExistingListItems(lists[0], newItemDictionary, existingItemDictionary);
                    CompareExistingToNewListItems(newItemDictionary, existingItemDictionary);
                }
                else
                {
                    CompareListItemUpdatedDates(existingItemDictionary);
                } 
                
            }
            
        }

        private KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext CreateUserSelectedContext(string customerNumber, string branchId)
        {
            KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext = new KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext();
            userSelectedContext.BranchId = branchId;
            userSelectedContext.CustomerId = customerNumber;
            return userSelectedContext;
        }

        private ListModel CreateUserList(string name, List<ListItemModel> items, Core.Models.EF.ListType listType, bool readOnly)
        {
            ListModel list = new ListModel();
            list.Name = GetListName(name, listType);
            list.Items = items;
            list.ReadOnly = readOnly;
            return list;
        }

        private string GetListName(string name, Core.Models.EF.ListType listType)
        {
            string listName = name;

            switch (listType)
            {
                case Core.Models.EF.ListType.Contract:
                    listName = "Contract - " + listName;
                    break;
                case Core.Models.EF.ListType.Worksheet:
                    listName = "History";
                    break;
            }

            return listName;
        }

        private List<ListItemModel> GetContractItems(
             string customerNumber,
             string branchId,
             string contractId
             )
        {
            List<ListItemModel> contractItems = stagingRepository
                            .ReadContractItems(customerNumber, branchId, contractId)
                            .AsEnumerable()
                            .Select(itemRow =>
                                new ListItemModel
                                {
                                    ItemNumber = itemRow.GetString("ItemNumber"),
                                    Position = itemRow.GetInt("BidLineNumber"),
                                    Category = itemRow.GetString("CategoryDescription")
                                }).ToList();
            return contractItems;
        }

        private List<ListItemModel> GetWorksheetItems(
             string customerNumber,
             string branchId
             )
        {
            List<ListItemModel> contractItems = stagingRepository
                            .ReadWorksheetItems(customerNumber, branchId)
                            .AsEnumerable()
                            .Select(itemRow =>
                                new ListItemModel
                                {
                                    ItemNumber = itemRow.GetString("ItemNumber"),
                                }).ToList();
            return contractItems;
        }
		        
        private void CompareContractItems(
            KeithLink.Svc.Core.Models.Profile.UserProfile userProfile
            , KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext userSelectedContext
            , string contractNumber)
        {
            //get existing contract items from list logic
            //Dictionary<string, ListItemModel> existingContractItems = listLogic.ReadListByType(userProfile, userSelectedContext, Core.Models.EF.ListType.Contract).Itesm;

            //get new contract items from app_data

            
            Dictionary<string, ListItemModel> testDictionary = new Dictionary<string, ListItemModel>();
            List<string> keys = testDictionary.Keys.ToList<string>();
        }

        private Dictionary<string, ListItemModel> CreateListItemDictionary(List<ListItemModel> listItems)
        {
            Dictionary<string, ListItemModel> itemDictionary = new Dictionary<string, ListItemModel>();
            
            foreach (ListItemModel lim in listItems)
            {
                itemDictionary.Add(lim.ItemNumber, lim);
            }

            return itemDictionary;
        }

        private void CompareNewToExistingListItems(ListModel list, Dictionary<string, ListItemModel> newItemDictionary, Dictionary<string, ListItemModel> existingItemDictionary)
        {
            foreach (KeyValuePair<string, ListItemModel> kvp in newItemDictionary)
            {
                if (existingItemDictionary.ContainsKey(kvp.Key))
                {
                    if (existingItemDictionary[kvp.Key].Status == Core.Enumerations.List.ListItemStatus.Added &&
                        existingItemDictionary[kvp.Key].ModifiedUtc.Day <= DateTime.Today.AddDays(Configuration.ListItemsDaysNew * -1).Day)
                    {
                        existingItemDictionary[kvp.Key].Status = Core.Enumerations.List.ListItemStatus.Current;
                        listLogic.UpdateItem(existingItemDictionary[kvp.Key]);
                    }
                    if (existingItemDictionary[kvp.Key].Status == Core.Enumerations.List.ListItemStatus.Deleted)
                    {
                        existingItemDictionary[kvp.Key].Status = Core.Enumerations.List.ListItemStatus.Added;
                        listLogic.UpdateItem(existingItemDictionary[kvp.Key]);
                    }
                }
                else
                {
                    newItemDictionary[kvp.Key].Status = Core.Enumerations.List.ListItemStatus.Added;
                    listLogic.AddItem(list.ListId, newItemDictionary[kvp.Key]);
                }
            }

            foreach (KeyValuePair<string, ListItemModel> kvp in existingItemDictionary)
            {
                if (!newItemDictionary.ContainsKey(kvp.Key))
                {
                    existingItemDictionary[kvp.Key].Status = Core.Enumerations.List.ListItemStatus.Deleted;
                    listLogic.UpdateItem(existingItemDictionary[kvp.Key]);
                }
            }
        }

        private void CompareExistingToNewListItems(Dictionary<string, ListItemModel> newItemDictionary, Dictionary<string, ListItemModel> existingItemDictionary)
        {
            foreach (KeyValuePair<string, ListItemModel> kvp in existingItemDictionary)
            {
                if (kvp.Value.Status == Core.Enumerations.List.ListItemStatus.Added &&
                    kvp.Value.ModifiedUtc.Day <= DateTime.Today.AddDays(Configuration.ListItemsDaysNew * -1).Day)
                {
                    existingItemDictionary[kvp.Key].Status = Core.Enumerations.List.ListItemStatus.Current;
                    listLogic.UpdateItem(existingItemDictionary[kvp.Key]);
                }
            }
        }

        private void CompareListItemUpdatedDates(Dictionary<string, ListItemModel> existingItemDictionary)
        {
            foreach (KeyValuePair<string, ListItemModel> kvp in existingItemDictionary)
            {
                if (kvp.Value.Status == Core.Enumerations.List.ListItemStatus.Added &&
                    kvp.Value.ModifiedUtc.Day <= DateTime.Today.AddDays(Configuration.ListItemsDaysNew * -1).Day)
                {
                    existingItemDictionary[kvp.Key].Status = Core.Enumerations.List.ListItemStatus.Current;
                    listLogic.UpdateItem(existingItemDictionary[kvp.Key]);
                }
            }
        }

        #endregion
        #endregion

    }
}
