using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace KeithLink.Svc.WebApi
{
    internal class ApiKeyedRouteConstraint : System.Web.Http.Routing.IHttpRouteConstraint
    {
        public const string ApiKeyHeaderName = "apiKey";
        private const string DefaultApiKey = "";

        public ApiKeyedRouteConstraint(List<string> allowedHeaderKeys)
        {
            AllowedHeaderKeys = allowedHeaderKeys;
        }

        public List<string> AllowedHeaderKeys
        {
            get;
            private set;
        }

        public bool Match(HttpRequestMessage request, System.Web.Http.Routing.IHttpRoute route, string parameterName, IDictionary<string, object> values, System.Web.Http.Routing.HttpRouteDirection routeDirection)
        {
            if (routeDirection == System.Web.Http.Routing.HttpRouteDirection.UriResolution)
            {
                string version = GetApiKey(request) ?? DefaultApiKey;
                if (!AllowedHeaderKeys.Contains(version))
                {
                    throw new Core.Exceptions.Profile.InvalidApiKeyException("Invalid API Key Detected");
                }
            }
            return true;
        }

        private string GetApiKey(HttpRequestMessage request)
        {
            string headerApiKey;
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues(ApiKeyHeaderName, out headerValues) && headerValues.Count() == 1)
            {
                headerApiKey = headerValues.First();
                return headerApiKey;
            }

            throw new Core.Exceptions.Profile.NoApiKeyProvidedException("No API Key Provided");
        }
    }
}