using KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace KeithLink.Svc.WebApi.Services {
    /// <summary>
    /// Summary description for PowerMenuPricingHandler
    /// </summary>
    public class PowerMenuPricingHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            // get data
            System.IO.StreamReader inputStream = new System.IO.StreamReader(context.Request.InputStream);
            string xml = inputStream.ReadToEnd();

            // parse header
            XmlSerializer serializer = new XmlSerializer(typeof(AuthHeader), new XmlRootAttribute("AuthHeader"));
            AuthHeader header = (AuthHeader)serializer.Deserialize(inputStream);

            // parse payload

            // process pricing

            // return results
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}