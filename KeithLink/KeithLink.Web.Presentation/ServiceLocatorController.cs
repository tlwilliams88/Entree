using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using KeithLink.Web.Core;

namespace KeithLink.Web.Presentation
{
    public class ServiceLocatorController : ApiController
    {
        [Route("servicelocator")]
        [Route("angular-app/servicelocator")]
        public ConfigurationInformation GetServiceConfiguration()
        {
            string clientApiEndpoint = ConfigurationManager.AppSettings[Constants.ClientApiEndpointConfigurationEntry];
            if (System.Web.HttpContext.Current.Request.Browser.Browser.ToLower() == "ie8")
            {
                clientApiEndpoint = ConfigurationManager.AppSettings[Constants.ClientApiEndpointConfigurationEntry];
            }

            return new ConfigurationInformation() { ClientApiEndpoint = clientApiEndpoint };
        }
    }
}
