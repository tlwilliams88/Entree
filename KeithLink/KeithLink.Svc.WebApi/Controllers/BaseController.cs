using KeithLink.Svc.WebApi.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace KeithLink.Svc.WebApi.Controllers
{
	[GlobalExceptionFilterAttribute]
	public class BaseController : ApiController
    {
        #region attributes
        private Core.Interface.Profile.IUserProfileRepository _userRepo;
        private Core.Models.Profile.UserProfile _user;
        #endregion

        #region ctor
        public BaseController(Core.Interface.Profile.IUserProfileRepository userProfileRepo)
        {
            _userRepo = userProfileRepo;
        }
        #endregion

        #region methods
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            if (controllerContext.RequestContext.Principal.Identity.IsAuthenticated && 
                string.Compare(controllerContext.RequestContext.Principal.Identity.AuthenticationType, "bearer", true) == 0)
            {
                Core.Models.Profile.UserProfileReturn retVal = _userRepo.GetUserProfile(
                                                                ((System.Security.Claims.ClaimsIdentity)this.ControllerContext.RequestContext.Principal.Identity).FindFirst("name").Value
                                                            );

                _user = retVal.UserProfiles[0];
                _user.IsAuthenticated = true;

                System.Security.Principal.GenericPrincipal genPrincipal = new System.Security.Principal.GenericPrincipal(_user, new string[] { "Owner" });
                controllerContext.RequestContext.Principal = genPrincipal;
            }
            else
            {
                _user = null;
            }
        }
        #endregion

        #region properties
        public Core.Models.Profile.UserProfile AuthenticatedUser
        {
			get
			{
				if (!ControllerContext.RequestContext.Principal.Identity.IsAuthenticated)
					return null;
				else
					return (Core.Models.Profile.UserProfile)ControllerContext.RequestContext.Principal.Identity;
			}
        }
		
        #endregion
    }
}