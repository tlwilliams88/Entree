using KeithLink.Svc.Core.Interface.Templates;
using KeithLink.Svc.Core.Models.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// TemplateController
    /// </summary>
    public class TemplateController : ApiController
    {
        #region attributes
        private ITemplatesRepository _templatesRepo;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="templatesRepo"></param>
        public TemplateController(ITemplatesRepository templatesRepo)
        {
            _templatesRepo = templatesRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Get a "static" template file
        /// </summary>
        [HttpPost]
        [ApiKeyedRoute("template")]
        public HttpResponseMessage Get(TemplateRequestModel trequest)
        {
            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(_templatesRepo.Get(trequest));
            result.Content.Headers.ContentDisposition = 
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            if(trequest.Format != null)
            {
                if (trequest.Format.Equals("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/csv");
                }
                // according to RFC-7231 do no send MIME type for unknown data
            }

            return result;
        }
        #endregion


    }
}
