using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class BaseController : ApiController
    {
        #region attributes
        private bool _isAuthenticated;
        private Core.Profile.IUserProfileRepository _userRepo;
        #endregion

        #region ctor
        public BaseController(Core.Profile.IUserProfileRepository userProfileRepo)
        {
            _userRepo = userProfileRepo;
        }
        #endregion

        #region methods
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            _isAuthenticated = controllerContext.RequestContext.Principal.Identity.IsAuthenticated;
        }
        #endregion

        #region properties
        public Core.Profile.UserProfile AuthenticatedUser
        {
            get 
            {
                if (IsAuthenticated)
                {
                    Core.Profile.UserProfileReturn retVal = _userRepo.GetUserProfile(
                                                                ((System.Security.Claims.ClaimsIdentity)this.ControllerContext.RequestContext.Principal.Identity).FindFirst("name").Value
                                                            );

                    return retVal.UserProfiles[0];

                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }
        #endregion
    }
}