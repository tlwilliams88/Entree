using KeithLink.Svc.Impl.Repository.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using Dapper;
using System.Data;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ContractChangesRepositoryImpl : DapperDatabaseConnection, IContractChangesRepository
    {
        #region attributes
        private const string STOREDPROC_GET_NEXT = "[List].[ReadNextContractListChange]";
        private const string STOREDPROC_UPDATE_SENT_ON_SET = "[List].[UpdateSentOnContractListChangeByCustomerNumberBranch]";
        private const string STOREDPROC_PURGE = "[List].[DeleteListItemsDelta]";
        #endregion

        #region constructor
        public ContractChangesRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        public List<ContractChange> ReadNextSet()
        {
            return Read<ContractChange>(new CommandDefinition(
                STOREDPROC_GET_NEXT,
                commandType: CommandType.StoredProcedure
            ));
        }

        public void Update(string CustomerNumber, string BranchId, bool Sent)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add("@Sent", Sent);
            parms.Add("@CustomerNumber", CustomerNumber);
            parms.Add("@BranchId", BranchId);

            ExecuteCommand(STOREDPROC_UPDATE_SENT_ON_SET, parms);
        }

        public void Purge(int PurgeDays)
        {
            if (PurgeDays < 0)
            {
                ExecuteCommand(STOREDPROC_PURGE, "@PurgeDays", PurgeDays);
            }
        }
    }
}
