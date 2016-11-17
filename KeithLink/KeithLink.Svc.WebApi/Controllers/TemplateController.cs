using KeithLink.Svc.Core.Interface.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class TemplateController : ApiController
    {
        #region attributes
        private ITemplatesRepository _templatesRepo;
        #endregion

        #region ctor
        public TemplateController(ITemplatesRepository templatesRepo)
        {
            _templatesRepo = templatesRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Get a "static" template file
        /// </summary>
        /// <param name="name">the name of the template</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("template/{name}")]
        public HttpResponseMessage Get(string name)
        {
            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(_templatesRepo.Get(name));
            result.Content.Headers.ContentDisposition = 
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");

            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");

            return result;
        }
        #endregion


    }
}
