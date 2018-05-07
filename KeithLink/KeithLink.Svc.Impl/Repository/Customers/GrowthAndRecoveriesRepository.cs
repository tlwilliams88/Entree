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
    public class GrowthAndRecoveriesRepository : IGrowthAndRecoveriesRepository {

        private IDapperDatabaseConnection connection;
        public GrowthAndRecoveriesRepository(IDapperDatabaseConnection dbConnection) {
            connection = dbConnection;
        }

        public List<GrowthAndRecoveriesModel> GetGrowthAdnGetGrowthAndRecoveryOpportunities(string customerNumber, string branchId) {
            List<GrowthAndRecoveriesModel> results = connection.Query<GrowthAndRecoveriesModel>(@"
                SELECT
	                g.Id,
	                g.BranchId,
	                g.CustomerNumber,
	                g.Amount,
	                g.GrowthAndRecoveryProductGroup,
	                g.GrowthAndRecoveryTypeKey,
	                pg.Code,
                    pg.Description
                FROM Customers.GrowthAndRecoveries g
                LEFT JOIN Customers.GrowthAndRecoveryProductGroups pg ON pg.GrowthAndRecoveryProductGroup = g.GrowthAndRecoveryProductGroup
                WHERE 
	                g.CustomerNumber = @CustomerNumber
                AND	g.BranchId = @BranchId
                ORDER BY g.Amount DESC",
                new {
                    CustomerNumber = customerNumber,
                    BranchId = branchId
                }).ToList();

            return results;
        }
    }
}
