// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.ETL.ElasticSearch;
using KeithLink.Svc.Core.Interface.InternalCatalog;

using KeithLink.Svc.Impl.Models.ElasticSearch.Item;
using KeithLink.Svc.Impl.Models.ETL;

// Core
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Sql;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.ElasticSearch;
using KeithLink.Svc.Core.Models.ElasticSearch.Item;

namespace KeithLink.Svc.Impl.Logic.ETL {
    public class ItemImportLogicImpl : IItemImport {

        #region attributes
        private const string Language = "en-US";
		private readonly string ItemSpec_ReplacementItem = "ReplacementItem";
		private readonly string ItemSpec_Replaced = "ItemBeingReplaced";
		private readonly string ItemSpec_CNDoc = "CNDoc";
        private readonly string ItemSpec_CNDoc_FriendlyName = "childnutrition";
        private readonly string ItemSpec_SellSheet = "FDAProductFlag";
        private readonly string ItemSpec_SellSheet_FriendlyName = "sellsheet";

        // Allergens
        private const string LEVEL_OF_CONTAINMENT = "LevelOfContainment";
        private const string ALLERGEN_TYPE_DESC = "AllergenTypeDesc";
        private const string FREE_FROM = "FREE_FROM";
        private const string CONTAINS = "CONTAINS";
        private const string MAY_CONTAIN = "MAY_CONTAIN";

        private IStagingRepository _stagingRepository;
        private IElasticSearchRepository _elasticSearchRepository;
        private IEventLogRepository _eventLog;
        #endregion

        #region constructor

        public ItemImportLogicImpl(IStagingRepository stagingRepository,
                                                IElasticSearchRepository elasticSearchRepository,
                                                IEventLogRepository eventLogRepository) {

            _stagingRepository = stagingRepository;
            _elasticSearchRepository = elasticSearchRepository;
            _eventLog = eventLogRepository;
        }

        #endregion

