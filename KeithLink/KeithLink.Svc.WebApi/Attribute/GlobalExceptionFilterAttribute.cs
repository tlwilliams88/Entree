using KeithLink.Common.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace KeithLink.Svc.WebApi.Attribute
{
	public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute 
	{
		private IEventLogRepository eventLogRepository;

		
		public override void OnException(HttpActionExecutedContext actionExecutedContext)
		{
			eventLogRepository = System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(KeithLink.Common.Core.Logging.IEventLogRepository))
				as KeithLink.Common.Core.Logging.IEventLogRepository;
			
			eventLogRepository.WriteErrorLog("Unhandled API Exception", actionExecutedContext.Exception);
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("And unhandled exception has occured") });
		}
	}
}