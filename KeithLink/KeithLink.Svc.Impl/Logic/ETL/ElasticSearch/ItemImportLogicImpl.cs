// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

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

namespace KeithLink.Svc.Impl.Logic.ETL {
    public class ItemImportLogicImpl : IItemImport {

        #region attributes
        private const string Language = "en-US";
        // ItemSpec_NonStock is not being used, remove it?
		private readonly string ItemSpec_NonStock = "NonStock";
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

        #region functions

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

        /// <summary>
        /// Builds a list of proprietary customers
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <param name="proprietaryItems"></param>
        /// <returns></returns>
        private string BuildProprietaryCustomerList(string itemNumber, Dictionary<string, List<string>> proprietaryItems)
		{
			if (!proprietaryItems.ContainsKey(itemNumber))
				return null;

			var sbCustomerList = new StringBuilder();

			foreach (var customer in proprietaryItems[itemNumber])
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
				if (proprietaryItems.ContainsKey(row.GetString("ItemNumber")))
				{
					proprietaryItems[row.GetString("ItemNumber")].Add(row.GetString("CustomerNumber"));
				}
				else
					proprietaryItems.Add(row.GetString("ItemNumber"), new List<string>() { row.GetString("CustomerNumber") });
			}

			return proprietaryItems;
		}

        /// <summary>
        /// Imports current staging item data to Elastic Search
        /// </summary>
        public void ImportItems() {
            //For performance debugging purposes
            var startTime = DateTime.Now;

            var branches = _stagingRepository.ReadAllBranches();

            //Parallel.ForEach(branches.AsEnumerable(), row =>
            foreach (DataRow row in branches.Rows) {
                if (!_elasticSearchRepository.CheckIfIndexExist( row.GetString( "BranchId" ).ToLower() )) {
                    _elasticSearchRepository.CreateEmptyIndex( row.GetString( "BranchId" ).ToLower() );
                    _elasticSearchRepository.MapProductProperties( row.GetString( "BranchId" ).ToLower(), ProductMapping );
                } else
                    _elasticSearchRepository.RefreshSynonyms( row.GetString( "BranchId" ).ToLower() );
            }

            var dataTable = _stagingRepository.ReadFullItemForElasticSearch();
            var products = new BlockingCollection<ItemUpdate>();

            var gsData = _stagingRepository.ReadGSDataForItems();

            var itemNutritions = BuildNutritionDictionary( gsData );
            var itemDiet = BuildDietDictionary( gsData );
            var itemAllergens = BuildAllergenDictionary( gsData );
            var proprietaryItems = BuildProprietaryItemDictionary( _stagingRepository.ReadProprietaryItems() );

            //Parallel.ForEach(dataTable.AsEnumerable(), row =>
            foreach (DataRow row in dataTable.Rows) {
                products.Add( PopulateElasticSearchItem( row, itemNutritions, itemDiet, itemAllergens, proprietaryItems ) );
            };

            int totalProcessed = 0;

            while (totalProcessed < products.Count) {
                var batch = products.Skip( totalProcessed ).Take( Configuration.ElasticSearchBatchSize ).ToList();

                _elasticSearchRepository.Create( string.Concat( batch.Select( i => i.ToJson() ) ) );

                totalProcessed += Configuration.ElasticSearchBatchSize;
            }

            _eventLog.WriteInformationLog( string.Format( "ImportItemsToElasticSearch Runtime - {0}", (DateTime.Now - startTime).ToString( "h'h 'm'm 's's'" ) ) );
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
            // TODO: Find out why this is commented out
			//data.IsProprietary = proprietaryItems.ContainsKey(row.GetString("ItemId"));
            data.IsProprietary = row.GetString("ItemType").Equals("P") ? true : false;
			data.ProprietaryCustomers = BuildProprietaryCustomerList(row.GetString("ItemId"), proprietaryItems);
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
            // TODO: Find out why this is commented out
			//if(row.GetString("NonStock").Equals("y", StringComparison.CurrentCultureIgnoreCase))
			//	item.index.data.itemspecification.Add(ItemSpec_NonStock);
            if (item.index.data.SellSheet.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                item.index.data.ItemSpecification.Add(ItemSpec_SellSheet_FriendlyName);

            return item;
        }
        #endregion

        #region properties
        #endregion
    }
}
