using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Common.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class ListController : BaseController {
        #region attributes
        private readonly IListServiceRepository listServiceRepository;
        #endregion

        #region ctor
		public ListController(IUserProfileLogic profileLogic, IListServiceRepository listServiceRepository)
			: base(profileLogic)
		{
            this.listServiceRepository = listServiceRepository;
        }
        #endregion

        #region methods

		[HttpPost]
		[ApiKeyedRoute("list/export/{listId}")]
		public HttpResponseMessage ExportList(long listId, ExportRequestModel exportRequest)
		{
			var list = listServiceRepository.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId);
			MemoryStream stream;

			if(exportRequest.Fields == null)
				stream = new ModelExporter<ListItemModel>(list.Items).Export(exportRequest.SelectedType);
			else
				stream = new ModelExporter<ListItemModel>(list.Items, exportRequest.Fields).Export(exportRequest.SelectedType);

			HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			result.Content.Headers.ContentDisposition.FileName = "export.csv";
			return result;
		}

        [HttpGet]
		[ApiKeyedRoute("list/")]
        public List<ListModel> List(bool header = false)
        {
			return listServiceRepository.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, header);
        }
		
        [HttpGet]
		[ApiKeyedRoute("list/{listId}")]
		public ListModel List(long listId)
        {
			return listServiceRepository.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId);
        }

        [HttpGet]
		[ApiKeyedRoute("list/labels")]
        public List<string> ListLabels()
        {
			return listServiceRepository.ReadListLabels(this.AuthenticatedUser, this.SelectedUserContext);
        }

        [HttpGet]
        [ApiKeyedRoute("list/reminders")]
        public List<ListModel> ListReminders() {
            return listServiceRepository.ReadReminders(this.AuthenticatedUser, this.SelectedUserContext);
        }

        [HttpPost]
		[ApiKeyedRoute("list/")]
		public NewListItem List(ListModel list, bool isMandatory = false)
        {
			return new NewListItem() { Id = listServiceRepository.CreateList(this.AuthenticatedUser.UserId, this.SelectedUserContext, list, isMandatory ? ListType.Mandatory : ListType.Custom) };
        }
		
        [HttpPost]
		[ApiKeyedRoute("list/{listId}/item")]
		public NewListItem AddItem(long listId, ListItemModel newItem)
        {
			return new NewListItem() { Id = listServiceRepository.AddItem(listId, newItem) };
        }

		[HttpPost]
		[ApiKeyedRoute("list/{listId}/items")]
		public ListModel AddItems(long listId, List<ListItemModel> newItems, bool allowDuplicates = false)
		{
			return listServiceRepository.AddItems(this.AuthenticatedUser, this.SelectedUserContext, listId, newItems);
		}
		
		[HttpPost]
		[ApiKeyedRoute("list/copy")]
		public void CopyList(ListCopyShareModel copyListModel)
		{
			listServiceRepository.CopyList(copyListModel);
		}

		[HttpPost]
		[ApiKeyedRoute("list/share")]
		public void ShareList(ListCopyShareModel copyListModel)
		{
			listServiceRepository.ShareList(copyListModel);
		}

		
        [HttpPut]
		[ApiKeyedRoute("list/item")]
		public void UpdateItem(ListItemModel updatedItem)
        {
			listServiceRepository.UpdateItem(updatedItem);
        }

		[HttpPut]
		[ApiKeyedRoute("list/")]
		public void Put(ListModel updatedList)
		{
			listServiceRepository.UpdateList(updatedList);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/{listId}")]
		public void DeleteList(long listId)
		{
			listServiceRepository.DeleteList(listId);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/")]
		public void DeleteList(List<long> listIds)
		{
			listServiceRepository.DeleteLists(listIds);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/item/{itemId}")]
		public void DeleteItem(long itemId)
		{
			listServiceRepository.DeleteItem(itemId);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/item")]
		public void DeleteItem(List<long> itemIds)
		{
			listServiceRepository.DeleteItems(itemIds);
		}



		
        #endregion
    }
}
