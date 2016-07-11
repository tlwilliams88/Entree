using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.SiteCatalog;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class ProductImageRepositoryImpl : IProductImageRepository
    {
        #region methods

        public ProductImageReturn GetImageList(string itemNumber, bool isBekItem = true) {
            ProductImageReturn retVal = new ProductImageReturn();

            using (HttpClient client = new HttpClient()) {
                StringBuilder queryString = new StringBuilder("ItemImage/GetList/");

                if(!isBekItem)
                { // the UPC we have on file is a gtin12; the format the image provider saved the images as is gtin14
                    // so we prepend 00 on the front of the number
                    queryString.Append("00");
                    queryString.Append(itemNumber);
                    queryString.Append("?BEKItem=false");
                }
                else
                {
                    queryString.Append(itemNumber);

                }

                Uri multiDocsUri = new Uri(Configuration.MultiDocsUrl);
                Uri endPoint = new Uri (multiDocsUri, queryString.ToString());

                HttpResponseMessage response = client.GetAsync(endPoint).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    retVal.ProductImages = JsonConvert.DeserializeObject<List<ProductImage>>(response.Content.ReadAsStringAsync().Result);

                    if (retVal.ProductImages != null && !String.IsNullOrEmpty(Configuration.MultiDocsProxyUrl)) {
                        foreach(var pi in retVal.ProductImages) {
                            if(pi != null && !String.IsNullOrEmpty(pi.Url)) {
                                pi.Url = pi.Url.Replace(Configuration.MultiDocsUrl, Configuration.MultiDocsProxyUrl);
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        #endregion
    }
}
