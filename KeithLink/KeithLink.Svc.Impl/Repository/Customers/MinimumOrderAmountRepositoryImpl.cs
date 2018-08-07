using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.DataConnection;
using KeithLink.Svc.Core.Models.Customers;

namespace KeithLink.Svc.Impl.Repository.Customers
{
    public class MinimumOrderAmountRepositoryImpl : IMinimumOrderAmountRepository
    {

        private IDapperDatabaseConnection connection;
        public MinimumOrderAmountRepositoryImpl(IDapperDatabaseConnection dbConnection)
        {
            connection = dbConnection;
        }

        public List<MinimumOrderAmountModel> GetMinimumOrderAmount(string customerNumber, string branchId)
        {
            List<MinimumOrderAmountModel> results = connection.Query<MinimumOrderAmountModel>(@"
                SELECT
                    Value AS ApprovedAmount
                FROM Customers.CustomerOptions
                WHERE 
	                CustomerNumber = @CustomerNumber
                AND	BranchId = @BranchId", new
            {
                CustomerNumber = customerNumber,
                BranchId = branchId
            }).ToList();

            return results;
        }
    }
}
