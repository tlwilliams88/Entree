using KeithLink.Common.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using KeithLink.Svc.Core.Exceptions.Profile;
using KeithLink.Svc.WebApi.Controllers;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using Newtonsoft.Json;

namespace KeithLink.Svc.WebApi.Attribute
{
	public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute 
	{
		private IEventLogRepository eventLogRepository;


        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            eventLogRepository = System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(KeithLink.Common.Core.Logging.IEventLogRepository))
                as KeithLink.Common.Core.Logging.IEventLogRepository;

			
			var errorMessage = string.Empty;

			errorMessage = GenerateDetailedErrorMessage(actionExecutedContext);

			if(string.IsNullOrEmpty(errorMessage))
				errorMessage = "Unhandled API Exception";

			eventLogRepository.WriteErrorLog(errorMessage, actionExecutedContext.Exception);

            if (actionExecutedContext.Exception.GetType().Name == typeof(InvalidApiKeyException).Name || actionExecutedContext.Exception.GetType().Name == typeof(NoApiKeyProvidedException).Name)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("API Key Invalid or Missing") });
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("An unhandled exception has occured") });
            }
        }

		private static string GenerateDetailedErrorMessage(HttpActionExecutedContext actionExecutedContext)
		{
			var errorMessage = string.Empty;
			if (actionExecutedContext.ActionContext.ControllerContext.Controller is BaseController)
			{
				UserProfile user = null;
				UserSelectedContext context = null;
				user = ((BaseController)actionExecutedContext.ActionContext.ControllerContext.Controller).AuthenticatedUser;
				context = ((BaseController)actionExecutedContext.ActionContext.ControllerContext.Controller).SelectedUserContext;
				var controllerAction = string.Format("{0}Controller.{1}", actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName);

				errorMessage = string.Format("Unhandled API Exception: \r\nUser: {0} \r\nCustomer: {1} \r\nController.Action: {2} \r\nMethod: {3} \r\nArguments: \r\n{4}\r\n\r\n",
					user == null ? string.Empty : user.EmailAddress,
					context == null ? string.Empty : context.CustomerId,
					controllerAction,
					actionExecutedContext.ActionContext.Request.Method,
					actionExecutedContext.ActionContext.ActionArguments == null ? string.Empty : string.Join("\r\n", actionExecutedContext.ActionContext.ActionArguments.Select(a => string.Format("\tKey: {0} Value: {1}", a.Key, JsonConvert.SerializeObject(a.Value))).ToArray()));
			}
			return errorMessage;
		}
	}
}