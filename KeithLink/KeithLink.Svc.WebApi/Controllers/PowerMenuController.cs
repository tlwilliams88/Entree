using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Orders;

using KeithLink.Svc.Core.Models.Orders.PowerMenu;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Paging;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Logic;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// PowerMenu interface controller
    /// </summary>
    [RoutePrefix("powermenu")]
    public class PowerMenuController : BaseController
    {

        #region attributes 
        private readonly IEventLogRepository _log;
        private readonly IOrderLogic _orderLogic;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="log"></param>
        public PowerMenuController(IUserProfileLogic profileLogic, IEventLogRepository log, IOrderLogic orderLogic) : base(profileLogic)
        {
            _log = log;
            _orderLogic = orderLogic;
        }

        /// <summary>
        /// Handler to import powermenu orders
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("order")]
        public HttpResponseMessage ImportPowerMenuOrder()
        {
            HttpResponseMessage returnValue = new HttpResponseMessage();

            var payload = this.Request.Content.ReadAsStringAsync().Result;
            payload = WebUtility.UrlDecode(payload.Replace("xml=", ""));

            XmlSerializer serializer = new XmlSerializer(typeof(VendorPurchaseOrderRequest));

            VendorPurchaseOrderRequest po;

            using (XmlReader rdr = XmlReader.Create(new StringReader(payload)))
            {
                po = (VendorPurchaseOrderRequest)serializer.Deserialize(rdr);
            }

            returnValue.Headers.Location = new Uri("http://shopqa.benekeith.com");

            returnValue.StatusCode = HttpStatusCode.OK;
            return returnValue;
        }

    }
}