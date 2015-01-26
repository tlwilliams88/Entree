using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.BEKWarmup.Models
{
	public class Messagingpreference
	{
		public string customerNumber { get; set; }
		public List<Preference> preferences { get; set; }
	}
}
