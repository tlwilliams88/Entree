using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.Notes
{ 
    public class NotesListHeader
{
        public long Id { get; set; }
        public string CustomerNumber { get; set; }
        public string BranchId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
        public List<NotesListDetail> Items { get; set; }
    }
}
