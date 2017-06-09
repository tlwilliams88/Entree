using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Dapper;
using EntityFramework.MappingAPI.Exceptions;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class CustomListHeadersRepositoryImpl : DapperDatabaseConnection, ICustomListHeadersRepository
    {
        #region attributes
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_NAME = "Name";
        private const string PARMNAME_USERID = "UserId";

        private const string SPNAME_GETBYCUST = "[List].[GetCustomListHeadersByCustomerNumberBranch]";
        private const string SPNAME_GETONE = "[List].[GetCustomListHeaderById]";
        private const string SPNAME_SAVE = "[List].[AddOrUpdateCustomListHeader]";
        #endregion

        #region constructor
        public CustomListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        #region methods
        public CustomListHeader GetCustomListHeader(long Id) {
            return ReadOne<CustomListHeader>(SPNAME_GETONE, PARMNAME_ID, Id);
        }

        public List<CustomListHeader> GetCustomListHeadersByCustomer(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return Read<CustomListHeader>(SPNAME_GETBYCUST, parms);
        }

        public void SaveCustomListHeader(CustomListHeader model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_BRANCH, model.BranchId);
            parms.Add(PARMNAME_CUSTNUM, model.CustomerNumber);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_NAME, model.Name);
            parms.Add(PARMNAME_USERID, model.UserId);

            ExecuteCommand(SPNAME_SAVE, parms);
        }
        #endregion
    }
}
