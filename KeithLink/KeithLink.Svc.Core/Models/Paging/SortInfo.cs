using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Paging
{
	[DataContract]
	public class SortInfo
	{
		[DataMember(Name = "field")]
		public string Field { get; set; }
		[DataMember(Name = "order")]
		public string Order { get; set; }
		public SortOrder SortOrder { get { return ParseSortOrder(this.Order); } }


		public static SortOrder ParseSortOrder(string sortOrder)
		{
			switch (sortOrder.ToLower())
			{
				case "asc":
				case "ascending":
					return SortOrder.Ascending;

				case "desc":
				case "descending":
					return SortOrder.Descending;

				default:
					return SortOrder.Ascending;
			}
		}
	}

	public enum SortOrder
	{
		Ascending,
		Descending
	}
	
}
