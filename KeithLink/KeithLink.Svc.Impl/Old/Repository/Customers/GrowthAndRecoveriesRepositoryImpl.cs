using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Interface.Customers;
using Entree.Core.Interface.DataConnection;
using Entree.Core.Models.Customers;

namespace KeithLink.Svc.Impl.Repository.Customers
{
    public class GrowthAndRecoveriesRepositoryImpl : IGrowthAndRecoveriesRepository {

        private IDapperDatabaseConnection connection;
        public GrowthAndRecoveriesRepositoryImpl(IDapperDatabaseConnection dbConnection) {
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
	                pg.Code as GroupingCode,
                    pg.Description as GroupingDescription,
                    g.ProductGroupingInsightKey,
                    g.CustomerInsightVersionKey
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
