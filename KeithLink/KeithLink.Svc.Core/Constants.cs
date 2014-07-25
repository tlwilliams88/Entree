using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public class Constants
    {
        public static string ReturnSizeQueryStringParam { get { return "size"; } }
        public static string ReturnFromQueryStringParam { get { return "from"; } }

        public static string CorsEnabledDomainsConfigurationEntry { get { return "CorsEnabledDomains"; } }
        public static string CorsEnabledHeadersConfigurationEntry { get { return "CorsEnabledHeaders"; } }
        public static string CorsEnabledMethodsConfigurationEntry { get { return "CorsEnabledMethods"; } }
        public static string ElasticSearchEndpointConfigurationEntry { get { return "ElasticSearchEndpoint"; } }
        public static string DefaultCategoryReturnSizeConfigurationEntry { get { return "DefaultCategoryReturnSize"; } }
        public static string DefaultProductReturnSizeConfigurationEntry { get { return "DefaultProductReturnSize"; } }
    }
}
