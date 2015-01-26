using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.BEKWarmup.Models
{
	class PagedCustomerResponse
	{
		public int totalResults { get; set; }
		public List<UserCustomer> results { get; set; }
	}
		
}
