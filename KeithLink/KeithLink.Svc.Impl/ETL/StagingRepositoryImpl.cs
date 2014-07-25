using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Impl.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.ETL
{
    public class StagingRepositoryImpl: IStagingRepository
    {
        public DataTable ReadAllBranches()
        {
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand("[ETL].[ReadBranches]", conn))
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

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[ReadItemsByBranch]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var paramBranchID = cmd.Parameters.Add("branchId", SqlDbType.VarChar);
                    paramBranchID.Direction = ParameterDirection.Input;
                    paramBranchID.Value = branchId;

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

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[ReadSubCategories]", conn))
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

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[ReadParentCategories]", conn))
                {
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public DataTable ReadFullItemForElasticSearch()
        {
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand("[ETL].[ReadFullItemData]", conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }
            }
            return dataTable;
        }
               
    }
}
