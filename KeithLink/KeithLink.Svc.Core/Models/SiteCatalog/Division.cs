using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
	[DataContract]
	public class Division
	{
		[DataMember(Name="id")]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}
