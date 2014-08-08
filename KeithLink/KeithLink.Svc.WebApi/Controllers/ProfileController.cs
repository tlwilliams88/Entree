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
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("profile/{emailAddress}")]
        public Core.Profile.UserProfileReturn GetUser(string emailAddress)
        {
            Impl.Profile.UserProfileRepository userRepo = new Impl.Profile.UserProfileRepository();

            return userRepo.GetUserProfile(emailAddress);
        }

        [HttpPost]
        [Route("profile/login/{emailAddress}/{password}")]
        public Core.Profile.UserProfileReturn Login(string emailAddress, string password)
        {
            Impl.Profile.UserProfileRepository userRepo = new Impl.Profile.UserProfileRepository();

            Core.Profile.UserProfileReturn retVal = null;

            if (userRepo.AuthenticateUser(emailAddress, password, out retVal))
            {
                return retVal;
            }
            else { 
                return null; 
            }
        }
    }
}