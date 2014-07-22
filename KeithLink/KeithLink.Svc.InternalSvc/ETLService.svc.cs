using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.InternalSvc.Interfaces;

namespace KeithLink.Svc.InternalSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ETLService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ETLService.svc or ETLService.svc.cs at the Solution Explorer and start debugging.
    public class ETLService : IETLService
    {
        private readonly ICatalogLogic categoryLogic;

        public ETLService(ICatalogLogic categoryLogic)
        {
            this.categoryLogic = categoryLogic;
        }

        public bool ProcessedStagedData()
        {
            categoryLogic.ProcessStagedData();
            return true;
        }
    }
}
