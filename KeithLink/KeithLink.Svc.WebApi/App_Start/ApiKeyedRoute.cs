using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi
{
    internal class ApiKeyedRoute : System.Web.Http.Routing.RouteFactoryAttribute
    {
        public ApiKeyedRoute(string template)
            : base(template)
        {
            AllowedApiKeys = Svc.Impl.Configuration.AllowedApiKeys;
        }
        public List<string> AllowedApiKeys
        {
            get;
            private set;
        }
        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new System.Web.Http.Routing.HttpRouteValueDictionary();
                constraints.Add("apikey", new ApiKeyedRouteConstraint(AllowedApiKeys));
                return constraints;
            }
        }
    }
}