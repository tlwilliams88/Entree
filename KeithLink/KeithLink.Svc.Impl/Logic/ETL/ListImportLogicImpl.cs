// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.InternalCatalog;

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

namespace KeithLink.Svc.Impl.Logic.ETL {
    public class ListImportLogicImpl : IListsImportLogic {
        #region attributes

        IStagingRepository _stagingRepository;
        IEventLogRepository _eventLogRepository;

        #endregion

        #region constructor

        public ListImportLogicImpl( IStagingRepository stagingRepository, IEventLogRepository eventLogRepository ) {
            _stagingRepository = stagingRepository;
            _eventLogRepository = eventLogRepository;
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

            } 
            catch (Exception ex) {
                _eventLogRepository.WriteErrorLog(String.Format("ETL: Error Importing contract lists -- whole process failed.  {0} -- {1}", ex.Message, ex.StackTrace));
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
            }

            
        }

        #endregion
    }
}
