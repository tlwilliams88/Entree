using KeithLink.Svc.Core.Enumerations.List;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	

	public class List: BaseEFModel
	{
		public Guid? UserId { get; set; }
		public string DisplayName { get; set; }
		[Index]
		public ListType Type { get; set; }
		[Index("IX_CustBranch", 1, IsUnique=false)]
		[MaxLength(10)]
		public string CustomerId { get; set; }
		[Index("IX_CustBranch",2, IsUnique = false)]
		[MaxLength(10)]
		public string BranchId { get; set; }
		public string AccountNumber { get; set; }
		public bool ReadOnly { get; set; }
        
		public virtual ICollection<ListItem> Items { get; set; }
		public virtual ICollection<ListShare> Shares { get; set; }
	}
}
