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

        #region functions

        /// <summary>
        /// Process and import contract items
        /// </summary>
        public void ImportContractItems() {
            DateTime startTime = DateTime.Now;

            try {
                _stagingRepository.ProcessContractItems();
            } catch (Exception ex) {
                _eventLogRepository.WriteErrorLog( "Error importing contract items lists", ex );
            }

            _eventLogRepository.WriteInformationLog(string.Format("ImportContractItems Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        /// <summary>
        /// Process and import worksheet items
        /// </summary>
        public void ImportWorksheetItems() {
            DateTime startTime = DateTime.Now;

            try {
                _stagingRepository.ProcessWorksheetItems();
            } catch (Exception ex) {
                _eventLogRepository.WriteErrorLog( "Error importing Worksheet items lists", ex );
            }

            _eventLogRepository.WriteInformationLog(string.Format("ImportWorksheetItems Runtime - {0}", (DateTime.Now - startTime).ToString("h'h 'm'm 's's'")));
        }

        #endregion
    }
}
