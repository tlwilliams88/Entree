using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Models.ContentManagement.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalContentManagementLogic : IInternalContentManagementLogic
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IContentManagementItemRepository contentManagementRepository;

        public InternalContentManagementLogic(IContentManagementItemRepository contentManagementRepository, IUnitOfWork unitOfWork)
		{
            this.contentManagementRepository = contentManagementRepository;
			this.unitOfWork = unitOfWork;
		}

        public void CreateContentItem(Core.Models.ContentManagement.ContentItemPostModel contentItemModel)
        {
            ContentItem contentItem = ToContentItem(contentItemModel); // TODO: move to extension method
            
            if (!String.IsNullOrEmpty(contentItemModel.Base64ImageData))
            {
                // TODO: persist image somewhere....
                contentItem.ImageUrl = "";
            }

            contentManagementRepository.Create(contentItem);
        }

        private static ContentItem ToContentItem(Core.Models.ContentManagement.ContentItemPostModel contentItemModel)
        {
            ContentItem contentItem = new ContentItem();
            contentItem.ActiveDateEnd = contentItemModel.ActiveDateEnd;
            contentItem.ActiveDateStart = contentItemModel.ActiveDateStart;
            contentItem.BranchId = contentItemModel.BranchId;
            contentItem.CampaignId = contentItemModel.CampaignId;
            contentItem.Content = contentItemModel.Content;
            contentItem.IsContentHtml = contentItemModel.IsContentHtml;
            contentItem.TagLine = contentItemModel.TagLine;
            contentItem.TargetUrl = contentItemModel.TargetUrl;
            contentItem.TargetUrlText = contentItemModel.TargetUrlText;

            return contentItem;
        }
    }
}
