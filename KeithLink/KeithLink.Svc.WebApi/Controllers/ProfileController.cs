﻿using KeithLink.Svc.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	

    public class ProfileController : BaseController
    {
        #region attributes
        private Core.Interface.Profile.ICustomerContainerRepository _custRepo;
        private Core.Interface.Profile.IUserProfileRepository _profileRepo;
        #endregion

        #region ctor
        public ProfileController(Core.Interface.Profile.ICustomerContainerRepository customerRepo, Core.Interface.Profile.IUserProfileRepository profileRepo) : base(profileRepo) {
            _custRepo = customerRepo;
            _profileRepo = profileRepo;
        }
        #endregion

        #region methods
        [AllowAnonymous]
        [HttpPost]
        [Route("profile/create")]
        public OperationReturnModel<Core.Models.Profile.UserProfileReturn> CreateUser(UserProfileModel userInfo)
        {

            OperationReturnModel<Core.Models.Profile.UserProfileReturn> retVal = new OperationReturnModel<Core.Models.Profile.UserProfileReturn>();

            try
            {
                Core.Models.Profile.CustomerContainerReturn custExists = _custRepo.SearchCustomerContainers(userInfo.CustomerName);

                // create the customer container if it does not exist
                if (custExists.CustomerContainers.Count != 1) { _custRepo.CreateCustomerContainer(userInfo.CustomerName); }

                retVal.SuccessResponse =_profileRepo.CreateUserProfile(userInfo.CustomerName, userInfo.Email, userInfo.Password, 
                                                                       userInfo.FirstName, userInfo.LastName, userInfo.Phone, 
                                                                       userInfo.RoleName);
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
            }


            return retVal;
        }

        [Authorize]
        [HttpGet]
        [Route("profile")]
        public Core.Models.Profile.UserProfileReturn GetUser(string emailAddress)
        {
            Impl.Profile.UserProfileRepository userRepo = new Impl.Profile.UserProfileRepository();

            return userRepo.GetUserProfile(emailAddress);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("profile/login")]
        public Core.Models.Profile.UserProfileReturn Login(LoginModel login)
        {
            Core.Models.Profile.UserProfileReturn retVal = null;

			if (_profileRepo.AuthenticateUser(login.Email, login.Password, out retVal))
			{
				return retVal;
			}
			else
			{
				return null;
			}
        }

        [Authorize()]
        [HttpGet]
        [Route("profile/searchcustomer/{searchText}")]
        public Core.Models.Profile.CustomerContainerReturn SearchCustomers(string searchText)
        {
            return _custRepo.SearchCustomerContainers(searchText);
        }
        #endregion
    }
}