﻿// KeithLink
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Impl.Models;
using System.IO;
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
            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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

        /// <summary>
        /// ReadSP customer item history
        /// </summary>
        /// <returns></returns>
        public void ProcessItemHistoryData(int numWeeks) {
            using (SqlConnection c = new SqlConnection(Configuration.BEKDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[ETL].[ProcessItemHistoryData]", c))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("NumWeeks", numWeeks);
                    cmd.CommandTimeout = 0;
                    c.Open();
                    cmd.ExecuteNonQuery();
                }
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
            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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

            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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
			using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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
                using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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
        /// Truncate the internal user access table
        /// </summary>
        public void PurgeInternalUserAccessTable() {
            try
            {
                using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
                {
                    using (var cmd = new SqlCommand("[ETL].[PurgeInternalUserAccess]", conn))
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
                eventLog.WriteErrorLog("Error purging internal user access table", ex);
            }
        }

        /// <summary>
        /// Import customers and addresses to CS
        /// </summary>
        public void ImportCustomersToCS()
		{
            try
            {
                using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
                {
                    using (var cmd = new SqlCommand("[ETL].[LoadOrgsAndAddressesToCS]", conn))
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
                eventLog.WriteErrorLog("Error importing organizations to CS", ex);
            }

        }

        /// <summary>
        /// ReadSP all branches
        /// </summary>
        /// <returns></returns>
        public DataTable ReadAllBranches()
        {
            return PopulateDataTable("[ETL].[ReadBranches]");
        }

        public DataTable ReadAllItemKeywords() {
            return PopulateDataTable("[ETL].[ReadAllItemKeywords]");
        }

        /// <summary>
        /// ReadSP brand control labels
        /// </summary>
        /// <returns></returns>
        public DataTable ReadBrandControlLabels()
        {
            return PopulateDataTable("[ETL].[ReadBrandControlLabels]");
        }

        /// <summary>
        /// ReadSP contract items
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <param name="divisionName"></param>
        /// <param name="contractNumber"></param>
        /// <returns></returns>
        public DataTable ReadContractItems(string customerNumber, string divisionName, string contractNumber)
        {
            var contractItems = new DataTable();

            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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
        /// ReadSP commerce server users
        /// </summary>
        /// <returns></returns>
        public DataTable ReadCSUsers()
        {
            return PopulateDataTable("[ETL].[ReadCSUsers]");
        }

        /// <summary>
        /// ReadSP customers
        /// </summary>
        /// <returns></returns>
        public DataTable ReadCustomers()
        {
            return PopulateDataTable("[ETL].[ReadCustomers]");
        }

        /// <summary>
        /// ReadSP Categories from Department Table
        /// </summary>
        /// <returns></returns>
        public DataTable ReadDepartmentCategories()
        {
            return PopulateDataTable("[ETL].[ReadDepartments]");
        }

        /// <summary>
        /// ReadSP DSR Images
        /// </summary>
        /// <returns></returns>
        public DataTable ReadDsrImages() {
            return PopulateDataTable( "[ETL].[ReadDsrImage]" );
        }

        /// <summary>
        /// ReadSP DSR info
        /// </summary>
        /// <returns></returns>
        public DataTable ReadDsrInfo()
        {
            return PopulateDataTable("[ETL].[ReadDsrInfo]");
        }

        /// <summary>
        /// ReadSP full item for ElasticSearch
        /// </summary>
        /// <param name="branchId">the specific branch to load</param>
        /// <returns></returns>
        public DataTable ReadFullItemForElasticSearch(string branchId)
        {
            return PopulateDataTable("[ETL].[ReadFullItemData]", 
                                     new List<SqlParameter>() { new SqlParameter("@BranchId", branchId) });
        }

        /// <summary>
        /// ReadSP nutritional information for items
        /// </summary>
        /// <returns></returns>
        public DataSet ReadGSDataForItems()
        {
            var gsData = new DataSet();

            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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

        public DataTable ReadPDMDataForItems() {
            return PopulateDataTable("[ETL].[ReadPDMData]");
        }

        /// <summary>
        /// ReadSP items
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public DataTable ReadItems(string branchId)
        {
            var itemTable = new DataTable();

            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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

		public DataTable ReadUNFIItems(string warehouse)
		{
			var itemTable = new DataTable();

			using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
			{
				using (var cmd = new SqlCommand("[ETL].[ReadUNFItemsByWarehouse]", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					var paramBranchID = cmd.Parameters.Add("warehouse", SqlDbType.VarChar);
					paramBranchID.Direction = ParameterDirection.Input;
					paramBranchID.Value = warehouse;

					cmd.CommandTimeout = 0;
					conn.Open();
					var da = new SqlDataAdapter(cmd);
					da.Fill(itemTable);
				}
			}
			return itemTable;
		}


        /// <summary>
        /// ReadSP parent categories
        /// </summary>
        /// <returns></returns>
        public DataTable ReadParentCategories()
        {
            return PopulateDataTable("[ETL].[ReadParentCategories]");
        }

        

        /// <summary>
        /// ReadSP proprietary items
        /// </summary>
        /// <returns></returns>
		public DataTable ReadProprietaryItems()
		{
			return PopulateDataTable("[ETL].[ReadProprietaryItems]");
		}


        /// <summary>
        /// ReadSP sub categories
        /// </summary>
        /// <returns></returns>
        public DataTable ReadSubCategories()
        {
            return PopulateDataTable("[ETL].[ReadSubCategories]");
        }

		/// <summary>
		/// ReadSP parent categories
		/// </summary>
		/// <returns></returns>
		public DataTable ReadUnfiCategories()
		{
			return PopulateDataTable("[ETL].[ReadUNFICategories]");
		}

		/// <summary>
		/// ReadSP proprietary items
		/// </summary>
		/// <returns></returns>
		public DataTable ReadUnfiSubCategories()
		{
			return PopulateDataTable("[ETL].[ReadUNFISubCategories]");
		}

        /// <summary>
        /// ReadSP worksheet items
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <param name="divisionName"></param>
        /// <returns></returns>
        public DataTable ReadWorksheetItems(string customerNumber, string divisionName)
        {
            var worksheetItems = new DataTable();

            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
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

        /// <summary>
        /// Helper function to populate data tables
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteProfileObjectQueryReturn(string query)
        {
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(Configuration.CSProfileDbConnection))
            {
                conn.Open();

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = 0;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public bool ExecuteProfileObjectQuery(string query)
        {
            try
            {
                using (var conn = new SqlConnection(Configuration.CSProfileDbConnection))
                {
                    using (var cmd = new SqlCommand(query.ToString(), conn))
                    {
                        //File.WriteAllText("c:\\query.txt", query.ToString()); //for debugging
                        cmd.CommandTimeout = 0;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                eventLog.WriteErrorLog(String.Format("Etl:  Error updating profile object. {0} {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

		public DataTable ReadUNFIItems()
		{
			return PopulateDataTable("[ETL].[ReadUNFIProducts]");
		}

		public List<string> ReadDistinctUNFIWarehouses()
		{
			var returnList = new List<string>();
			try
			{
				using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
				{
					using (var cmd = new SqlCommand("[ETL].[ReadUNFIDistinctWarehouses]", conn))
					{
						cmd.CommandTimeout = 0;
						conn.Open();

						var reader = cmd.ExecuteReader();
						while (reader.Read())
						{
							returnList.Add(reader[0].ToString());
						}
					}
				}
				return returnList;
			}
			catch (Exception ex)
			{
				eventLog.WriteErrorLog(String.Format("Etl:  Error reading distinct UNFI warehouses. {0} {1}", ex.Message, ex.StackTrace));
				return returnList;
			}
		}

        /// <summary>
        /// ReadSP parent categories
        /// </summary>
        /// <returns></returns>
        public DataTable ReadUnfiEastCategories()
        {
            return PopulateDataTable("[ETL].[ReadUNFIEastCategories]");
        }

        /// <summary>
        /// ReadSP proprietary items
        /// </summary>
        /// <returns></returns>
        public DataTable ReadUnfiEastSubCategories()
        {
            return PopulateDataTable("[ETL].[ReadUNFIEastSubCategories]");
        }

        public DataTable ReadUNFIEastItems(string warehouse)
        {
            var itemTable = new DataTable();

            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[ReadUNFIEastItemsByWarehouse]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var paramBranchID = cmd.Parameters.Add("warehouse", SqlDbType.VarChar);
                    paramBranchID.Direction = ParameterDirection.Input;
                    paramBranchID.Value = warehouse;

                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(itemTable);
                }
            }
            return itemTable;
        }

        public List<string> ReadDistinctUNFIEastWarehouses()
        {
            var returnList = new List<string>();
            try
            {
                using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
                {
                    using (var cmd = new SqlCommand("[ETL].[ReadUNFIEastDistinctWarehouses]", conn))
                    {
                        cmd.CommandTimeout = 0;
                        conn.Open();

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            returnList.Add(reader[0].ToString());
                        }
                    }
                }
                return returnList;
            }
            catch (Exception ex)
            {
                eventLog.WriteErrorLog(String.Format("Etl:  Error reading distinct UNFI warehouses. {0} {1}", ex.Message, ex.StackTrace));
                return returnList;
            }
        }

        public DataTable ReadUNFIEastItems()
        {
            return PopulateDataTable("[ETL].[ReadUNFIEastProducts]");
        }
        #endregion



    }
}
