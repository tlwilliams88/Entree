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
    /// <summary>
    /// recent item controller
    /// </summary>
    public class RecentItemController : BaseController {
        #region attributes
        private readonly IListLogic listServiceRepository;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="listServiceRepository"></param>
        /// <param name="profileLogic"></param>
        public RecentItemController(IListLogic listServiceRepository,  IUserProfileLogic profileLogic)  : base(profileLogic) {
			this.listServiceRepository = listServiceRepository;
        }
        #endregion

        #region methods
		/// <summary>
		/// Retrieve recently viewed items
		/// </summary>
		/// <returns></returns>
        [HttpGet]
		[ApiKeyedRoute("recent/")]
		public List<RecentItem> Recent() {
			return listServiceRepository.ReadRecent(this.AuthenticatedUser, this.SelectedUserContext);
		}

		/// <summary>
		/// Add a new recently viewed item
		/// </summary>
		/// <param name="itemnumber"></param>
		[HttpPost]
		[ApiKeyedRoute("recent/{itemnumber}")]
		public void Recent(string itemnumber) {
			listServiceRepository.AddRecentlyViewedItem(this.AuthenticatedUser, this.SelectedUserContext, itemnumber);
		}
				
        #endregion

    }
}
