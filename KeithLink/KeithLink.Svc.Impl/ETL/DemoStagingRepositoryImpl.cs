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
	public class DemoStagingRepositoryImpl: IStagingRepository
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
			//Two possible ways for this to work. 
			//1. The script below can be executed and the system can populate customers as usual, which will contain the scrubed customer names. This could also be be made into a Stored Procedure
			//2. This can simply return an empty DataTable, and a seperate script will have to be manly executed to populate customer info. 
			//Option 1 would be the preferred method, as it would be automated and use existing code, but the SQL users will have to be granted
			//readonly access into KPay in order to access the invoices table.
			//For right now this will use option 2


			return new DataTable();

			/*
			var demoCustomerScript = "select " +
									"	ec.CO AS BranchNumber, " +
									"	ec.CustomerNumber, " +
									"	CASE WHEN a.DsrName IS NULL THEN 'Customer ' + CAst(a.Rank as nvarchar) ELSE SUBSTRING(a.DsrName, 0, CHARINDEX(' ', a.DsrName) ) + ' ' + CAst(a.Rank as nvarchar) END as CustomerName, " +
									"	ec.CustomerName, " +
									"	ec.Address1, " +
									"	ec.Address2, " +
									"	ec.City, " +
									"	ec.[State], " +
									"	ec.ZipCode, " +
									"	ec.Telephone, " +
									"	ec.SalesRep as DsrNumber, " +
									"	ec.ChainStoreCode as NationalOrRegionalAccountNumber, " +
									"	ec.[Contract] as ContractNumber, " +
									"	ec.PORequiredFlag, " +
									"	ec.PowerMenu, " +
									"	ec.ContractOnly, " +
									"	ec.TermCode, " +
									"	ec.CreditLimit, " +
									"	ec.CreditHoldFlag, " +
									"	ec.DateOfLastPayment, " +
									"	ec.AmountDue, " +
									"	ec.CurrentBalance, " +
									"	ec.PDACXAge1 BalanceAge1, " +
									"	ec.PDACXAge2 BalanceAge2, " +
									"	ec.PDACXAge3 BalanceAge3, " +
									"	ec.PDACXAge4 BalanceAge4, " +
									"	ec.AchType, " +
									"	ec.DsmNumber " +
									"	from ( " +
									"       select rank() over (PARTITION by d.BranchId, d.dsrnumber ORDER BY inv.amountdue desc) as [Rank], " +
									"       inv.BranchId as BranchId, " +
									"       d.DsrNumber as DsrNumber,  " +
									"       d.Name as DsrName, " +
									"       inv.customernumber, " +
									"       c.CustomerName, " +
									"       inv.amountdue as amount " +
									"       from (select left(division, 3) as BranchId, " +
									"                                  customernumber,  " +
									"                                  sum(amountdue) as amountdue " +
									"              from  kpay.dbo.invoice i " +
									"                       where i.itemsequence = 0 " +
									"              and    i.invoicedate > dateadd(m,-6,getdate()) " +
									"              group by i.Division,  i.customernumber) inv  " +
									"       left outer join etl.Staging_Customer c " +
									"              on (inv.BranchId = c.CO " +
									"                       and inv.customernumber = right(c.customernumber,6)) " +
									"       left outer join BranchSupport.Dsrs d " +
									"              on (c.co = d.BranchId " +
									"                       and c.SalesRep =  d.DsrNumber) " +
									"          where c.ActiveFlag = 'A' " +
									"          and inv.amountdue > 0 " +
									"       group by inv.BranchId, d.BranchId, d.DsrNumber, d.Name, c.CustomerName ,inv.customernumber,inv.amountdue " +
									") a inner join etl.Staging_Customer ec on a.customernumber = LTRIM(RTRIM(ec.CustomerNumber)) AND a.BranchId = ec.CO " +
									"where a.[Rank] <=3 " +
									"order by BranchId, DsrNumber ";


			return PopulateDataTable(demoCustomerScript);
			 */
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


		public DataTable ReadDsrImages()
		{
			return PopulateDataTable("[ETL].[ReadDsrImage]");
		}
	}
}
