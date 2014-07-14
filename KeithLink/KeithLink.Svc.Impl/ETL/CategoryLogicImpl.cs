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

namespace KeithLink.Svc.Impl.ETL
{
    public class CategoryLogicImpl: ICategoryLogic
    {
        public void ProcessStagedCategories()
        {
            var dataTable = new DataTable();
            var childTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT CategoryId, [ETL].initcap(CategoryName) as CategoryName, PPICode FROM [ETL].Staging_Category WHERE CategoryId like '%000'", conn))
                {
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }

                using (var cmd = new SqlCommand("SELECT CategoryId, [ETL].initcap(CategoryName) as CategoryName, PPICode FROM [ETL].Staging_Category WHERE CategoryId not like '%000' AND CategoryId <> 'AA001 '", conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(childTable);
                } 
            }

            //Refactor once the initial Core project is checked in
            CatalogSiteAgent catalogSiteAgent = new CatalogSiteAgent();
            catalogSiteAgent.SiteName = Configuration.CSSiteName;
            catalogSiteAgent.IgnoreInventorySystem = false;
            
            CatalogContext context = CatalogContext.Create(catalogSiteAgent);

            
            var baseCatalog = context.GetCatalog(Configuration.BaseCatalog);
            var currentCatagories = GetFullCategoryList(context); //Get Current Categories
            
            
            foreach (DataRow row in dataTable.Rows)
            {
                if (!currentCatagories.Where(c => c.Name.Equals(row.GetString("CategoryId", true))).Any())
                {
                    var newCat = baseCatalog.CreateCategory("Category", row.GetString("CategoryId", trim: true), true, null, row.GetString("CategoryName", true));
                    newCat["PPICode"] = row.GetString("PPICode", true);
                    newCat.Save();
                }
            }

            foreach (DataRow row in childTable.Rows)
            {

                if (!currentCatagories.Where(c => c.Name.Equals(row.GetString("CategoryId", true))).Any())
                    try
                    {
                        var newCat = baseCatalog.CreateCategory("Category", row.GetString("CategoryId", trim: true), true, string.Format("{0}000", row.GetString("CategoryId", true).Substring(0, 2)), row.GetString("CategoryName", true));
                        newCat["PPICode"] = row.GetString("PPICode", true);
                        newCat.Save();
                    }
                    catch{} //TODO: Log/handle error
            }


        }
        
        public void ProcessStagedItems()
        {
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                using (var cmd = new SqlCommand(" SELECT DISTINCT top 100 " +
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
                                                    " Order by i.[ItemId] "
                                                    , conn))
                {
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }                
            }


            //Refactor once the initial Core project is checked in
            CatalogSiteAgent catalogSiteAgent = new CatalogSiteAgent();
            catalogSiteAgent.SiteName = Configuration.CSSiteName;
            catalogSiteAgent.IgnoreInventorySystem = false;

            CatalogContext context = CatalogContext.Create(catalogSiteAgent);
            var currentCategories = GetFullCategoryList(context);
            var baseCatalog = context.GetCatalog(Configuration.BaseCatalog);
            
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    var cat = currentCategories.Where(c => c.Name.Equals(row.GetString("CategoryId"))).FirstOrDefault();
                    
