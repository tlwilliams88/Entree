using KeithLink.Svc.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	

    public class ProfileController : ApiController
    {
        #region attributes
        private Core.Profile.ICustomerContainerRepository _custRepo;
        private Core.Profile.IUserProfileRepository _profileRepo;
        #endregion

        #region ctor
        public ProfileController(Core.Profile.ICustomerContainerRepository customerRepo, Core.Profile.IUserProfileRepository profileRepo) {
            _custRepo = customerRepo;
            _profileRepo = profileRepo;
        }
        #endregion

        #region methods
        //[HttpGet]
		//[Route("profile/{emailAddress}")]
		//public Core.Profile.UserProfileReturn GetUser(string emailAddress)
		//{
		//	Impl.Profile.UserProfileRepository userRepo = new Impl.Profile.UserProfileRepository();

		//	return userRepo.GetUserProfile(emailAddress);
		//}

        [HttpPost]
        [Route("profile/login")]
        public Core.Profile.UserProfileReturn Login(LoginModel login)
        {
            Core.Profile.UserProfileReturn retVal = null;

			if (_profileRepo.AuthenticateUser(login.Email, login.Password, out retVal))
			{
				return retVal;
			}
			else
			{
				return null;
			}
        }

        [HttpGet]
        [Route("profile/searchcustomer/{searchText}")]
        public Core.Profile.CustomerContainerReturn SearchCustomers(string searchText)
        {
            return _custRepo.SearchCustomerContainers(searchText);
        }
        #endregion
    }
}