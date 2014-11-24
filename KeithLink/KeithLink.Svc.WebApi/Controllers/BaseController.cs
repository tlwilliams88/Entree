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

namespace KeithLink.Svc.WebApi.Controllers
{
	[RequireHttps]
	[GlobalExceptionFilterAttribute]
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

                        //Verify that the authenticated user has access to this customer/branch
                        bool isGuest = _user.RoleName.Equals(KeithLink.Svc.Core.Constants.ROLE_EXTERNAL_GUEST, StringComparison.InvariantCultureIgnoreCase);
                        bool isCustomerSelected = (!string.IsNullOrEmpty(this.SelectedUserContext.CustomerId));
                        bool userHasAccessToCustomer = (_user.UserCustomers != null &&
                            _user.UserCustomers.Where(c => c.CustomerBranch.Equals(this.SelectedUserContext.BranchId, StringComparison.InvariantCultureIgnoreCase)
                                && c.CustomerNumber.Equals(this.SelectedUserContext.CustomerId)).Any());

                        if ((isGuest && isCustomerSelected) || (!isGuest && !userHasAccessToCustomer))
                        {
                            throw new Exception(string.Format("Authenticated user does not have access to passed CustomerId/Branch ({0}/{1})", this.SelectedUserContext.CustomerId, this.SelectedUserContext.BranchId));
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