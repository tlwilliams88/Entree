using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class ListController : ApiController
    {

        private readonly IListLogic listLogic;

        public ListController(IListLogic listLogic)
        {
            this.listLogic = listLogic;
        }


        [HttpGet]
		[Route("list/{branchId}")]
        public List<UserList> List(string branchId, bool header = false)
        {
			var model = listLogic.ReadAllLists(branchId, header);
            return model;
        }

        [HttpPut]
		[Route("list/")]
		public void Put(UserList updatedList)
        {
            listLogic.UpdateList(updatedList);
        }


        [HttpGet]
		[Route("list/{branchId}/{listId}")]
		public UserList List(Guid listId)
        {
            return listLogic.ReadList(listId);
        }

        [HttpGet]
		[Route("list/{branchId}/labels")]
        public List<string> ListLabels(string branchId)
        {
            return listLogic.ReadListLabels(branchId);
        }

        [HttpGet]
		[Route("list/{branchId}/{listId}/labels")]
		public List<string> ListLabels(string branchId, Guid listId)
        {
            return listLogic.ReadListLabels(listId);
        }

        [HttpPost]
		[Route("list/{branchId}")]
		public Guid List(string branchId, UserList list)
        {
            return listLogic.CreateList(branchId, list);
        }

        [HttpDelete]
		[Route("list/{listId}")]
		public void DeleteList(Guid listId)
        {
            listLogic.DeleteList(listId);
        }

        [HttpPost]
		[Route("list/{listId}/item")]
		public NewItem AddItem(Guid listId, ListItem newItem)
        {
			var newGuid = new NewItem() { ListItemId = listLogic.AddItem(listId, newItem) };
			return newGuid;
        }

        [HttpPut]
		[Route("list/{listId}/item")]
		public void UpdateItem(Guid listId, ListItem updatedItem)
        {
            listLogic.UpdateItem(listId, updatedItem);
        }

        [HttpDelete]
		[Route("list/{listId}/item/{itemId}")]
		public UserList DeleteItem(Guid listId, Guid itemId)
        {
            return listLogic.DeleteItem(listId, itemId);
        }
    }
}
