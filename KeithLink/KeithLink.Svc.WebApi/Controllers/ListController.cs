using KeithLink.Svc.Core;
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

        private readonly IListRepository listRepository;

        public ListController(IListRepository listRepository)
        {
            this.listRepository = listRepository;
        }


        [HttpGet]
        [Route("list")]
        public List<UserList> List()
        {
            var model = listRepository.ReadAllLists();
            return model;
        }

        [HttpPut]
        [Route("list/{listId}")]
        public void Put(Guid listId, string newName)
        {
            //TODO: This endpoint is currently not working
            listRepository.UpdateListName(listId, newName);
        }


        [HttpGet]
        [Route("list/{listId}")]
        public UserList List(Guid listId)
        {
            return listRepository.ReadList(listId);
        }

        [HttpGet]
        [Route("list/labels")]
        public List<string> ListLabels()
        {
            return listRepository.ReadListLabels();
        }

        [HttpGet]
        [Route("list/{listId}/labels")]
        public List<string> ListLabels(Guid listId)
        {
            return listRepository.ReadListLabels(listId);
        }

        [HttpPost]
        [Route("list")]
        public Guid List(UserList list)
        {
            return listRepository.CreateList(list);
        }

        [HttpDelete]
        [Route("list/{listId}")]
        public void DeleteList(Guid listId)
        {
            listRepository.DeleteList(listId);
        }

        [HttpPost]
        [Route("list/{listId}/item")]
        public Guid? AddItem(Guid listId, ListItem newItem)
        {
            return listRepository.AddItem(listId, newItem);
        }

        [HttpPut]
        [Route("list/{listId}/item")]
        public void UpdateItem(Guid listId, ListItem updatedItem)
        {
            listRepository.UpdateItem(listId, updatedItem);
        }

        [HttpDelete]
        [Route("list/{listId}/item/{itemId}")]
        public void DeleteItem(Guid listId, Guid itemId)
        {
            listRepository.DeleteItem(listId, itemId);
        }
    }
}
