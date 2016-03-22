﻿// Commerce Server
using CommerceServer.Core.Catalog;
using CommerceServer.Core.Profiles;

// KeithLink
using KeithLink.Common.Core;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.ETL;
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
    public class CatalogLogicImpl : KeithLink.Svc.Core.Interface.ETL.ICatalogLogic
    {
        #region attributes
        private const string Language = "en-US";
        // Should this be removed?
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
                       ""name_edge_ngram_analyzed"" : {
           					""type"" : ""string"",
							""index_analyzer"" : ""ngram_analyzer"",
                            ""search_analyzer"": ""whitespace_analyzer""
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
        private readonly IMessagingLogic messageLogic;
        
        #endregion

        #region constructor
        public CatalogLogicImpl(ICatalogInternalRepository catalogRepository, IStagingRepository stagingRepository, IElasticSearchRepository elasticSearchRepository,
			                    IEventLogRepository eventLog, IUserProfileLogic userProfile, IInternalListLogic listLogic, 
                                IMessagingLogic messageLogic)
        {
            this.catalogRepository = catalogRepository;
            this.stagingRepository = stagingRepository;
            this.elasticSearchRepository = elasticSearchRepository;
			this.eventLog = eventLog;
            this.userProfileLogic = userProfile;
			this.listLogic = listLogic;
            this.messageLogic = messageLogic;
        }
        #endregion

        #region methods
        private MSCommerceCatalogCollection2Catalog[] BuildCatalogs() {
            var catalogs = new List<MSCommerceCatalogCollection2Catalog>();
            var dataTable = stagingRepository.ReadAllBranches();

            foreach(DataRow row in dataTable.Rows) {
                var newCatalog = new MSCommerceCatalogCollection2Catalog() { name = row.GetString("BranchId"), productUID = "ProductId", startDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), endDate = DateTime.Now.AddYears(500).ToString("yyyy-MM-ddTHH:mm:ss"), languages = Language, DefaultLanguage = Language, ReportingLanguage = Language };
                newCatalog.DisplayName = CreateDisplayName(row.GetString("Description"));

                newCatalog.Category = GenerateCategories();
                newCatalog.Product = GenerateProducts(row.GetString("BranchId"));
                catalogs.Add(newCatalog);
            }

            return catalogs.ToArray();
        }

        private MSCommerceCatalogCollection2Catalog[] BuildUNFICatalogs() {
            var catalogs = new List<MSCommerceCatalogCollection2Catalog>();

            var warehouses = stagingRepository.ReadDistinctUNFIWarehouses();

            foreach(var warehouse in warehouses) {
                var newCatalog = new MSCommerceCatalogCollection2Catalog() { name = string.Format("unfi_{0}", warehouse), productUID = "ProductId", startDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), endDate = DateTime.Now.AddYears(500).ToString("yyyy-MM-ddTHH:mm:ss"), languages = Language, DefaultLanguage = Language, ReportingLanguage = Language };
                newCatalog.DisplayName = CreateDisplayName(string.Format("UNFI_{0}", warehouse));

                newCatalog.Category = GenerateUNFICategories();
                newCatalog.Product = GenerateUNFIProducts(warehouse);
                catalogs.Add(newCatalog);
            }

            return catalogs.ToArray();
        }

        private static DisplayName[] CreateDisplayName(string value) {
            return new DisplayName[1] { new DisplayName() { language = Language, Value = value } };
        }

        private MSCommerceCatalogCollection2CatalogCategory[] GenerateCategories() {
            List<MSCommerceCatalogCollection2CatalogCategory> categories = new List<MSCommerceCatalogCollection2CatalogCategory>();

            var prefixesToExclude = Configuration.CategoryPrefixesToExclude.Split(',').ToList();



            var dataTable = stagingRepository.ReadParentCategories();
            var childTable = stagingRepository.ReadSubCategories();

            foreach(DataRow cat in dataTable.Rows) {
                var newSubCat = new MSCommerceCatalogCollection2CatalogCategory() { name = cat.GetString("CategoryId"), Definition = "Category" };
                newSubCat.DisplayName = CreateDisplayName(cat.GetString("CategoryName"));
                categories.Add(newSubCat);
            }

            foreach(DataRow subCat in childTable.Rows) {

                if(prefixesToExclude.Contains(subCat.GetString("CategoryId").Substring(0, 2)))
                    continue;

                var newSubCat = new MSCommerceCatalogCollection2CatalogCategory() { name = subCat.GetString("CategoryId"), Definition = "Category" };
                newSubCat.DisplayName = CreateDisplayName(subCat.GetString("CategoryName"));
                newSubCat.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = string.Format("{0}000", subCat.GetString("CategoryId", true).Substring(0, 2)) } };
                categories.Add(newSubCat);


            }

            return categories.ToArray();
        }

        private MSCommerceCatalogCollection2CatalogProduct[] GenerateProducts(string branchId) {
            var products = new List<MSCommerceCatalogCollection2CatalogProduct>();
            var itemTable = stagingRepository.ReadItems(branchId);
            var prefixesToExclude = Configuration.CategoryPrefixesToExclude.Split(',').ToList();

            foreach(DataRow row in itemTable.Rows) {
                if(prefixesToExclude.Contains(row.GetString("CategoryId").Substring(0, 2)))
                    continue;

                var newProd = new MSCommerceCatalogCollection2CatalogProduct() { ProductId = row.GetString("ItemId"), Definition = "Item" };
                newProd.DisplayName = new DisplayName[1] { new DisplayName() { language = "en-US", Value = row.GetString("Name") } };
                newProd.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = row.GetString("CategoryId"), rank = "0" } };
                newProd.listprice = "0";
                products.Add(newProd);
            }

            return products.ToArray();
        }

        private MSCommerceCatalogCollection2CatalogCategory[] GenerateUNFICategories() {
            List<MSCommerceCatalogCollection2CatalogCategory> categories = new List<MSCommerceCatalogCollection2CatalogCategory>();

            var prefixesToExclude = Configuration.CategoryPrefixesToExclude.Split(',').ToList();



            var dataTable = stagingRepository.ReadUNFIItems();
            DataView dataView = new DataView(dataTable);
            var parentCategories = dataView.ToTable(true, "Category");
            var childCategories = dataView.ToTable(true, "Category", "Subgroup");

            foreach(DataRow cat in parentCategories.Rows) {
                var newSubCat = new MSCommerceCatalogCollection2CatalogCategory() { name = cat.GetString("Category"), Definition = "Category" };
                newSubCat.DisplayName = CreateDisplayName(cat.GetString("Category"));
                categories.Add(newSubCat);
            }

            foreach(DataRow subCat in childCategories.Rows) {
                var newSubCat = new MSCommerceCatalogCollection2CatalogCategory() { name = subCat.GetString("Subgroup"), Definition = "Category" };
                newSubCat.DisplayName = CreateDisplayName(subCat.GetString("Subgroup"));
                newSubCat.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = subCat.GetString("Category") } };
                categories.Add(newSubCat);


            }

            return categories.ToArray();
        }

        private MSCommerceCatalogCollection2CatalogProduct[] GenerateUNFIProducts(string warehouse) {
            var products = new List<MSCommerceCatalogCollection2CatalogProduct>();
            var itemTable = stagingRepository.ReadUNFIItems(warehouse);


            foreach(DataRow row in itemTable.Rows) {
                var newProd = new MSCommerceCatalogCollection2CatalogProduct() { ProductId = row.GetString("ProductNumber"), Definition = "Item" };
                newProd.DisplayName = new DisplayName[1] { new DisplayName() { language = "en-US", Value = row.GetString("Description") } };
                newProd.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = row.GetString("Subgroup"), rank = "0" } };
                newProd.listprice = "0";
                products.Add(newProd);
            }

            return products.ToArray();
        }

        public void ImportCatalog()
        {
			try
			{
				DateTime start = DateTime.Now;
				eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import catalog to CS {0}", start.ToString()));

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

				TimeSpan took = DateTime.Now - start;
				eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import catalog to CS.  Process took {0}", took.ToString()));
			}
			catch (Exception e)
			{
				eventLog.WriteErrorLog(String.Format("ETL: Error Importing catalog -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));
			}
        }

		public void ImportUNFICatalog()
		{
			try
			{
				DateTime start = DateTime.Now;
				eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import catalog to CS {0}", start.ToString()));

				//Create root level catalog object
				MSCommerceCatalogCollection2 catalog = new MSCommerceCatalogCollection2();
				catalog.version = "3.0"; //Required for the import to work

				//Create the BaseCatalog
				catalog.Catalog = BuildUNFICatalogs();

				var memoryStream = new MemoryStream();
				var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.Unicode);
				var serializer = new XmlSerializer(typeof(MSCommerceCatalogCollection2));

				serializer.Serialize(streamWriter, catalog);
				memoryStream.Position = 0;
				var catalogNames = string.Join(",", catalog.Catalog.Select(c => c.name).ToList().ToArray());

				catalogRepository.ImportXML(new CatalogImportOptions() { Mode = ImportMode.Full, TransactionMode = TransactionMode.NonTransactional, CatalogsToImport = catalogNames }, memoryStream);

				TimeSpan took = DateTime.Now - start;
				eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import catalog to CS.  Process took {0}", took.ToString()));
			}
			catch (Exception e)
			{
				eventLog.WriteErrorLog(String.Format("ETL: Error Importing catalog -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));
			}
		}
        #endregion  
    }
}
