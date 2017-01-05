using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Cart;

using KeithLink.Svc.Core.Models.PowerMenu.Order;

using KeithLink.Svc.Core.Extensions.PowerMenu;

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
        private readonly IShoppingCartService _cartService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="log"></param>
        public PowerMenuController(IUserProfileLogic profileLogic, IEventLogRepository log,
            IShoppingCartService cartService) : base(profileLogic)
        {
            _log = log;
            _cartService = cartService;
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

            Guid newCart = _cartService.ImportFromPowerMenu(po);

            string redirectTo = string.Format("{0}/#/cart/{1}", KeithLink.Svc.Impl.Configuration.PresentationUrl, newCart);
            returnValue.Headers.Location = new Uri(redirectTo);
            returnValue.StatusCode = HttpStatusCode.Redirect;

            return returnValue;
        }
    }
}