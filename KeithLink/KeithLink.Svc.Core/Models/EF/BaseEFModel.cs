using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class BaseEFModel
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		[Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedUtc { get; set; }
		public DateTime ModifiedUtc { get; set; }
	}
}
