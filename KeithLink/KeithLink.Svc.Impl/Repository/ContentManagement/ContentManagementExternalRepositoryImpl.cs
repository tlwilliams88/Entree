using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine;
using KeithLink.Svc.Core.Interface.ContentManagement;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Impl.Repository.ContentManagement {
    public class ContentManagementExternalRepositoryImpl : IContentManagementExternalRepository {
        #region attributes
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public ContentManagementExternalRepositoryImpl(IEventLogRepository logRepo) {
            _log = logRepo;
        }
        #endregion

        #region methods
        public List<ContentItem> GetAllContent() {
            using (HttpClient client = new HttpClient()) {
                try {
                    HttpResponseMessage response = client.GetAsync(Configuration.MarketingContentUrl).Result;

                    if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK) || response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent)) {
                        using (System.IO.MemoryStream tempImgStream = new System.IO.MemoryStream()) {
                            string rawJson = System.Net.WebUtility.HtmlDecode(response.Content.ReadAsStringAsync().Result);
                            
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContentItem>>(rawJson);
                        }
                    } else {
                        throw new ApplicationException("Marketing content not found");
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error connecting to the Marketing Content server", ex);
                    throw;
                }
            }            
        }
        #endregion
    }
}
