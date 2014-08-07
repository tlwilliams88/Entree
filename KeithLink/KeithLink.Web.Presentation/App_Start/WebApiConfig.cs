using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KeithLink.Web.Presentation
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            // force JSON responses
            config.Formatters.Clear();
            config.Formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());
        }
    }
}
