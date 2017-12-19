﻿using Dapper;
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
    public class OrderedItemsFromListRepositoryImpl : DapperDatabaseConnection, IOrderedItemsFromListRepository
    {
        #region attributes
        private const string PARMNAME_CONTROLNUM = "ControlNumber";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_SOURCELIST = "SourceList";

        private const string STOREDPROC_GET_ONE = "[Orders].[ReadOrderItemListAssociation]";
        private const string STOREDPROC_WRITE_ONE = "[Orders].[WriteOrderItemListAssociation]";
        private const string STOREDPROC_PURGE_BY_DAYS = "[Orders].[PurgeOrderItemsListAssociation]";
        #endregion

        #region constructor
        public OrderedItemsFromListRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        public OrderItemFromList Read(string controlNumber, string itemNumber)
        {
            return ReadOne<OrderItemFromList>(new CommandDefinition(
                STOREDPROC_GET_ONE,
                new { @ControlNumber = controlNumber, @ItemNumber = itemNumber },
                commandType: CommandType.StoredProcedure
            ));
        }

        public void Write(OrderItemFromList o2l) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_CONTROLNUM, o2l.ControlNumber);
            parms.Add(PARMNAME_ITEMNUMBER, o2l.ItemNumber);
            parms.Add(PARMNAME_SOURCELIST, o2l.SourceList);

            ExecuteCommand(STOREDPROC_WRITE_ONE, parms);
        }

        public void Purge(int PurgeDays)
        {
            if (PurgeDays < 0)
            {
                ExecuteCommand(STOREDPROC_PURGE_BY_DAYS, "@PurgeDays", PurgeDays);
            }
        }
    }
}
