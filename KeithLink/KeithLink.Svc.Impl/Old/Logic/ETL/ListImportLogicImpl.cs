﻿// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using Entree.Core.Interface.ETL;
using Entree.Core.Interface.InternalCatalog;

using KeithLink.Svc.Impl.Models;

// Core
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Sql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Interface.Orders;
using Entree.Core.Interface.Lists;

namespace KeithLink.Svc.Impl.Logic.ETL {
    public class ListImportLogicImpl : IListsImportLogic {
        #region attributes

        IStagingRepository _stagingRepository;
        IEventLogRepository _eventLogRepository;
        private readonly IOrderedFromListRepository _order2ListRepo;
        private readonly IOrderedItemsFromListRepository _orderItemsFromListRepo;
        private readonly IContractChangesRepository _contractChangesRepo;

        #endregion

        #region constructor

        public ListImportLogicImpl( IStagingRepository stagingRepository, IEventLogRepository eventLogRepository,
            IOrderedFromListRepository order2ListRepo, IOrderedItemsFromListRepository orderItemsFromListRepo, IContractChangesRepository contractChangesRepo) {
            _stagingRepository = stagingRepository;
            _eventLogRepository = eventLogRepository;
            _order2ListRepo = order2ListRepo;
            _orderItemsFromListRepo = orderItemsFromListRepo;
            _contractChangesRepo = contractChangesRepo;
        }

        #endregion

        #region methods

        /// <summary>
        /// Process and import contract items
        /// </summary>
        public void ImportContractItems() {
            try {
                DateTime start = DateTime.Now;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import contract lists {0}", start.ToString()));

                _stagingRepository.ProcessContractItems();

                TimeSpan took = DateTime.Now - start;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import contract lists.  Process took {0}", took.ToString()));

                start = DateTime.Now;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Starting:  Purge contract changes {0}", start.ToString()));

                _contractChangesRepo.Purge(Configuration.ContractChangesPurgeDays);

                took = DateTime.Now - start;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Finished:  Purge contract changes.  Process took {0}", took.ToString()));
            }
            catch (Exception ex) {
                _eventLogRepository.WriteErrorLog(String.Format("ETL: Error Importing contract lists -- whole process failed.  {0} -- {1}", ex.Message, ex.StackTrace));

                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex,
                    "ETL: Error Importing contract lists -- whole process failed.");

            }
        }

        /// <summary>
        /// Process and import worksheet items
        /// </summary>
        public void ImportWorksheetItems() {
            DateTime startTime = DateTime.Now;

            try {
                DateTime start = DateTime.Now;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import history lists {0}", start.ToString()));

                _stagingRepository.ProcessWorksheetItems();

                TimeSpan took = DateTime.Now - start;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import history lists.  Process took {0}", took.ToString()));

            } 
            catch (Exception ex) {
                _eventLogRepository.WriteErrorLog(String.Format("ETL: Error Importing history lists -- whole process failed.  {0} -- {1}", ex.Message, ex.StackTrace));

                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex,
                    "ETL: Error Importing history lists -- whole process failed.");
            }

            try
            {
                DateTime start = DateTime.Now;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Starting:  Purge cart/order to list associations {0}", start.ToString()));

                _order2ListRepo.Purge(Configuration.CartOrOrder2ListIdPurgeDays);

                TimeSpan took = DateTime.Now - start;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Finished:  Purge cart/order to list associations.  Process took {0}", took.ToString()));

            }
            catch (Exception ex)
            {
                _eventLogRepository.WriteErrorLog(String.Format("ETL: Error Purge cart/order to list associations -- whole process failed.  {0} -- {1}", ex.Message, ex.StackTrace));
                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex,
                    "ETL: Error Purge cart/order to list associations -- whole process failed.");
            }

            try
            {
                DateTime start = DateTime.Now;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Starting:  Purge cart items/order from list associations {0}", start.ToString()));

                _orderItemsFromListRepo.Purge(Configuration.CartOrOrder2ListIdPurgeDays);

                TimeSpan took = DateTime.Now - start;
                _eventLogRepository.WriteInformationLog(String.Format("ETL: Import Process Finished:  Purge cart items/order from list associations.  Process took {0}", took.ToString()));

            }
            catch (Exception ex)
            {
                _eventLogRepository.WriteErrorLog(String.Format("ETL: Error Purge cart items/order from list associations -- whole process failed.  {0} -- {1}", ex.Message, ex.StackTrace));
                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex,
                    "ETL: Error Purge cart items/order from list associations -- whole process failed.");
            }
        }

        #endregion
    }
}
