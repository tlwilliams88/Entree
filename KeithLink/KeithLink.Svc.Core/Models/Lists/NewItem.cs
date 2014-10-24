using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class NewListItem
	{
		[DataMember(Name = "listitemid")]
		public long? Id { get; set; }
	}
}
