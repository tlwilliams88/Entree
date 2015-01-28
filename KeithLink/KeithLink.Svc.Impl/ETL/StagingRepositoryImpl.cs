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
            return PopulateDataTable("[ETL].[ReadBranches]");
        }

        public DataTable ReadBrandControlLabels()
        {
            return PopulateDataTable("[ETL].[ReadBrandControlLabels]");
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
            return PopulateDataTable("[ETL].[ReadSubCategories]");
        }

        public DataTable ReadParentCategories()
        {
            return PopulateDataTable("[ETL].[ReadParentCategories]");
        }

        public DataTable ReadFullItemForElasticSearch()
        {
            return PopulateDataTable("[ETL].[ReadFullItemData]");
        }

        private DataTable PopulateDataTable(string sql)
        {
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public DataSet ReadGSDataForItems()
        {
            var gsData = new DataSet();

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[ReadItemGS1Data]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(gsData);
                }
            }
            return gsData;
        }


		public DataTable ReadProprietaryItems()
		{
			return PopulateDataTable("[ETL].[ReadProprietaryItems]");
		}
        public DataTable ReadCustomers()
        {
            return PopulateDataTable("[ETL].[ReadCustomers]");
        }

        public DataTable ReadCSUsers()
        {
            return PopulateDataTable("[ETL].[ReadCSUsers]");
        }
		
		public void ProcessInvoices()
		{
			using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
			{
				using (var cmd = new SqlCommand("[ETL].[ProcessStagedInvoices]", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 0;
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

        public DataTable ReadContractItems(string customerNumber, string divisionName, string contractNumber)
        {
            var contractItems = new DataTable();

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[ReadContractItems]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var paramCustomerNumber = cmd.Parameters.Add("CustomerNumber", SqlDbType.VarChar);
                    var paramDivisionName = cmd.Parameters.Add("DivisionName", SqlDbType.VarChar);
                    var paramContractNumber = cmd.Parameters.Add("ContractNumber", SqlDbType.VarChar);

                    paramCustomerNumber.Direction = ParameterDirection.Input;
                    paramDivisionName.Direction = ParameterDirection.Input;
                    paramContractNumber.Direction = ParameterDirection.Input;

                    paramCustomerNumber.Value = customerNumber;
                    paramDivisionName.Value = divisionName;
                    paramContractNumber.Value = contractNumber;

                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(contractItems);
                }
            }

            return contractItems;
        }
        public DataTable ReadWorksheetItems(string customerNumber, string divisionName)
        {
            var worksheetItems = new DataTable();

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[ReadWorksheetItems]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var paramCustomerNumber = cmd.Parameters.Add("CustomerNumber", SqlDbType.VarChar);
                    var paramDivisionName = cmd.Parameters.Add("DivisionName", SqlDbType.VarChar);

                    paramCustomerNumber.Direction = ParameterDirection.Input;
                    paramDivisionName.Direction = ParameterDirection.Input;

                    paramCustomerNumber.Value = customerNumber;
                    paramDivisionName.Value = divisionName;

                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(worksheetItems);
                }
            }

            return worksheetItems;
        }

        public DataTable ReadDsrInfo()
        {
            return PopulateDataTable("[ETL].[ReadDsrInfo]");
        }


    }
}
