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
        [HttpPost]
        [Route("profile/create")]
        public void CreateUser(UserProfileModel userInfo)
        {
            Core.Profile.CustomerContainerReturn custExists = _custRepo.SearchCustomerContainers(userInfo.CustomerName);

            // create the customer container if it does not exist
            if (custExists.CustomerContainers.Count != 1) { _custRepo.CreateCustomerContainer(userInfo.CustomerName); }

            _profileRepo.CreateUserProfile(userInfo.CustomerName, userInfo.Email, userInfo.Password, userInfo.FirstName, userInfo.LastName, userInfo.Phone, userInfo.RoleName);
        }

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