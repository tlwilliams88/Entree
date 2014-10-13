using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceServer.Foundation.SequenceComponents;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class UserOrganizationsCommitter : ProfileCommitterBase
    {
        protected override string ProfileModelName
        {
            get { return "UserOrganizations"; }
        }

        public UserOrganizationsCommitter()
        {
        }
    }
}
