using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.BEKWarmup.Models
{
	public class Preference
	{
		public int notificationType { get; set; }
		public string description { get; set; }
		public List<object> selectedChannels { get; set; }
	}
}
