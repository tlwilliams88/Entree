using Dapper;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class OrderedFromListRepositoryImpl : DapperDatabaseConnection, IOrderedFromListRepository
    {
        #region attributes
        private const string PARMNAME_CONTROLNUM = "ControlNumber";
        private const string PARMNAME_LISTID = "ListId";
        private const string PARMNAME_LISTTYPE = "ListType";

        private const string STOREDPROC_GET_ONE = "[Orders].[ReadOrderListAssociation]";
        private const string STOREDPROC_WRITE_ONE = "[Orders].[WriteOrderListAssociation]";
        private const string STOREDPROC_DELETE_ONE = "[Orders].[DeleteOrderListAssociation]";
        private const string STOREDPROC_PURGE_BY_DAYS = "[Orders].[PurgeOrderListAssociation]";
        #endregion

        #region constructor
        public OrderedFromListRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        public OrderedFromList Read(string controlNumber)
        {
            return ReadOne<OrderedFromList>(new CommandDefinition(
                STOREDPROC_GET_ONE,
                new { @ControlNumber = controlNumber },
                commandType: CommandType.StoredProcedure
            ));
        }
 
        public void Write(OrderedFromList o2l)
        {
            if(o2l.ListId != null)
            {
                DynamicParameters parms = new DynamicParameters();
                parms.Add(PARMNAME_CONTROLNUM, o2l.ControlNumber);
                parms.Add(PARMNAME_LISTID, o2l.ListId.Value);
                parms.Add(PARMNAME_LISTTYPE, o2l.ListType);

                ExecuteSPCommand(STOREDPROC_WRITE_ONE, parms);
            }
        }


        public void Delete(string controlNumber)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_CONTROLNUM, controlNumber);

            ExecuteSPCommand(STOREDPROC_DELETE_ONE, parms);
        }

        public void Purge(int PurgeDays)
        {
            if (PurgeDays < 0)
            {
                ExecuteSPCommand(STOREDPROC_PURGE_BY_DAYS, "@PurgeDays", PurgeDays);
            }
        }
    }
}
