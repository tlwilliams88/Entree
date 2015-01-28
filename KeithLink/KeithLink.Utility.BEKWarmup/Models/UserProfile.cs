using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.BEKWarmup.Models
{
	public class UserProfile
	{
		public object branchid { get; set; }
		public string customernumber { get; set; }
		public string emailaddress { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public object phonenumber { get; set; }
		public string rolename { get; set; }
		public string userid { get; set; }
		public List<UserCustomer> user_customers { get; set; }
		public List<Messagingpreference> messagingpreferences { get; set; }
	}
}
