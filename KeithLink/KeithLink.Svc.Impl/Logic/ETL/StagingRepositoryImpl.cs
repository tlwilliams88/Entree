// KeithLink
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Impl.Models;

// Core
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.ETL
{
    public class StagingRepositoryImpl: IStagingRepository {
        #region attributes

        private readonly IEventLogRepository eventLog;

        #endregion

        #region constructor

        public StagingRepositoryImpl(IEventLogRepository eventLog)
        {
            this.eventLog = eventLog;
        }

        #endregion

        #region function

        /// <summary>
        /// Process contrac items
        /// </summary>
		public void ProcessContractItems()
		{
            try
            {
                using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
                {
                    using (var cmd = new SqlCommand("[ETL].[ProcessContractItemList]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteErrorLog("Error Processing Contract Lists", ex);
            }
            
		}

        /// <summary>
        /// Read customer item history
        /// </summary>
        /// <returns></returns>
        public void ProcessItemHistoryData(int weeks) {
            try {
                using (SqlConnection c = new SqlConnection( Configuration.AppDataConnectionString )) {
                    using (SqlCommand cmd = new SqlCommand( "[ETL].[ProcessItemHistoryData]", c )) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue( "NumWeeks", weeks );

                        cmd.CommandTimeout = 0;
                        c.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            } catch (Exception ex) {
                eventLog.WriteErrorLog( "Error processing Item History data", ex );
            }
        }

        /// <summary>
        /// Helper function to populate data tables
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Helper function to populate datatable that accepts parameters
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private DataTable PopulateDataTable( string procedure, List<SqlParameter> parameters ) {
            DataTable dataTable = new DataTable();

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand(procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (SqlParameter p in parameters) {
                        cmd.Parameters.Add( p );
                    }

                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Process invoices
        /// </summary>
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

        /// <summary>
        /// Process worksheet items
        /// </summary>
		public void ProcessWorksheetItems()
		{
            try
            {
                using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
                {
                    using (var cmd = new SqlCommand("[ETL].[ProcessWorksheetList]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteErrorLog("Error Processing History Lists", ex);
            }

        }

        /// <summary>
        /// Read all branches
        /// </summary>
        /// <returns></returns>
        public DataTable ReadAllBranches()
        {
            return PopulateDataTable("[ETL].[ReadBranches]");
        }

        /// <summary>
        /// Read brand control labels
        /// </summary>
        /// <returns></returns>
        public DataTable ReadBrandControlLabels()
        {
            return PopulateDataTable("[ETL].[ReadBrandControlLabels]");
        }

        /// <summary>
        /// Read contract items
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <param name="divisionName"></param>
        /// <param name="contractNumber"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Read commerce server users
        /// </summary>
        /// <returns></returns>
        public DataTable ReadCSUsers()
        {
            return PopulateDataTable("[ETL].[ReadCSUsers]");
        }

        /// <summary>
        /// Read customers
        /// </summary>
        /// <returns></returns>
        public DataTable ReadCustomers()
        {
            return PopulateDataTable("[ETL].[ReadCustomers]");
        }

        
        /// <summary>
        /// Read DSR Images
        /// </summary>
        /// <returns></returns>
        public DataTable ReadDsrImages() {
            return PopulateDataTable( "[ETL].[ReadDsrImage]" );
        }

        /// <summary>
        /// Read DSR info
        /// </summary>
        /// <returns></returns>
        public DataTable ReadDsrInfo()
        {
            return PopulateDataTable("[ETL].[ReadDsrInfo]");
        }

        /// <summary>
        /// Read full item for ElasticSearch
        /// </summary>
        /// <returns></returns>
        public DataTable ReadFullItemForElasticSearch()
        {
            return PopulateDataTable("[ETL].[ReadFullItemData]");
        }

        /// <summary>
        /// Read nutritional information for items
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Read items
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Read parent categories
        /// </summary>
        /// <returns></returns>
        public DataTable ReadParentCategories()
        {
            return PopulateDataTable("[ETL].[ReadParentCategories]");
        }

        /// <summary>
        /// Read proprietary items
        /// </summary>
        /// <returns></returns>
		public DataTable ReadProprietaryItems()
		{
			return PopulateDataTable("[ETL].[ReadProprietaryItems]");
		}

        /// <summary>
        /// Read sub categories
        /// </summary>
        /// <returns></returns>
        public DataTable ReadSubCategories()
        {
            return PopulateDataTable("[ETL].[ReadSubCategories]");
        }

        /// <summary>
        /// Read worksheet items
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <param name="divisionName"></param>
        /// <returns></returns>
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

        #endregion
    }
}
