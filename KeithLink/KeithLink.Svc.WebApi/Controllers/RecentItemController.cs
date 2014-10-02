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
    public class RecentItemController : BaseController
    {
		private readonly IRecentlyViewedListLogic recentlyViewedLogic;

		public RecentItemController(IRecentlyViewedListLogic recentlyViewedLogic, IUserProfileRepository userProfileRepo)
			: base(userProfileRepo)
        {
			this.recentlyViewedLogic = recentlyViewedLogic;
        }

		[HttpGet]
		[ApiKeyedRoute("recent/")]
		public List<RecentItem> Recent()
		{
			return recentlyViewedLogic.Read(this.AuthenticatedUser, this.RequestCatalogInfo);
		}

		[HttpPost]
		[ApiKeyedRoute("recent/{itemnumber}")]
		public void Recent(string itemnumber)
		{
			recentlyViewedLogic.AddItem(this.AuthenticatedUser, this.RequestCatalogInfo, itemnumber);
		}

		[HttpDelete]
		[ApiKeyedRoute("recent/")]
		public void RecentDelete()
		{
			recentlyViewedLogic.Clear(this.AuthenticatedUser, this.RequestCatalogInfo);
		}

    }
}
