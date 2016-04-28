using KeithLink.Common.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class ProductImageRepositoryImpl : KeithLink.Svc.Core.Interface.SiteCatalog.IProductImageRepository
    {
        #region methods

        public KeithLink.Svc.Core.Models.SiteCatalog.ProductImageReturn GetImageList(string itemNumber)
        {
            KeithLink.Svc.Core.Models.SiteCatalog.ProductImageReturn retVal = new Core.Models.SiteCatalog.ProductImageReturn();

            using (HttpClient client = new HttpClient())
            {
                string endPoint = Configuration.MultiDocsBEKImageListEndpoint.Inject(new
                {
                    baseUrl = Configuration.MultiDocsUrl,
                    ItemNumber = itemNumber
                });

                System.Net.Http.HttpResponseMessage response = client.GetAsync(endPoint).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    retVal.ProductImages = JsonConvert.DeserializeObject<List<KeithLink.Svc.Core.Models.SiteCatalog.ProductImage>>(response.Content.ReadAsStringAsync().Result);
                    if (retVal.ProductImages != null && !String.IsNullOrEmpty(Configuration.MultiDocsProxyUrl))
                        foreach (var pi in retVal.ProductImages)
                        if (pi != null && !String.IsNullOrEmpty(pi.Url))
                            pi.Url = pi.Url.Replace(Configuration.MultiDocsUrl, Configuration.MultiDocsProxyUrl);
                }
            }

            return retVal;
        }
        public KeithLink.Svc.Core.Models.SiteCatalog.ProductImageReturn GetNonBEKImageList(string UPC)
        {
            KeithLink.Svc.Core.Models.SiteCatalog.ProductImageReturn retVal = new Core.Models.SiteCatalog.ProductImageReturn();

            using (HttpClient client = new HttpClient())
            {
                string endPoint = Configuration.MultiDocsNonBEKImageListEndpoint.Inject(new {
                    baseUrl = Configuration.MultiDocsUrl,
                    UPC = UPC
                });


                System.Net.Http.HttpResponseMessage response = client.GetAsync(endPoint).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    retVal.ProductImages = JsonConvert.DeserializeObject<List<KeithLink.Svc.Core.Models.SiteCatalog.ProductImage>>(response.Content.ReadAsStringAsync().Result);
                    if (retVal.ProductImages != null && !String.IsNullOrEmpty(Configuration.MultiDocsProxyUrl))
                        foreach (var pi in retVal.ProductImages)
                            if (pi != null && !String.IsNullOrEmpty(pi.Url))
                                pi.Url = pi.Url.Replace(Configuration.MultiDocsUrl, Configuration.MultiDocsProxyUrl);
                }
            }

            return retVal;
        }

        #endregion
    }
}
