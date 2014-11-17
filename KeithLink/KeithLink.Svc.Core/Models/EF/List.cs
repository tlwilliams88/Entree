using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public enum ListType
	{
		Custom,
		Favorite,
		Contract,
		Recent,
		Notes,
		Worksheet,
        ContractItemsAdded,
        ContractItemsDeleted,
        Reminder,
		Mandatory
	}

	public class List: BaseEFModel
	{
		public Guid UserId { get; set; }
		public string DisplayName { get; set; }
		public ListType Type { get; set; }
		public string CustomerId { get; set; }
		public string BranchId { get; set; }
		public string AccountNumber { get; set; }
		public bool ReadOnly { get; set; }
        
		public virtual ICollection<ListItem> Items { get; set; }
		public virtual ICollection<ListShare> Shares { get; set; }
	}
}
