using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	public class LoginModel
	{
		public string Email { get; set; }
		public string Password { get; set; }
	}

    public class ProfileController : ApiController
    {
        
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
            Impl.Profile.UserProfileRepository userRepo = new Impl.Profile.UserProfileRepository();

            Core.Profile.UserProfileReturn retVal = null;

			if (userRepo.AuthenticateUser(login.Email, login.Password, out retVal))
			{
				return retVal;
			}
			else
			{
				return null;
			}
        }
    }
}