                    if (cat == null)
                    {
                        var existingProduct = baseCatalog.GetRootCategory().ChildProducts.Where(e => e.ProductId.Equals(row.GetString("ItemId"))).FirstOrDefault() ;
                        

                        if (existingProduct == null)
                            SetProductCustomProperties(((BaseCatalog)baseCatalog).CreateProduct("Item", row.GetString("ItemId"), 1, null, row.GetString("Name")), row);
                        else
                            UpdateProduct(existingProduct, row);
                        
                    }
                    else
                    {
                        var existingProduct = cat.ChildProducts.Where(p => p.ProductId.Equals(row.GetString("ItemId"))).FirstOrDefault();
                        
                        if (existingProduct == null)
                            SetProductCustomProperties(((BaseCatalog)baseCatalog).CreateProduct("Item", row.GetString("ItemId"), 1, row.GetString("CategoryId"), row.GetString("Name")), row);
                        else
                            UpdateProduct(existingProduct, row);
                    }
                }
                catch { } //TODO: Log/handle exception
            }

        }

        public void ProcessStagedBranches()
        {
            var dataTable = new DataTable();
            var exclusionTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                conn.Open();
                    
                using (var cmd = new SqlCommand("SELECT * FROM [ETL].Staging_Branch WHERE LocationTypeId=3", conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }

                using (var cmd = new SqlCommand(" SELECT  DISTINCT TOP 100 " +
                                            " 	LTRIM(RTRIM(b.BranchId)) as BranchId, " +
                                            "       i.[ItemId] " +
                                            " FROM " +
                                            " 	ETL.Staging_ItemData i cross join " +
                                            " 	ETL.Staging_Branch b  " +
                                            " WHERE " +
                                            " 	NOT EXISTS (SELECT TOP 1 ItemId FROM ETL.Staging_ItemData WHERE ItemId = i.ItemId AND BranchId = b.BranchID) " +
                                            " order by " +
                                            " 	i.ItemId; ", conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(exclusionTable);
                }
            }

            //Refactor once the initial Core project is checked in
            CatalogSiteAgent catalogSiteAgent = new CatalogSiteAgent();
            catalogSiteAgent.SiteName = Configuration.CSSiteName;
            catalogSiteAgent.IgnoreInventorySystem = false;

            CatalogContext context = CatalogContext.Create(catalogSiteAgent);
            var currentCategories = GetFullCategoryList(context);
            var baseCatalog = context.GetCatalog(Configuration.BaseCatalog);

            foreach (DataRow row in dataTable.Rows)
            {
                var vc = context.CreateVirtualCatalog(row.GetString("BranchId"), "en-US", null);
                
                vc.AddVirtualCatalogRule(Configuration.BaseCatalog, null, null, null, false);

                var rules = exclusionTable.AsEnumerable().Where(e => e.Field<string>("BranchId") == row.GetString("BranchId"));

                foreach (var exRow in rules)
                    try
                    {
                        vc.AddVirtualCatalogRule(Configuration.BaseCatalog, null, exRow.GetString("ItemId"), null, true);
                    }
                    catch { }
                vc.Save();
                vc.Rebuild(true);
            }
        }

        #region Helper Methods
        private void UpdateProduct(Product existingProduct, DataRow row)
        {
            var existing = new KeithLink.Svc.Core.Product(){
            
                ProductId = existingProduct.ProductId,
                Name = existingProduct.DisplayName,
                Description = existingProduct["Description"] == null ? null : existingProduct["Description"].ToString(),
                Brand = existingProduct["Brand"].ToString() == null ? null : existingProduct["Brand"].ToString(),
                UPC = existingProduct["UPC"].ToString() == null ? null : existingProduct["UPC"].ToString(),
                Pack = existingProduct["Pack"].ToString() == null ? null : existingProduct["Pack"].ToString(),
                Size = existingProduct["Size"].ToString() == null ? null : existingProduct["Size"].ToString(),
                MfrName = existingProduct["MfrName"].ToString() == null ? null : existingProduct["MfrName"].ToString(),
                MfrNumber = existingProduct["MfrNumber"].ToString() == null ? null : existingProduct["MfrNumber"].ToString()
        };

            var current = new KeithLink.Svc.Core.Product()
            {
                ProductId = row.GetString("ItemId"),
                Name = row.GetString("Name"),
                Description = row.GetString("Description"),
                Brand = row.GetString("Brand"),
                UPC = row.GetString("UPC"),
                Pack = row.GetString("Pack"),
                Size = row.GetString("Size"),
                MfrName = row.GetString("MfrName"),
                MfrNumber = row.GetString("MfrNumber")
            };

            if (Crypto.CalculateMD5Hash(existing) != Crypto.CalculateMD5Hash(current))
                SetProductCustomProperties(existingProduct, row);
        }

        private void SetProductCustomProperties(Product prod, DataRow row)
        {
            prod["UPC"] = row.GetString("UPC");
            prod["Description"] = row.GetString("Description");
            prod["Brand"] = row.GetString("Brand");
            prod["Pack"] = row.GetString("Pack");
            prod["Size"] = row.GetString("Size");
            prod["MfrNumber"] = row.GetString("MfrNumber");
            prod["MfrName"] = row.GetString("MfrName");
            prod.Save();     
        }
        
        private List<Category> GetFullCategoryList(CatalogContext context)
        {
            var currentCatagories = context.GetCategory(Configuration.BaseCatalog, null).ChildCategories.ToList(); //Get Current Categories



            List<Category> subCategories = new List<Category>();
            foreach (var cat in currentCatagories)
            {
                foreach (var sub in cat.ChildCategories)
                    subCategories.Add(sub);
            }

            currentCatagories.AddRange(subCategories);

            return currentCatagories;
        }
        #endregion

    }

    

}
