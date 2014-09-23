using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.WebApi.Models;
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
        private KeithLink.Common.Core.Logging.IEventLogRepository _log;
        #endregion

        #region ctor
        public ProfileController(Core.Interface.Profile.ICustomerContainerRepository customerRepo, 
                                 Core.Interface.Profile.IUserProfileRepository profileRepo,
                                 KeithLink.Common.Core.Logging.IEventLogRepository logRepo ) : base(profileRepo) {
            _custRepo = customerRepo;
            _profileRepo = profileRepo;
            _log = logRepo;
        }
        #endregion

        #region methods
        [AllowAnonymous]
        [HttpPost]
        [ApiKeyedRoute("profile/create")]
        public OperationReturnModel<UserProfileReturn> CreateUser(UserProfileModel userInfo)
        {

            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try
            {
                CustomerContainerReturn custExists = _custRepo.SearchCustomerContainers(userInfo.CustomerName);

                // create the customer container if it does not exist
                if (custExists.CustomerContainers.Count != 1) { _custRepo.CreateCustomerContainer(userInfo.CustomerName); }

                retVal.SuccessResponse =_profileRepo.CreateUserProfile(userInfo.CustomerName, userInfo.Email, userInfo.Password, 
                                                                       userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, 
                                                                       userInfo.RoleName);
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }


            return retVal;
        }

        [AllowAnonymous]
        [HttpPost]
        [ApiKeyedRoute("profile/register")]
        public OperationReturnModel<UserProfileReturn> CreateGuest(GuestProfileModel guestInfo) {

            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                CustomerContainerReturn custExists = _custRepo.SearchCustomerContainers(KeithLink.Svc.Core.Constants.AD_GUEST_CONTAINER);

                // create the customer container if it does not exist
                if (custExists.CustomerContainers.Count != 1) { _custRepo.CreateCustomerContainer(KeithLink.Svc.Core.Constants.AD_GUEST_CONTAINER); }

                retVal.SuccessResponse = _profileRepo.CreateGuestProfile(guestInfo.Email, guestInfo.Password, guestInfo.BranchId);
            } catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("profile")]
        public UserProfileReturn GetUser(string email)
        {
            if (string.Compare(email, AuthenticatedUser.EmailAddress, true) == 0)
            {
                UserProfileReturn retVal = new UserProfileReturn();
                retVal.UserProfiles.Add(this.AuthenticatedUser);

                return retVal;
            }
            else
            {
                return _profileRepo.GetUserProfile(email);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [ApiKeyedRoute("profile/searchcustomer/{searchText}")]
        public CustomerContainerReturn SearchCustomers(string searchText)
        {
            return _custRepo.SearchCustomerContainers(searchText);
        }

        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile/password")]
        public OperationReturnModel<string> UpdatePassword(UpdatePasswordModel pwInfo) {
            return new OperationReturnModel<string> {
                SuccessResponse = _profileRepo.UpdateUserPassword(pwInfo.Email, pwInfo.OriginalPassword, pwInfo.NewPassword)
            };
        }

        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("profile")]
        public OperationReturnModel<UserProfileReturn> UpdateUser(UserProfileModel userInfo) {
            OperationReturnModel<UserProfileReturn> retVal = new OperationReturnModel<UserProfileReturn>();

            try {
                if (userInfo.UserId == null || userInfo.UserId.Length == 0) { userInfo.UserId = this.AuthenticatedUser.UserId.ToString("B"); }

                _profileRepo.UpdateUserProfile(userInfo.UserId.ToGuid(), userInfo.Email, userInfo.FirstName, 
                                               userInfo.LastName, userInfo.PhoneNumber, userInfo.BranchId);

                retVal.SuccessResponse = _profileRepo.GetUserProfile(userInfo.Email);
            } catch (ApplicationException axe) {
                retVal.ErrorMessage = axe.Message;

                _log.WriteErrorLog("Application exception", axe);
            } catch (Exception ex) {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;

                _log.WriteErrorLog("Unhandled exception", ex);
            }


            return retVal;
        }
        #endregion
    }
}