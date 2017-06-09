using System;

namespace KeithLink.Svc.Core.Models.Lists.ReminderItems
{ 
    public class ReminderItemsListHeader
    {
        public long Id { get; set; }
        public string BranchId { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
