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

    }
}