        #region methods
        /// <summary>
        /// Imports current staging item data to Elastic Search
        /// </summary>
        public void ImportItems()
        {
            try
            {
                DateTime start = DateTime.Now;
                _eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import items to ES {0}", start.ToString()));

                DataSet gsData = _stagingRepository.ReadGSDataForItems();

                Dictionary<string, PdmEnrichedItem> pdmDict = BuildPDMDictionary();

                Dictionary<string, List<ItemNutrition>> itemNutritions = BuildNutritionDictionary(gsData);

                Dictionary<string, List<Diet>> itemDiet = BuildDietDictionary(gsData);

                Dictionary<string, Allergen> itemAllergens = BuildAllergenDictionary(gsData);

                Dictionary<string, List<string>> proprietaryItems = 
                    BuildProprietaryItemDictionary(_stagingRepository.ReadProprietaryItems());

                DataTable branches = _stagingRepository.ReadAllBranches();

                DataTable itemKeywordsTable = _stagingRepository.ReadAllItemKeywords();

                Dictionary<string, string> itemKeywords = new Dictionary<string, string>();
                foreach (DataRow row in itemKeywordsTable.Rows) {
                    itemKeywords.Add(row.GetString("ItemNumber"), row.GetString("Keywords"));
                }

                foreach (DataRow row in branches.Rows)
                {
                    _eventLog.WriteInformationLog(String.Format("ETL: Import Item Process :  Start Import items to ES branch {0}", row.GetString("BranchId").ToLower()));
                    try
                    {
                        if (!_elasticSearchRepository.CheckIfIndexExist(row.GetString("BranchId").ToLower()))
                        {
                            _elasticSearchRepository.CreateEmptyIndex(row.GetString("BranchId").ToLower());
                            _elasticSearchRepository.MapProductProperties(row.GetString("BranchId").ToLower(), GetProductMapping());
                        }
                        else
                            _elasticSearchRepository.RefreshSynonyms(row.GetString("BranchId").ToLower());
                    }
                    catch (Exception ex1)
                    {
                        _eventLog.WriteErrorLog(String.Format("ETL: Error Importing items to ES -- error updating indexes or synonyms.  {0} -- {1}", ex1.Message, ex1.StackTrace));

                        KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex1,
                            "ETL: Error Importing items to ES -- error updating indexes or synonyms.");
                    }

                    try
                    {
                        DataTable dataTable = 
                            _stagingRepository.ReadFullItemForElasticSearch(row.GetString("BranchId").ToLower());

                        List<string> ESItems = 
                            _elasticSearchRepository.ReadListOfProductsByBranch
                                (row.GetString("BranchId").ToLower());

                        BlockingCollection<IESItem> products = new BlockingCollection<IESItem>();

                        foreach (DataRow rowItem in dataTable.Rows)
                        {
                            if (rowItem.GetString("BranchId").Equals
                                (row.GetString("BranchId"), StringComparison.CurrentCultureIgnoreCase))
                            {
                                PdmEnrichedItem pdmItem = null;

                                if (pdmDict.ContainsKey(rowItem.GetString("ItemId")))
                                {
                                    pdmItem = pdmDict[rowItem.GetString("ItemId")];
                                }

                                products.Add(PopulateElasticSearchItem(rowItem, itemNutritions, itemDiet,
                                                                       itemAllergens, proprietaryItems, pdmItem, 
                                                                       ESItems, itemKeywords));

                                ESItems.Remove(rowItem.GetString("ItemId"));
                            }
                        };

                        foreach(string toDelete in ESItems)
                        {
                            ItemDelete del = new ItemDelete();
                            del.delete = new RootData();
                            del.delete._id = toDelete;
                            del.delete._index = row.GetString("BranchId").ToLower();
                            products.Add(del);
                        }

                        int totalProcessed = 0;

                        while (totalProcessed < products.Count)
                        {
                            try
                            {
                                var batch = products.Skip(totalProcessed).Take(Configuration.ElasticSearchBatchSize).ToList();

                                _elasticSearchRepository.Create(string.Concat(batch.Select(i => i.ToJson())));

                                totalProcessed += Configuration.ElasticSearchBatchSize;
                            }
                            catch (Exception ex2)
                            {
                                _eventLog.WriteErrorLog(String.Format("ETL: Error Importing items to ES -- error importing individual item to ES.  {0} -- {1}", ex2.Message, ex2.StackTrace));

                                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex2, 
                                    "ETL: Error Importing items to ES -- error importing individual item to ES.");
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        _eventLog.WriteErrorLog(String.Format("ETL: Error Importing items to ES -- error updating indexes or synonyms.  {0} -- {1}", ex1.Message, ex1.StackTrace));

                        KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex1, 
                            "ETL: Error Importing items to ES -- error updating indexes or synonyms.");
                    }
                    _eventLog.WriteInformationLog(String.Format("ETL: Import Item Process :  End Import items to ES branch {0}", row.GetString("BranchId").ToLower()));
                }



                TimeSpan took = DateTime.Now - start;
                _eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import items to ES.  Process took {0}", took.ToString()));
            }
            catch (Exception e)
            {
                _eventLog.WriteErrorLog(String.Format("ETL: Error Importing items to ES -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));

                KeithLink.Common.Impl.Email.ExceptionEmail.Send(e, 
                    "ETL: Error Importing items to ES -- whole process failed.");
            }

        }

        public void ImportUNFIItems()
		{
			try
			{
				DateTime start = DateTime.Now;
				_eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import UNFI items to ES {0}", start.ToString()));

                var warehouses = _stagingRepository.ReadDistinctUNFIWarehouses();

				var products = new BlockingCollection<IESItem>();

                var items = _stagingRepository.ReadUNFIItems();

                foreach (var warehouse in warehouses)
				{
					if (!_elasticSearchRepository.CheckIfIndexExist(string.Format("unfi_{0}", warehouse)))
					{
						_elasticSearchRepository.CreateEmptyIndex(string.Format("unfi_{0}", warehouse));
						_elasticSearchRepository.MapProductProperties(string.Format("unfi_{0}", warehouse), GetProductMapping());
					}

                    List<string> ESItems =
                        _elasticSearchRepository.ReadListOfProductsByBranch
                            (string.Format("unfi_{0}", warehouse));

                    foreach (DataRow row in items.Rows)
                    {
                        if (row.GetString("WarehouseNumber").Equals(warehouse))
                        {
                            products.Add(PopulateUNFIElasticSearchItem(row, ESItems));
                        }

                        ESItems.Remove(row.GetString("ProductNumber"));
                    };

                    foreach (string toDelete in ESItems)
                    {
                        ItemDelete del = new ItemDelete();
                        del.delete = new RootData();
                        del.delete._id = toDelete;
                        del.delete._index = string.Format("unfi_{0}", warehouse);
                        products.Add(del);
                    }
                }

                int totalProcessed = 0;

				while (totalProcessed < products.Count)
				{
					try
					{
						var batch = products.Skip(totalProcessed).Take(Configuration.ElasticSearchBatchSize).ToList();

						_elasticSearchRepository.Create(string.Concat(batch.Select(i => i.ToJson())));

						totalProcessed += Configuration.ElasticSearchBatchSize;
					}
					catch (Exception ex2)
					{
						_eventLog.WriteErrorLog(String.Format("ETL: Error Importing items to ES -- error importing individual item to ES.  {0} -- {1}", ex2.Message, ex2.StackTrace));

                        KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex2, 
                            "ETL: Error Importing items to ES -- error importing individual item to ES.");
                    }
                }


				TimeSpan took = DateTime.Now - start;
				_eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import UNFI items to ES.  Process took {0}", took.ToString()));
			}
			catch (Exception e)
			{
				_eventLog.WriteErrorLog(String.Format("ETL: Error Importing UNFI items to ES -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));

                KeithLink.Common.Impl.Email.ExceptionEmail.Send(e, 
                    "ETL: Error Importing UNFI items to ES -- whole process failed.");
            }
        }

        #endregion

        #region helper methods

        /// <summary>
        /// Builds a dictionary of Allergen information from a DataSet
        /// </summary>
        /// <param name="gsData"></param>
        /// <returns></returns>
        private Dictionary<string, Allergen> BuildAllergenDictionary(DataSet gsData)
        {
            var itemAllergen = new Dictionary<string, Allergen>();

            foreach (DataRow row in gsData.Tables[2].Rows)
            {
				if (itemAllergen.ContainsKey(row.GetString("gtin")))
				{
					Allergen existing = itemAllergen[row.GetString("gtin")];
					MapAllergen(ref existing, row);
				}
				else
				{
					Allergen newItem = new Allergen();
					MapAllergen(ref newItem, row);
					itemAllergen.Add(row.GetString("gtin"), newItem);
				}
            }

            return itemAllergen;
        }

        /// <summary>
        /// Builds a dictionary of diet information from a dataset
        /// </summary>
        /// <param name="gsData"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Builds a dictionary of nutritional information
        /// </summary>
        /// <param name="gsData">DataSet</param>
        /// <returns></returns>
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

        private Dictionary<string, PdmEnrichedItem> BuildPDMDictionary() {
            DataTable items = _stagingRepository.ReadPDMDataForItems();

            return items.AsEnumerable()
                        .Select(i => new PdmEnrichedItem {
                            Brand          = i.Field<string>("brand"),
                            CreatedBy      = i.Field<string>("CreatedBy"),
                            CreatedUTC     = i.Field<DateTime>("CreatedUTC"),
                            Desription     = i.Field<string>("Description"),
                            ItemNumber     = i.Field<string>("ItemNumber"),
                            Manufacturer   = i.Field<string>("Manufacturer"),
                            ModifiedUTC    = i.Field<DateTime>("ModifiedUTC"),
                            Name           = i.Field<string>("Name"),
                            Status         = i.Field<string>("Status"),
                            UpdatedBy      = i.Field<string>("UpdatedBy")
                        })
                        .ToDictionary(i => i.ItemNumber);
        }

        /// <summary>
        /// Builds a list of proprietary customers
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <param name="proprietaryItems"></param>
        /// <returns></returns>
        private string BuildProprietaryCustomerList(string key, Dictionary<string, List<string>> proprietaryItems)
		{
			if (!proprietaryItems.ContainsKey(key))
				return null;

			var sbCustomerList = new StringBuilder();

			foreach (var customer in proprietaryItems[key])
				sbCustomerList.AppendFormat("{0} ", customer);

			return sbCustomerList.ToString();

		}

        /// <summary>
        /// Builds a dictionary of properietary items
        /// </summary>
        /// <param name="propItemData"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> BuildProprietaryItemDictionary(DataTable propItemData)
		{
			var proprietaryItems = new Dictionary<string, List<string>>();

			foreach (DataRow row in propItemData.Rows)
			{
                string key = string.Format("{0}-{1}", row.GetString("ItemNumber"), row.GetString("DivisionNumber"));
                if (proprietaryItems.ContainsKey(key))
				{
					proprietaryItems[key].Add(row.GetString("CustomerNumber"));
				}
				else
					proprietaryItems.Add(key, new List<string>() { row.GetString("CustomerNumber") });
			}

			return proprietaryItems;
		}

        private string GetProductMapping(){
            dynamic mapping = new {
                product = new {
                    properties = new {
                        categoryname_not_analyzed = new { type = "keyword", index = "true" },
                        parentcategoryname_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        brand_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        marketingbrand_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        brand_description_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        keywords = new {
                            type = "string",
                            analyzer = "keyword",
                            fields = new {
                                english = new {
                                    type = "string",
                                    analyzer = "english"
                                },
                                whitespace = new {
                                    type = "string",
                                    analyzer = "whitespace_analyzer",
                                    search_analyzer = "whitespace_analyzer"
                                }
                            }
                        },
                        description = new {
                            type = "string",
                            fields = new {
                                english = new {
                                    type = "string",
                                    analyzer = "english"
                                }
                            }
                        },
                        marketingdescription = new {
                            type = "string",
                            fields = new {
                                english = new {
                                    type = "string",
                                    analyzer = "english"
                                }
                            }
                        },
                        name = new {
                            type = "string",
                            fields = new {
                                english = new {
                                    type = "string",
                                    analyzer = "english"
                                },
                                ngram = new {
                                    type = "string",
                                    analyzer = "ngram_analyzer"
                                }
                            }
                        },
                        marketingname = new {
                            type = "string",
                            fields = new {
                                english = new {
                                    type = "string",
                                    analyzer = "english"
                                },
                                ngram = new {
                                    type = "string",
                                    analyzer = "ngram_analyzer"
                                }
                            }
                        },
                        itemnumber = new {
                            type = "string",
                            fields = new {
                                ngram = new {
                                    type = "string",
                                    analyzer = "ngram_analyzer",
                                    search_analyzer = "whitespace_analyzer"
                                }
                            }
                        },
                        mfrnumber = new {
                            type = "string",
                            fields = new {
                                ngram = new {
                                    type = "string",
                                    analyzer = "ngram_analyzer",
                                    search_analyzer = "whitespace_analyzer"
                                }
                            }
                        },
                        gtin = new {
                            type = "string",
                            fields = new {
                                ngram = new {
                                    type = "string",
                                    analyzer = "ngram_analyzer",
                                    search_analyzer = "whitespace_analyzer"
                                }
                            }
                        },
                        upc = new {
                            type = "string",
                            fields = new {
                                ngram = new {
                                    type = "string",
                                    analyzer = "ngram_analyzer",
                                    search_analyzer = "whitespace_analyzer"
                                }
                            }
                        },
                        name_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        name_ngram_analyzed = new { type = "text", analyzer = "ngram_analyzer", search_analyzer = "whitespace_analyzer" },
                        marketingname_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        marketingname_ngram_analyzed = new { type = "string", analyzer = "ngram_analyzer", search_analyzer = "whitespace_analyzer" },
                        mfrname_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        marketingmanufacturer_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        preferreditemcode = new { type = "keyword", index = "not_analyzed" },
                        status1_not_analyzed = new { type = "keyword", index = "not_analyzed" },
                        nutritional = new {
                            @properties = new {
                                diet = new {
                                    @properties = new {
                                        diettype = new { type = "keyword", index = "not_analyzed" }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(mapping);
        }

        /// <summary>
        /// Maps Allergen information, this update happens by reference
        /// </summary>
        /// <param name="allergen">updated by reference</param>
        /// <param name="row"></param>
        private void MapAllergen(ref Allergen allergen, DataRow row)
		{
			switch (row.GetString(LEVEL_OF_CONTAINMENT))
			{
				case FREE_FROM:
					allergen.FreeFrom.Add(row.GetString(ALLERGEN_TYPE_DESC));
					break;
				case CONTAINS:
					allergen.Contains.Add(row.GetString(ALLERGEN_TYPE_DESC));
					break;
				case MAY_CONTAIN:
					allergen.MayContain.Add(row.GetString(ALLERGEN_TYPE_DESC));
					break;
			}
		}

        /// <summary>
        /// Takes a DataRow and tries to map it to a Diet object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Diet MapDiet(DataRow row)
        {
            return new Diet() { DietType = row.GetString("DietType") };
        }

        /// <summary>
        /// Takes a DataRow and tries to map it to an object
        /// </summary>
        /// <param name="subRow"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Populate ItemUpdate with ElasticSearch data to be pushed
        /// </summary>
        /// <param name="row"></param>
        /// <param name="nutrition"></param>
        /// <param name="diets"></param>
        /// <param name="allergens"></param>
        /// <param name="proprietaryItems"></param>
        /// <returns></returns>
        private IESItem PopulateElasticSearchItem(DataRow row, Dictionary<string, List<ItemNutrition>> nutrition, 
                                                     Dictionary<string, List<Diet>> diets, 
                                                     Dictionary<string, Allergen> allergens, 
                                                     Dictionary<string, List<string>> proprietaryItems, 
                                                     PdmEnrichedItem pdmData, List<string> existingItems,
                                                     Dictionary<string, string> itemKeywords) {
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
            data.BrandDescription = (row.GetString("BrandDescription").Length > 0)? 
                                     row.GetString("BrandDescription") : 
                                     row.GetString("Brand");
            data.BrandDescriptionNotAnalyzed = (row.GetString("BrandDescription").Length > 0) ?
                                     row.GetString("BrandDescription") :
                                     row.GetString("Brand");
            data.BrandControlLabel = row.GetString("BrandControlLabel");
            data.Buyer = row.GetString("Buyer");
            data.Cases = row.GetString("Cases");
            data.CategoryId = row.GetString("CategoryId");
            data.CategoryName = row.GetString("CategoryName");
            data.CategoryNameNotAnalyzed = row.GetString("CategoryName");
            data.CatMgr = row.GetString("CatMgr");
            data.Department = row.GetString("Class").Length == 0 ? string.Empty : row.GetString("Class").Substring(0, 1);
            data.Description = Regex.Replace(row.GetString("Description"), @"[^0-9a-zA-Z /\~!@#$%^&*()_]+?", string.Empty);
            data.CaseOnly = row.GetString("ICSEOnly");
            data.ItemClass = row.GetString("Class");
            data.ItemType = row.GetString("ItemType");
            data.Kosher = row.GetString("Kosher");
            if(pdmData != null) {
                data.MarketingBrand = pdmData.Brand;
                data.MarketingBrandNotAnalyzed = pdmData.Brand;
                data.MarketingDescription = pdmData.Desription;
                data.MarketingManufacturer = pdmData.Manufacturer;
                data.MarketingManufacturerNotAnalyzed = pdmData.Manufacturer;
                data.MarketingName = pdmData.Name;
                data.MarketingNameNgramAnalyzed = pdmData.Name;
                data.MarketingNameNotAnalyzed = pdmData.Name;
            }
            data.MfrName = row.GetString("MfrName");
            data.MfrNameNotAnalyzed = row.GetString("MfrName");
            data.MfrNumber = row.GetString("MfrNumber");
            data.Name = row.GetString("Name");
            data.NameNotAnalyzed = row.GetString("Name").ToLower();
            data.NameNGramAnalyzed = row.GetString("Name").ToLower();
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
            data.IsProprietary = row.GetString("ItemType").Equals("P") ? true : false;
            string key = string.Format("{0}-{1}", row.GetString("ItemId"), row.GetString("BranchId"));
            data.ProprietaryCustomers = BuildProprietaryCustomerList(key, proprietaryItems);
            data.AverageWeight = (row.GetDouble("FPNetWt") > 0 ? row.GetDouble("FPNetWt") / 100 : (row.GetDouble("GrossWt") > 0 ? row.GetDouble("GrossWt") / 100 : 0));
            data.Nutritional = nutInfo;

            if (itemKeywords.ContainsKey(row.GetString("ItemId"))) {
                data.Keywords = itemKeywords[row.GetString("ItemId")];
            }
           
            RootData index = new RootData();
            index._id = row.GetString("ItemId");
            index._index = row.GetString("BranchId").ToLower();
            index.data = data;


            if (existingItems.Contains(row.GetString("ItemId")))
            {
                ItemUpdate item = new ItemUpdate();
                item.index = index;

                item.index.data.ItemSpecification = new List<string>();

                if (item.index.data.ReplacementItem != "000000")
                    item.index.data.ItemSpecification.Add(ItemSpec_ReplacementItem);
                if (item.index.data.ReplacedItem != "000000")
                    item.index.data.ItemSpecification.Add(ItemSpec_Replaced);
                if (item.index.data.ChildNutrition.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                    item.index.data.ItemSpecification.Add(ItemSpec_CNDoc_FriendlyName);
                if (item.index.data.SellSheet.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                    item.index.data.ItemSpecification.Add(ItemSpec_SellSheet_FriendlyName);

                return item;
            }
            else
            {
                ItemInsert item = new ItemInsert();
                item.index = index;

                item.index.data.ItemSpecification = new List<string>();

                if (item.index.data.ReplacementItem != "000000")
                    item.index.data.ItemSpecification.Add(ItemSpec_ReplacementItem);
                if (item.index.data.ReplacedItem != "000000")
                    item.index.data.ItemSpecification.Add(ItemSpec_Replaced);
                if (item.index.data.ChildNutrition.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                    item.index.data.ItemSpecification.Add(ItemSpec_CNDoc_FriendlyName);
                // TODO: Find out why this is commented out
                //if(row.GetString("NonStock").Equals("y", StringComparison.CurrentCultureIgnoreCase))
                //	item.index.data.itemspecification.Add(ItemSpec_NonStock);
                if (item.index.data.SellSheet.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                    item.index.data.ItemSpecification.Add(ItemSpec_SellSheet_FriendlyName);

                return item;
            }
        }


		private IESItem PopulateUNFIElasticSearchItem(DataRow row, List<string> existingItems)
		{

			var data = new AdditionalData()
			{
				WarehouseNumber = row.GetString("WarehouseNumber"),
                ItemNumber = row.GetString("ProductNumber"),
				Name = row.GetString("Description"),
                NameNotAnalyzed = row.GetString("Description"),
				Brand = row.GetString("Brand"),
				BrandNotAnalyzed = row.GetString("Brand"),
				BrandDescription = row.GetString("Brand"),
                BrandDescriptionNotAnalyzed = row.GetString("Brand"),
				BranchId = "unfi",
				CLength = row.GetNullableDouble("CLength"),
				CWidth = row.GetNullableDouble("CWidth"),
				CHeight = row.GetNullableDouble("CHeight"),
				TempZone = row.GetString("TempControl"),
				UnitOfSale = row.GetString("UnitOfSale"),
				CatalogDept = row.GetString("CatalogDept"),
				ShipMinExpire = row.GetString("ShipMinExpire"),
				MfrItemNumber = row.GetString("ProductNumber"),
				MinOrder = row.GetNullableInt("MinOrder"),
				VendorCasesPerTier = row.GetNullableInt("VendorCasesPerTier"),
				VendorTiersPerPallet = row.GetNullableInt("VendorTiersPerPallet"),
				VendorCasesPerPallet = row.GetNullableInt("VendorCasesPerPallet"),
				CaseQuantity = row.GetNullableInt("CaseQuantity"),
                Cases = row.GetString("OnHandQty"),
                PutUp = row.GetString("PutUp"),
				ContSize = row.GetNullableDouble("ContSize"),
				ContUnit = row.GetString("ContUnit"),
				TCSCode = row.GetString("TCSCode"),
				Upc = row.GetString("RetailUPC"),
				CaseUPC = row.GetString("CaseUPC"),
				AverageWeight = row.GetDouble("Weight"),
				PLength = row.GetNullableDouble("PLength"),
				PHeight = row.GetNullableDouble("PHeight"),
				PWidth = row.GetNullableDouble("PWidth"),
				Status = row.GetString("Status"),
				ItemType = row.GetString("Type"),
				ParentCategoryName = row.GetString("Category"),
				ParentCategoryNameNotAnalyzed = row.GetString("Category"),
				CategoryName = row.GetString("Subgroup"),
				CategoryNameNotAnalyzed = row.GetString("Subgroup"),
				CategoryId="KO",
				Flag1 = row.GetString("Flag1"),
				Flag2 = row.GetString("Flag2"),
				Flag3 = row.GetString("Flag3"),
				Flag4 = row.GetString("Flag4"),
				OnHandQty = row.GetNullableInt("OnHandQty"),
				Vendor1 = row.GetString("Vendor"),
                CasePrice = row.GetDecimal("CasePrice"),
                PackagePrice = row.GetDecimal("EachPrice"),
                StockedInBranches = row.GetString("StockedInBranches"),
			};

			RootData index = new RootData();
			index._id = data.MfrItemNumber.ToString();
			index._index = string.Format("unfi_{0}", data.WarehouseNumber);
			index.data = data;

            if (existingItems.Contains(data.MfrItemNumber.ToString()))
            {
                ItemUpdate item = new ItemUpdate();
                item.index = index;
                return item;
            }
            else
            {
                ItemInsert item = new ItemInsert();
                item.index = index;
                return item;
            }
            return null;
		}
        #endregion

    }
}
