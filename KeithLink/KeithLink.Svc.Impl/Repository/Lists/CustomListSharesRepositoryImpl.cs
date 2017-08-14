using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class CustomListSharesRepositoryImpl : DapperDatabaseConnection, ICustomListSharesRepository
    {
        #region attributes

        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_RETVAL = "ReturnValue";

        private const string SPNAME_DELETE = "[List].[DeleteCustomListShare]";
        private const string SPNAME_GETBYCUST = "[List].[GetCustomListSharesByCustomerNumberBranch]";
        private const string SPNAME_GETBYHEADER = "[List].[GetCustomListSharesByListId]";
        private const string SPNAME_GETONE = "[List].[GetCustomListShare]";
        private const string SPNAME_SAVE = "[List].[SaveCustomListShare]";
        #endregion

        #region constructor
        public CustomListSharesRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods

        public void DeleteCustomListShares(long id) {
            ExecuteCommand(SPNAME_DELETE, PARMNAME_ID, id);
        }

        public CustomListShare GetCustomListShare(long id) {
            return ReadOne<CustomListShare>(SPNAME_GETONE, PARMNAME_ID, id);
        }

        public List<CustomListShare> GetCustomListSharesByCustomer(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return Read<CustomListShare>(SPNAME_GETBYCUST, parms);
        }

        public List<CustomListShare> GetCustomListSharesByHeaderId(long parentId) {
            return Read<CustomListShare>(SPNAME_GETBYHEADER, PARMNAME_HEADERID, parentId);
        }

        public long SaveCustomListShare(CustomListShare model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, model.BranchId);
            parms.Add(PARMNAME_CUSTNUM, model.CustomerNumber);
            parms.Add(PARMNAME_HEADERID, model.HeaderId);
            parms.Add(PARMNAME_RETVAL, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETVAL);
        }

        #endregion
    }
}
