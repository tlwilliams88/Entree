using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.WebApi.Attribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Cors;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Impl.Helpers;
using System.IO;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Export;

namespace KeithLink.Svc.WebApi.Controllers
{
	[RequireHttps]
	[GlobalExceptionFilterAttribute]
	[AddCustomHeaderAttribute]
	public class BaseController : ApiController
    {
        #region attributes
        private IUserProfileLogic   _profileLogic;
        private UserProfile         _user;
        #endregion

        #region ctor
        public BaseController(IUserProfileLogic UserProfileLogic)
        {
            _profileLogic = UserProfileLogic;
        }
        #endregion

        #region methods
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            try
            {
                base.Initialize(controllerContext);

                if (controllerContext.RequestContext.Principal.Identity.IsAuthenticated &&
                    string.Compare(controllerContext.RequestContext.Principal.Identity.AuthenticationType, "bearer", true) == 0)
                {
                    UserProfileReturn retVal = _profileLogic.GetUserProfile(((ClaimsIdentity)this.ControllerContext.RequestContext.Principal.Identity).FindFirst("name").Value);

                    _user = retVal.UserProfiles[0];
                    _user.IsAuthenticated = true;

                    //TODO: add user's role
                    GenericPrincipal genPrincipal = new GenericPrincipal(_user, new string[] { "Owner" });
                    controllerContext.RequestContext.Principal = genPrincipal;

                    if (Request.Headers.Contains("userSelectedContext")) 
                    {
                        this.SelectedUserContext = JsonConvert.DeserializeObject<UserSelectedContext>(Request.Headers.GetValues("userSelectedContext").FirstOrDefault().ToString());

						if (!_user.IsInternalUser)//For now, don't verify internal users
						{
							//TODO: Need to update check now that customers are no longer included with the user profile
							//Verify that the authenticated user has access to this customer/branch
							bool isGuest = _user.RoleName.Equals(KeithLink.Svc.Core.Constants.ROLE_EXTERNAL_GUEST, StringComparison.InvariantCultureIgnoreCase);
							bool isCustomerSelected = (!string.IsNullOrEmpty(this.SelectedUserContext.CustomerId));

							var customer = _profileLogic.GetCustomerForUser(this.SelectedUserContext.CustomerId, this.SelectedUserContext.BranchId, _user.UserId);

							bool userHasAccessToCustomer = customer != null;

							if ((isGuest && isCustomerSelected) || (!isGuest && !userHasAccessToCustomer))
							{
								throw new Exception(string.Format("Authenticated user does not have access to passed CustomerId/Branch ({0}/{1})", this.SelectedUserContext.CustomerId, this.SelectedUserContext.BranchId));
							}
						}
                    }

                }
                else
                {
                    _user = null;
                }
            }
            catch (Exception ex)
            {
                string sEvent = "BEK Exception - BaseController:Initialize - " + ex.Message + ": " + ex.StackTrace;

                try
                {
                    IEventLogRepository eventLogRepository = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IEventLogRepository)) as IEventLogRepository;

                    eventLogRepository.WriteErrorLog("Unhandled API Exception", ex);
                }
                catch (Exception)
                {
                    string sSource = "BEK_Shop";
                    string sLog = "Application";

                    if (!EventLog.SourceExists(sSource))
                        EventLog.CreateEventSource(sSource, sLog);
                    EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Warning, 234);
                }

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("An unhandled exception has occured") });
            }
        }

		public HttpResponseMessage ExportModel<T>(List<T> model, ExportRequestModel exportRequest) where T : class, IExportableModel
		{
			var exportLogic = System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IModelExportLogic<T>)) as IModelExportLogic<T>;
                
			MemoryStream stream;
			if (exportRequest.Fields == null)
				stream = exportLogic.Export(model, exportRequest.SelectedType);// new ModelExporter<T>(model).Export(exportRequest.SelectedType);
			else
			{
				stream = exportLogic.Export(model,exportRequest.Fields,  exportRequest.SelectedType); //new ModelExporter<T>(model, exportRequest.Fields).Export(exportRequest.SelectedType);
			}
			HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			
			if(exportRequest.SelectedType.Equals("excel", StringComparison.CurrentCultureIgnoreCase))
				result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			else if(exportRequest.SelectedType.Equals("tab", StringComparison.CurrentCultureIgnoreCase))
				result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/tab-separated-values");
			else
				result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");

			return result;
		}

        #endregion

        #region properties
        public UserProfile AuthenticatedUser
        {
			get
			{
				if (!ControllerContext.RequestContext.Principal.Identity.IsAuthenticated)
					return null;
				else
					return (UserProfile)ControllerContext.RequestContext.Principal.Identity;
			}
        }

		public UserSelectedContext SelectedUserContext { get; set; }
		
        #endregion
    }
}