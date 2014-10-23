using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class RecentItemController : BaseController {
        #region attributes
        private readonly IListServiceRepository listServiceRepository;
        
        #endregion

        #region ctor
        public RecentItemController(IListServiceRepository listServiceRepository,  IUserProfileLogic profileLogic)  : base(profileLogic) {
			this.listServiceRepository = listServiceRepository;
        }
        #endregion

        #region methods
        [HttpGet]
		[ApiKeyedRoute("recent/")]
		public List<RecentItem> Recent() {
			return listServiceRepository.ReadRecent(this.AuthenticatedUser, this.SelectedUserContext);
		}

		[HttpPost]
		[ApiKeyedRoute("recent/{itemnumber}")]
		public void Recent(string itemnumber) {
			listServiceRepository.AddRecentlyViewedItem(this.AuthenticatedUser, this.SelectedUserContext, itemnumber);
		}
				
        #endregion

    }
}
