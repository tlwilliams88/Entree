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

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class ListController : BaseController {
        #region attributes
        private readonly IListLogic listLogic;
        #endregion

        #region ctor
        public ListController(IListLogic listLogic, IUserProfileLogic profileLogic): base (profileLogic) {
            this.listLogic = listLogic;
        }
        #endregion

        #region methods
        [HttpGet]
		[ApiKeyedRoute("list/")]
        public List<UserList> List(bool header = false)
        {
			var model = listLogic.ReadAllLists(this.AuthenticatedUser, this.RequestCatalogInfo, header);
            return model;
        }
		
        [HttpGet]
		[ApiKeyedRoute("list/{listId}")]
		public UserList List(Guid listId)
        {
			return listLogic.ReadList(this.AuthenticatedUser, listId, this.RequestCatalogInfo);
        }

        [HttpGet]
		[ApiKeyedRoute("list/labels")]
        public List<string> ListLabels()
        {
			return listLogic.ReadListLabels(this.AuthenticatedUser.UserId, this.RequestCatalogInfo);
        }

        [HttpGet]
		[ApiKeyedRoute("list/{listId}/labels")]
		public List<string> ListLabels(Guid listId)
        {
			return listLogic.ReadListLabels(this.AuthenticatedUser.UserId, listId);
        }

        [HttpPost]
		[ApiKeyedRoute("list/")]
		public NewItem List(UserList list)
        {
			var newGuid = new NewItem() { ListItemId = listLogic.CreateList(this.AuthenticatedUser.UserId, this.RequestCatalogInfo, list) };
			return newGuid;
        }
		
        [HttpPost]
		[ApiKeyedRoute("list/{listId}/item")]
		public NewItem AddItem(Guid listId, ListItem newItem)
        {
			var newGuid = new NewItem() { ListItemId = listLogic.AddItem(this.AuthenticatedUser.UserId, listId, newItem) };
			return newGuid;
        }

		[HttpPost]
		[ApiKeyedRoute("list/{listId}/items")]
		public UserList AddItems(Guid listId, List<ListItem> newItems, bool allowDuplicates = false)
		{
			return listLogic.AddItems(AuthenticatedUser, this.RequestCatalogInfo, listId, newItems, false);
		}
		
        [HttpPut]
		[ApiKeyedRoute("list/{listId}/item")]
		public void UpdateItem(Guid listId, ListItem updatedItem)
        {
			listLogic.UpdateItem(this.AuthenticatedUser.UserId, listId, updatedItem, this.RequestCatalogInfo);
        }

		[HttpPut]
		[ApiKeyedRoute("list/")]
		public void Put(UserList updatedList)
		{
			listLogic.UpdateList(this.AuthenticatedUser.UserId, updatedList, this.RequestCatalogInfo);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/{listId}")]
		public void DeleteList(Guid listId)
		{
			listLogic.DeleteList(this.AuthenticatedUser.UserId, listId);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/")]
		public void DeleteList(List<Guid> listIds)
		{
			listLogic.DeleteLists(this.AuthenticatedUser.UserId, listIds);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/{listId}/item/{itemId}")]
		public void DeleteItem(Guid listId, Guid itemId)
		{
			listLogic.DeleteItem(this.AuthenticatedUser.UserId, listId, itemId);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/{listId}/item")]
		public void DeleteItem(Guid listId, List<Guid> itemIds)
		{
			var t = itemIds;
			listLogic.DeleteItems(this.AuthenticatedUser.UserId, listId, itemIds);
		}
        #endregion
    }
}
