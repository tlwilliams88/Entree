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
            unitOfWork.SaveChanges();
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


        public List<Core.Models.ContentManagement.ContentItemViewModel> ReadActiveContentItemsByBranch(string branchId, int count)
        {
            IEnumerable<ContentItem> contentItems = contentManagementRepository.ReadActiveContentItemsByBranch(branchId, count);
            List<Core.Models.ContentManagement.ContentItemViewModel> itemModels = new List<Core.Models.ContentManagement.ContentItemViewModel>();
            foreach (var item in contentItems)
            {
                itemModels.Add(ToItemViewModel(item));
            }

            return itemModels;
        }

        public void DeleteContentItemById(int itemId)
        {
            ContentItem toDelete = contentManagementRepository.ReadById(itemId);
            contentManagementRepository.Delete(toDelete);
            unitOfWork.SaveChanges();
        }

        public Core.Models.ContentManagement.ContentItemViewModel ReadContentItemById(int itemId)
        {
            ContentItem item = contentManagementRepository.ReadById(itemId);
            return ToItemViewModel(item);
        }

        private static Core.Models.ContentManagement.ContentItemViewModel ToItemViewModel(ContentItem item)
        {
            Core.Models.ContentManagement.ContentItemViewModel itemModel = new Core.Models.ContentManagement.ContentItemViewModel();
            itemModel.ImageUrl = item.ImageUrl;
            itemModel.ContentItemId = item.Id;
            itemModel.ActiveDateEnd = item.ActiveDateEnd;
            itemModel.ActiveDateStart = item.ActiveDateStart;
            itemModel.BranchId = item.BranchId;
            itemModel.CampaignId = item.CampaignId;
            itemModel.Content = item.Content;
            itemModel.IsContentHtml = item.IsContentHtml;
            itemModel.ProductId = item.ProductId;
            itemModel.TagLine = item.TagLine;
            itemModel.TargetUrl = item.TargetUrl;
            itemModel.TargetUrlText = item.TargetUrlText;
            return itemModel;
        }
    }
}
