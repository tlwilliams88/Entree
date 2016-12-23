﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ETL
{
    public interface IStagingRepository
    {
        void PurgeInternalUserAccessTable();
        DataTable ReadAllBranches();
        DataTable ReadBrandControlLabels();
        DataTable ReadItems(string branchId);
        DataTable ReadSubCategories();
        DataTable ReadParentCategories();
		DataTable ReadUnfiCategories();
		DataTable ReadUnfiSubCategories();
        DataTable ReadFullItemForElasticSearch();
        DataSet ReadGSDataForItems();
        DataTable ReadPDMDataForItems();
        DataTable ReadProprietaryItems();
        DataTable ReadCustomers();
        DataTable ReadCSUsers();
        DataTable ReadContractItems(string customerNumber, string divisionName, string contractNumber);
        DataTable ReadWorksheetItems(string customerNumber, string divisionName);
        DataTable ReadDsrInfo();
        DataTable ReadDsrImages();
        void ProcessContractItems();
		void ProcessInvoices();
        void ProcessItemHistoryData(int numWeeks);
		void ProcessWorksheetItems();
        DataTable ExecuteProfileObjectQueryReturn(string query);
        bool ExecuteProfileObjectQuery(string query);
        void ImportCustomersToCS();

		DataTable ReadUNFIItems();
		List<string> ReadDistinctUNFIWarehouses();
		DataTable ReadUNFIItems(string warehouse);
    }
}
