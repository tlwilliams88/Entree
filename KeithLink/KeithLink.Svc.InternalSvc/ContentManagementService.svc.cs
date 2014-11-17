using KeithLink.Svc.Core.Models.ContentManagement;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.InternalSvc.Interfaces;
using KeithLink.Common.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InvoiceService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select InvoiceService.svc or InvoiceService.svc.cs at the Solution Explorer and start debugging.
	public class ContentManagementService : IContentManagementService
	{
		private IInternalContentManagementLogic internalCmsLogic;
        private IEventLogRepository eventLogRepository;

        public ContentManagementService(IInternalContentManagementLogic internalCmsLogic, IEventLogRepository eventLogRepository)
		{
			this.internalCmsLogic = internalCmsLogic;
            this.eventLogRepository = eventLogRepository;
		}

		public void CreateContentItem(ContentItemPostModel contentItem)
		{
            internalCmsLogic.CreateContentItem(contentItem);
            if (!String.IsNullOrEmpty(contentItem.Base64ImageData))
            {
                this.eventLogRepository.WriteInformationLog("ContentManagementService.CreateContentItem: Received base64 encoded data ____" 
                    + contentItem.Base64ImageData + "____");
		    }
        }
	}
}
