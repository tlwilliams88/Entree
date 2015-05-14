using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KeithLink.Svc.WebApi.Services {
    /// <summary>
    /// Summary description for PowerMenuPricingHandler
    /// </summary>
    public class PowerMenuPricingHandler : IHttpHandler {
        #region attributes
        private const string POWERMENU_NAMESPACE = "http://HawkeyeFoodservice.org/";

        private IDependencyScope _scope;
        #endregion

        #region ctor
        public PowerMenuPricingHandler() {
            _scope = GlobalConfiguration.Configuration.DependencyResolver.BeginScope();
        }
        #endregion

        #region methods
        private DateTime ConvertEffectiveDate(string effDateString) {
            if (effDateString.Length == 8) {
                int year = int.Parse(effDateString.Substring(0, 4));
                int month = int.Parse(effDateString.Substring(4, 2));
                int day = int.Parse(effDateString.Substring(6, 2));

                return new DateTime(year, month, day);
            } else {
                return new DateTime();
            }
        }

        public void ProcessRequest(HttpContext context) {
            // get data
            string xml = new StreamReader(context.Request.InputStream).ReadToEnd();

            XDocument doc = XDocument.Load(new StringReader(xml));
            XNamespace xns = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/");

            AuthHeader header = GetSoapHeader(doc.Descendants(xns + "Header").First().FirstNode);
            PricingRequest body = GetSoapBody(doc.Descendants(xns + "Body").First().FirstNode);

            // process pricing
            ProductReturn returnedPrices = new ProductReturn();

            IUserProfileLogic profileLogic = _scope.GetService(typeof(IUserProfileLogic)) as IUserProfileLogic;
            UserProfileReturn profileLogicReturn = profileLogic.GetUserProfile(header.UserName, false);

            if (profileLogicReturn.UserProfiles.Count > 0) {
                UserProfile profile = profileLogicReturn.UserProfiles[0];

                PagedResults<Customer> customers = profileLogic.CustomerSearch(profile, body.customerNumber, new Core.Models.Paging.PagingModel(), string.Empty);

                if (customers.TotalResults > 0) {
                    returnedPrices.Products.AddRange(GetItemPricing(customers.Results[0].CustomerBranch, body.customerNumber, body.products, ConvertEffectiveDate(body.effDate)));
                }
            }

            // return results
            SoapEnvelope soap = new SoapEnvelope();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            namespaces.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            soap.Body.Response.Results = returnedPrices;
            //XmlWriter writer = XmlWriter.Create(context.Response.OutputStream, new XmlWriterSettings() { Encoding = System.Text.Encoding.UTF8 });

            //XmlTypeMapping soapMapping = new SoapReflectionImporter().ImportTypeMapping(retVal.GetType());
            //XmlTypeMapping soapMapping = new SoapReflectionImporter().ImportTypeMapping(typeof(ProductReturn));
            //XmlSerializer serializer = new XmlSerializer(typeof(PricingResponse));
            XmlSerializer serializer = new XmlSerializer(typeof(SoapEnvelope));
            //XmlSerializer serializer = new XmlSerializer(soapMapping);

            context.Response.ContentType = "text/xml";
            //serializer.Serialize(context.Response.OutputStream, returnedPrices);
            serializer.Serialize(context.Response.OutputStream, soap, namespaces);
            //writer.WriteStartElement("GetProductsWithPriceResponse", "http://benekeith.com");
            //serializer.Serialize(writer, retVal);
            //return retVal;
        }

        private List<Core.Models.SiteCatalog.Pricing.PowerMenu.Product> GetItemPricing(string branchId, string customerNumber, List<ProductLine> products,
                                                                                                                        DateTime effectiveDate) {
            List<Core.Models.SiteCatalog.Pricing.PowerMenu.Product> retVal = new List<Core.Models.SiteCatalog.Pricing.PowerMenu.Product>();

            List<Core.Models.SiteCatalog.Product> productList = (from ProductLine p in products
                                                                 select new Core.Models.SiteCatalog.Product { ItemNumber = p.ProductNumber }).ToList();

            IPriceLogic priceLogic = _scope.GetService(typeof(IPriceLogic)) as IPriceLogic;
            PriceReturn prices = priceLogic.GetPrices(branchId, customerNumber, effectiveDate, productList);

            foreach (Price price in prices.Prices) {
                KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu.Product currentItem = new KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu.Product();

                currentItem.ProductNumber = price.ItemNumber;
                currentItem.IsAuthorized = (price.CasePrice > 0 || price.PackagePrice > 0);
                currentItem.IsActive = true;
                currentItem.AvailableQty = 0;

                ProductLine myProduct = (from ProductLine p in products
                                         where p.ProductNumber == currentItem.ProductNumber
                                         select p).FirstOrDefault();
                //if (item.CatchWeight) {
                //    currentItem.Price = decimal.Parse(item.CasePrice);
                //    currentItem.PurchaseByUnit = "Lb";
                if (myProduct.Unit.Length == 0 || myProduct.Unit.Equals("case", StringComparison.InvariantCultureIgnoreCase)) {
                    currentItem.Price = (decimal)price.CasePrice;
                    currentItem.PurchaseByUnit = "cs";
                } else if (myProduct.Unit.Equals("each", StringComparison.InvariantCultureIgnoreCase)) {
                    currentItem.Price = (decimal)price.PackagePrice;
                    currentItem.PurchaseByUnit = "ea";
                } else {
                    currentItem.Price = 0;
                    currentItem.PurchaseByUnit = string.Empty;
                }

                retVal.Add(currentItem);
            }

            return retVal;
        }

        private PricingRequest GetSoapBody(XNode bodyNode) {
            XmlSerializer serializer = new XmlSerializer(typeof(PricingRequest), new XmlRootAttribute("GetProductsWithPrice") { Namespace = POWERMENU_NAMESPACE });
            return (PricingRequest)serializer.Deserialize(new StringReader(bodyNode.ToString()));
        }

        private AuthHeader GetSoapHeader(XNode headerNode) {
            XmlSerializer serializer = new XmlSerializer(typeof(AuthHeader), new XmlRootAttribute("AuthHeader") { Namespace = POWERMENU_NAMESPACE });
            return (AuthHeader)serializer.Deserialize(new StringReader(headerNode.ToString()));
        }
        #endregion

        #region properties
        public bool IsReusable {
            get {
                return false;
            }
        }
        #endregion
    }
}