using KeithLink.Svc.Core.ETL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.ETL
{
    public class StagingRepositoryImpl: IStagingRepository
    {
        public DataTable ReadAllBranches()
        {
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(SQL_ReadBranches, conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public DataTable ReadItems(string branchId)
        {
            var itemTable = new DataTable();

            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                using (var cmd = new SqlCommand(string.Format(SQL_ReadItems_IncludeBranch, branchId), conn))
                {
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(itemTable);
                }
            }
            return itemTable;
        }

        public DataTable ReadSubCategories()
        {
            var childTable = new DataTable();

            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                using (var cmd = new SqlCommand(SQL_ReadSubCategories, conn))
                {
                    conn.Open();

                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(childTable);
                }
            }
            return childTable;
        }

        public DataTable ReadParentCategories()
        {
            var dataTable = new DataTable();

            using (var conn = new SqlConnection(Configuration.StagingConnectionString))
            {
                using (var cmd = new SqlCommand(SQL_ReadParentCategories, conn))
                {
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }
            }
            return dataTable;
        }

        #region SQL
        private const string SQL_ReadBranchesItems = " SELECT  DISTINCT " +
                                            " 	LTRIM(RTRIM(b.BranchId)) as BranchId, " +
                                            "       i.[ItemId] " +
                                            " FROM " +
                                            " 	ETL.Staging_ItemData i cross join " +
                                            " 	ETL.Staging_Branch b  " +
                                            " WHERE " +
                                            " 	NOT EXISTS (SELECT TOP 1 ItemId FROM ETL.Staging_ItemData WHERE ItemId = i.ItemId AND BranchId = b.BranchID) " +
                                            " order by " +
                                            " 	i.ItemId; ";

        private const string SQL_ReadBranches = "SELECT * FROM [ETL].Staging_Branch WHERE LocationTypeId=3";

        private const string SQL_ReadItems_IncludeBranch = " SELECT DISTINCT " +
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

        private const string SQL_ReadParentCategories = "SELECT CategoryId, [ETL].initcap(CategoryName) as CategoryName, PPICode FROM [ETL].Staging_Category WHERE CategoryId like '%000'";
        private const string SQL_ReadSubCategories = "SELECT CategoryId, [ETL].initcap(CategoryName) as CategoryName, PPICode FROM [ETL].Staging_Category WHERE CategoryId not like '%000'";
        #endregion        
    }
}
