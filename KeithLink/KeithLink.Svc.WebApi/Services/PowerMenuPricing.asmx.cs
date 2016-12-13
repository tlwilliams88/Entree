﻿using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace KeithLink.Svc.WebApi.Services {
    /// <summary>
    /// 
    /// </summary>
    public class AuthHeader : SoapHeader {
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }

    // per Josh's request, we are making the namespace match that of the request from PowerMenu
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://HawkeyeFoodservice.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class PowerMenuPricing : System.Web.Services.WebService {
        #region attributes
        /// <summary>
        /// 
        /// </summary>
        public AuthHeader header;
        /// <summary>
        /// 
        /// </summary>
        public SoapUnknownHeader[] unknownHeaders;

        private IDependencyScope _scope;
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        public PowerMenuPricing() {
            _scope = GlobalConfiguration.Configuration.DependencyResolver.BeginScope();
        }
        #endregion

        #region methods
        /// <summary>
        /// Get prices for all of the products
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <param name="products"></param>
        /// <param name="effDate"></param>
        /// <param name="getAllFields"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("unknownHeaders")]
        [SoapHeader("header")]
        public ProductReturn GetProductsWithPrice(string customerNumber, ProductLine[] products, DateTime effDate, 
                                                                        bool getAllFields) {
            if (header == null) { header = LoadHeader(unknownHeaders[0]); }

            ProductReturn retVal = new ProductReturn();

            IUserProfileLogic profileLogic = _scope.GetService(typeof(IUserProfileLogic)) as IUserProfileLogic;
            UserProfileReturn profileLogicReturn = profileLogic.GetUserProfile(header.UserName, false);

            if (profileLogicReturn.UserProfiles.Count > 0) {
                UserProfile profile = profileLogicReturn.UserProfiles[0];

                PagedResults<Customer> customers = profileLogic.CustomerSearch(profile, customerNumber, new Core.Models.Paging.PagingModel(), string.Empty, CustomerSearchType.Customer);

                if (customers.TotalResults > 0) {
                    retVal.Products.AddRange(GetItemPricing(customers.Results[0].CustomerBranch, customerNumber, products, effDate));
                }
            }

            return retVal;
        }

        private List<Core.Models.SiteCatalog.Pricing.PowerMenu.Product> GetItemPricing(string branchId, string customerNumber, ProductLine[] products, 
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
                currentItem.IsCatchWeight = false;

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

        private AuthHeader LoadHeader(SoapUnknownHeader authHeader) {
            AuthHeader header = new AuthHeader();

            header.UserName = authHeader.Element.ChildNodes[1].InnerText;
            header.Password = authHeader.Element.ChildNodes[3].InnerText;

            return header;
        }
        #endregion
    }


}
