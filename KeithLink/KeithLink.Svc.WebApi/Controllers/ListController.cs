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
        [Route("list")]
        public List<UserList> List(bool header = false)
        {
			var model = listLogic.ReadAllLists(header);
            return model;
        }

        [HttpPut]
        [Route("list")]
        public void Put(UserList updatedList)
        {
            listLogic.UpdateList(updatedList);
        }


        [HttpGet]
        [Route("list/{listId}")]
        public UserList List(Guid listId)
        {
            return listLogic.ReadList(listId);
        }

        [HttpGet]
        [Route("list/labels")]
        public List<string> ListLabels()
        {
            return listLogic.ReadListLabels();
        }

        [HttpGet]
        [Route("list/{listId}/labels")]
        public List<string> ListLabels(Guid listId)
        {
            return listLogic.ReadListLabels(listId);
        }

        [HttpPost]
        [Route("list")]
        public Guid List(UserList list)
        {
            return listLogic.CreateList(list);
        }

        [HttpDelete]
        [Route("list/{listId}")]
        public void DeleteList(Guid listId)
        {
            listLogic.DeleteList(listId);
        }

        [HttpPost]
        [Route("list/{listId}/item")]
        public Guid? AddItem(Guid listId, ListItem newItem)
        {
            return listLogic.AddItem(listId, newItem);
        }

        [HttpPut]
        [Route("list/{listId}/item")]
        public void UpdateItem(Guid listId, ListItem updatedItem)
        {
            listLogic.UpdateItem(listId, updatedItem);
        }

        [HttpDelete]
        [Route("list/{listId}/item/{itemId}")]
        public void DeleteItem(Guid listId, Guid itemId)
        {
            listLogic.DeleteItem(listId, itemId);
        }
    }
}
