using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Models.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.WebApi.Repository.ContentManagement
{
	public class ContentManagementServiceRepositoryImpl : IContentManagementServiceRepository
	{		
		private com.benekeith.ContentManagementService.IContentManagementService serviceClient;

		public ContentManagementServiceRepositoryImpl(com.benekeith.ContentManagementService.IContentManagementService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

        public void CreateContentItem(ContentItemPostModel item)
        {
            serviceClient.CreateContentItem(item);
        }

        public List<ContentItemViewModel> ReadActiveContentItemsByBranch(string branchId, int count)
        {
            return serviceClient.ReadActiveContentItemsByBranch(branchId, count).ToList();
        }

        public void DeleteContentItemById(int id)
        {
            serviceClient.DeleteContentItemById(id);
        }

        public ContentItemViewModel ReadContentItemById(int itemId)
        {
            return serviceClient.ReadContentItemById(itemId);
        }
    }
}
