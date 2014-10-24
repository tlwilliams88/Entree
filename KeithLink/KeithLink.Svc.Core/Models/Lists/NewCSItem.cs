using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class NewCSItem
	{
		[DataMember(Name = "listitemid")]
		public Guid? Id { get; set; }
	}
}
