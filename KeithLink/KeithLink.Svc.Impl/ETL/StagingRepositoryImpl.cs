﻿using KeithLink.Svc.Core.ETL;
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

        public DataTable ReadUniqueUsers()
        {
            //hard coded until user/customer data structure is defined
            
            //Code to use for sql calls
            //return PopulateDataTable("[ETL].[usp_ECOM_SelectCSUsers]");
            
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            DataRow rowTwo;

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "UserId";
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Email";
            table.Columns.Add(column);

            row = table.NewRow();
            row["UserId"] = "{fcbd9217-980f-4030-88c3-9a3e8d459fce}";
            table.Rows.Add(row);

            rowTwo = table.NewRow();
            rowTwo["Email"] = "jason@jason.com";
            table.Rows.Add(rowTwo);

            return table;
        }

        public DataTable ReadContractItems(string CustomerNumber, string DivisionName, string ContractNumber)
        {
            var contractItems = new DataTable();

            using (var conn = new SqlConnection(Configuration.AppDataConnectionString))
            {
                using (var cmd = new SqlCommand("[ETL].[usp_ECOM_SelectContractItems]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var paramCustomerNumber = cmd.Parameters.Add("CustomerNumber", SqlDbType.VarChar);
                    var paramDivisionName = cmd.Parameters.Add("DivisionName", SqlDbType.VarChar);
                    var paramContractNumber = cmd.Parameters.Add("ContractNumber", SqlDbType.VarChar);

                    paramCustomerNumber.Direction = ParameterDirection.Input;
                    paramDivisionName.Direction = ParameterDirection.Input;
                    paramContractNumber.Direction = ParameterDirection.Input;

                    paramCustomerNumber.Value = CustomerNumber;
                    paramDivisionName.Value = DivisionName;
                    paramContractNumber.Value = ContractNumber;

                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(contractItems);
                }
            }

            return contractItems;
        }

    }
}
