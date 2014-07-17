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

namespace KeithLink.Svc.Impl.ETL
{
    public class CategoryLogicImpl: ICategoryLogic
    {
        private const string Language = "en-US";

        public void ImportCatalog()
        {
            //Create root level catalog object
            MSCommerceCatalogCollection2 catalog = new MSCommerceCatalogCollection2();
            catalog.version = "3.0"; //Required for the import to work

            //Create the BaseCatalog
            catalog.Catalog = BuildCatalogs(); 
            
            CatalogSiteAgent catalogSiteAgent = new CatalogSiteAgent(); //TODO: Switch to solution wide method for connecting to CS
            catalogSiteAgent.SiteName = Configuration.CSSiteName;
            catalogSiteAgent.IgnoreInventorySystem = false;

            CatalogContext context = CatalogContext.Create(catalogSiteAgent);
            
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.Unicode);
            var serializer = new XmlSerializer(typeof(MSCommerceCatalogCollection2));

            TextWriter tw = new StreamWriter(@"C:\Dev\TestExportFinal.xml", false, Encoding.Unicode);
            serializer.Serialize(tw, catalog);
            tw.Close();

            serializer.Serialize(streamWriter, catalog);
            memoryStream.Position = 0;
            var catalogNames = string.Join(",", catalog.Catalog.Select(c => c.name).ToList().ToArray());
            ImportProgress importProgress = context.ImportXml(new CatalogImportOptions() { Mode = ImportMode.Full, TransactionMode = TransactionMode.NonTransactional, CatalogsToImport = catalogNames }, memoryStream);
            while (importProgress.Status == CatalogOperationsStatus.InProgress)
            {
                System.Threading.Thread.Sleep(3000);
                // Call the refresh method to refresh the current status
                importProgress.Refresh();
            }
            //TODO: Log an errors that occured during the import
        }

        private MSCommerceCatalogCollection2Catalog[] BuildCatalogs()
        {
            var catalogs = new List<MSCommerceCatalogCollection2Catalog>();
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(ReadBranches, conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }                
            }

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
        
        #region Helper Methods
        private MSCommerceCatalogCollection2VirtualCatalog[] GenerateVirtualCatalogs()
        {
            var dataTable = new DataTable();
            var exclusionTable = new DataTable();
            var virtualCatalogs = new List<MSCommerceCatalogCollection2VirtualCatalog>();

            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(ReadBranches, conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }

                using (var cmd = new SqlCommand(ReadBranchesItems, conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(exclusionTable);
                }
            }


            foreach (DataRow row in dataTable.Rows)
            {
                var vc = new MSCommerceCatalogCollection2VirtualCatalog() { name = row.GetString("BranchId"), startDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), endDate = DateTime.Now.AddYears(500).ToString("yyyy-MM-ddTHH:mm:ss"), languages = Language, DefaultLanguage = Language, ReportingLanguage = Language };
                vc.DisplayName = CreateDisplayName(row.GetString("Description"));

                //Include the base catalog
                vc.IncludeRule = new List<MSCommerceCatalogCollection2VirtualCatalogIncludeRule> { new MSCommerceCatalogCollection2VirtualCatalogIncludeRule() { baseCatalog = Configuration.BaseCatalog } }.ToArray();


                var rules = exclusionTable.AsEnumerable().Where(e => e.Field<string>("BranchId") == row.GetString("BranchId"));
                var excludeRules = new List<MSCommerceCatalogCollection2VirtualCatalogExcludeRule>();
                foreach (var exRow in rules)
                    try
                    {
                        //Exlude the specific products that don't belong to this virtual catalog
                        excludeRules.Add(new MSCommerceCatalogCollection2VirtualCatalogExcludeRule() { baseCatalog = Configuration.BaseCatalog, ProductId = exRow.GetString("ItemId") });
                    }
                    catch { }

                vc.ExcludeRule = excludeRules.ToArray();
                virtualCatalogs.Add(vc);
            }

            return virtualCatalogs.ToArray();

        }
        
        private MSCommerceCatalogCollection2CatalogProduct[] GenerateProducts(string branchId)
        {
            var itemTable = new DataTable();
            var products = new List<MSCommerceCatalogCollection2CatalogProduct>();

            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                using (var cmd = new SqlCommand(string.Format(ReadItems_IncludeBranch, branchId), conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(itemTable);
                }
            }

            foreach (DataRow row in itemTable.Rows)
            {
                var newProd = new MSCommerceCatalogCollection2CatalogProduct() { ProductId = row.GetString("ItemId"), Definition = "Item" };
                newProd.DisplayName = new DisplayName[1] { new DisplayName() { language = "en-US", Value = row.GetString("Name") } };
                newProd.ParentCategory = new ParentCategory[1] { new ParentCategory() { Value = row.GetString("CategoryId"), rank = "0" } };
                products.Add(newProd);
            }

            return products.ToArray();
        }

        private MSCommerceCatalogCollection2CatalogCategory[] GenerateCategories()
        {
            var dataTable = new DataTable();
            var childTable = new DataTable();
            List<MSCommerceCatalogCollection2CatalogCategory> categories = new List<MSCommerceCatalogCollection2CatalogCategory>();

            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                using (var cmd = new SqlCommand(ReadParentCategories, conn))
                {
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }

                using (var cmd = new SqlCommand(ReadSubCategories, conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(childTable);
                }
            }

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
        #endregion

        #region SQL
        private const string ReadBranchesItems = " SELECT  DISTINCT " +
                                            " 	LTRIM(RTRIM(b.BranchId)) as BranchId, " +
                                            "       i.[ItemId] " +
                                            " FROM " +
                                            " 	ETL.Staging_ItemData i cross join " +
                                            " 	ETL.Staging_Branch b  " +
                                            " WHERE " +
                                            " 	NOT EXISTS (SELECT TOP 1 ItemId FROM ETL.Staging_ItemData WHERE ItemId = i.ItemId AND BranchId = b.BranchID) " +
                                            " order by " +
                                            " 	i.ItemId; ";

        private const string ReadBranches = "SELECT * FROM [ETL].Staging_Branch WHERE LocationTypeId=3";

        private const string ReadItems_IncludeBranch = " SELECT DISTINCT " +
                                                    "       i.[ItemId] " +
                                                    "       ,ETL.initcap([Name]) as Name " +
                                                    "       ,ETL.initcap([Description]) as Description " +
                                                    "       ,ETL.initcap([Brand]) as Brand " +
                                                    "       ,[Pack] " +
                                                    "       ,[Size] " +
                                                    "       ,[UPC] " +
                                                    "       ,[MfrNumber] " +
                                                    "       ,ETL.initcap([MfrName]) as MfrName " +
                                                    "       ,i.CategoryId " +
                                                    "   FROM [ETL].[Staging_ItemData] i inner join " +
                                                    "   ETL.Staging_Category c on i.CategoryId = c.CategoryId " +
                                                    " WHERE " +
                                                    "   i.BranchId = '{0}' " +
                                                    " Order by i.[ItemId] ";

        private const string ReadParentCategories = "SELECT CategoryId, [ETL].initcap(CategoryName) as CategoryName, PPICode FROM [ETL].Staging_Category WHERE CategoryId like '%000'";
        private const string ReadSubCategories = "SELECT CategoryId, [ETL].initcap(CategoryName) as CategoryName, PPICode FROM [ETL].Staging_Category WHERE CategoryId not like '%000' AND CategoryId <> 'AA001 '";
        #endregion        
    }
}
