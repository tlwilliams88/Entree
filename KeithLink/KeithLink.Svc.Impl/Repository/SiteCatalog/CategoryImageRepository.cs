using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class CategoryImageRepository : ICategoryImageRepository
    {
        #region attribute
        #endregion

        #region ctor
        #endregion

        #region methods/functions
        public CategoryImageReturn GetImageByCategory(string categoryId)
        {
            CategoryImageReturn returnValue = new CategoryImageReturn();

            using (HttpClient client = new HttpClient())
            {
                StringBuilder queryString = new StringBuilder("CategoryImage/Get/");
                queryString.Append(categoryId);

                if (Configuration.MultiDocsUrl.EndsWith("/") == false) { queryString.Insert(0, "/"); }

                string endPoint = string.Concat(Configuration.MultiDocsUrl, queryString);

                System.Net.Http.HttpResponseMessage response = client.GetAsync(endPoint).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    returnValue.CategoryImage = JsonConvert.DeserializeObject<CategoryImage>(response.Content.ReadAsStringAsync().Result);
                    if (returnValue.CategoryImage != null && !String.IsNullOrEmpty(returnValue.CategoryImage.Url))
                        returnValue.CategoryImage.Url.Replace("http://testmultidocs.bekco.com/", Configuration.MultiDocsUrl);
                }
            }

            return returnValue;
        }
        #endregion

        #region properties
        #endregion
    }
}
