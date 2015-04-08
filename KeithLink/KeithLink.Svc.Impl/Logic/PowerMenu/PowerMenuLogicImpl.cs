using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Repository.PowerMenu;
using KeithLink.Svc.Core.Models.PowerMenu;
using KeithLink.Svc.Core.Interface.PowerMenu;
using KeithLink.Svc.Core.Extensions.PowerMenu;

using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.Impl.Logic.PowerMenu {
    public class PowerMenuLogicImpl : IPowerMenuLogic {

        #region attributes
            IPowerMenuRepository _pmRepository;
            IUserProfileLogic _userProfile;
        #endregion


        public PowerMenuLogicImpl( IPowerMenuRepository powermenuRepository, IUserProfileLogic userProfile ) {
            _pmRepository = powermenuRepository;
            _userProfile = userProfile;
        }

        /// <summary>
        /// Serialize and send xml object to PowerMenu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool SendAccountRequestToPowerMenu( PowerMenuSystemRequestModel request ) {
            return _pmRepository.SendPowerMenuAccountRequests( request.ToXML() );
        }

        /// <summary>
        /// Send xml string to PowerMenu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool SendAccountRequestToPowerMenu( string emailAddress ) {
            bool returnValue = false;

            PowerMenuSystemRequestModel powerMenuRequest = new PowerMenuSystemRequestModel();

            UserProfileReturn userInfo = _userProfile.GetUserProfile( emailAddress, false );

            List<Customer> customers = _userProfile.GetCustomersForExternalUser( userInfo.UserProfiles[0].UserId );

            powerMenuRequest = (from customer in customers
                                 where customer.IsPowerMenu == true
                                 select new PowerMenuSystemRequestModel() {
                                     User = new PowerMenuSystemRequestUserModel() {
                                         Username = emailAddress,
                                         Password = String.Concat( customer.CustomerNumber, customer.CustomerBranch ),
                                         ContactName = customer.PointOfContact,
                                         CustomerNumber = customer.CustomerNumber,
                                         EmailAddress = customer.Email,
                                         PhoneNumber = customer.Phone,
                                         State = "TX" // does this need to be dynamic?
                                     },
                                     Login = new PowerMenuSystemRequestAdminModel() {
                                         AdminUsername = Configuration.PowerMenuAdminUsername,
                                         AdminPassword = Configuration.PowerMenuAdminPassword
                                     },
                                     Operation = PowerMenuSystemRequestModel.Operations.Add
                                 }).First();

            returnValue = SendAccountRequestToPowerMenu( powerMenuRequest );

            return returnValue;
        }
    }
}
