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
    public class ListController : BaseController
    {
		private readonly IListLogic listLogic;

		public ListController(IListLogic listLogic, IUserProfileRepository userProfileRepo): base (userProfileRepo)
        {
            this.listLogic = listLogic;
        }


        [HttpGet]
		[Route("list/{branchId}")]
        public List<UserList> List(string branchId, bool header = false)
        {
			var model = listLogic.ReadAllLists(this.AuthenticatedUser, branchId, header);
            return model;
        }

        [HttpPut]
		[Route("list/")]
		public void Put(UserList updatedList)
        {
			listLogic.UpdateList(this.AuthenticatedUser.UserId, updatedList);
        }


        [HttpGet]
		[Route("list/{branchId}/{listId}")]
		public UserList List(Guid listId)
        {
			return listLogic.ReadList(this.AuthenticatedUser, listId);
        }

        [HttpGet]
		[Route("list/{branchId}/labels")]
        public List<string> ListLabels(string branchId)
        {
			return listLogic.ReadListLabels(this.AuthenticatedUser.UserId, branchId);
        }

        [HttpGet]
		[Route("list/{branchId}/{listId}/labels")]
		public List<string> ListLabels(string branchId, Guid listId)
        {
			return listLogic.ReadListLabels(this.AuthenticatedUser.UserId, listId);
        }

        [HttpPost]
		[Route("list/{branchId}")]
		public NewItem List(string branchId, UserList list)
        {
			var newGuid = new NewItem() { ListItemId = listLogic.CreateList(this.AuthenticatedUser.UserId, branchId, list) };
			return newGuid;
        }

        [HttpDelete]
		[Route("list/{listId}")]
		public void DeleteList(Guid listId)
        {
			listLogic.DeleteList(this.AuthenticatedUser.UserId, listId);
        }

        [HttpPost]
		[Route("list/{listId}/item")]
		public NewItem AddItem(Guid listId, ListItem newItem)
        {
			var newGuid = new NewItem() { ListItemId = listLogic.AddItem(this.AuthenticatedUser.UserId, listId, newItem) };
			return newGuid;
        }

        [HttpPut]
		[Route("list/{listId}/item")]
		public void UpdateItem(Guid listId, ListItem updatedItem)
        {
			listLogic.UpdateItem(this.AuthenticatedUser.UserId, listId, updatedItem);
        }

        [HttpDelete]
		[Route("list/{listId}/item/{itemId}")]
		public void DeleteItem(Guid listId, Guid itemId)
        {
			listLogic.DeleteItem(this.AuthenticatedUser.UserId, listId, itemId);
        }
    }
}
