using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Test
{
	public static class TestSessionObject
	{
		public static UserSelectedContext TestUserContext
		{
			get
			{
				return new UserSelectedContext() { CustomerId = "024418", BranchId = "FDF" };
			}
		}

		public static UserProfile TestAuthenticatedUser
		{
			get
			{
				return new UserProfile()
				{
					BranchId = null,
					CustomerNumber = "709333",
					DefaultCustomer = null,
					DSMNumber = "",
					DSMRole = "",
					DSRNumber = "",
					EmailAddress = "sabroussard@somecompany.com",
					FirstName = "Stephen",
					ImageUrl = "http://testmultidocs.bekco.com/avatar/be61fff2-64f0-439e-bb4f-323250dab145",
					IsAuthenticated = true,
					IsDemo = null,
					IsInternalUser = false,
					IsKBITCustomer = false,
					IsPowerMenuCustomer = false,
					LastName = "Broussard",
					PasswordExpired = false,
					PhoneNumber = null,
					PowerMenuGroupSetupUrl = "",
					PowerMenuLoginUrl = "http://bekpmwsq1.bekco.com/main/Logon.aspx?username=&password=be61&path=MAIN&customerlist=410300&order=true&framed=false&lang=ENG&country=USA",
					PowerMenuPermissionsUrl = "",
					RoleName = "owner",
					UserId = new Guid("be61fff2-64f0-439e-bb4f-323250dab145"),
					UserName = "sabroussard",
					UserNameToken = "c2Ficm91c3NhcmQtMjAxNTA0MjExMTUwMzY="
				};
			}
		}
		
	}
}
