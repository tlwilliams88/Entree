﻿using System;

namespace KeithLink.Svc.Core.Models.Lists.ReminderItems
{
    public class ReminderItemsListDetail
    {
        public long Id { get; set; }
        public long ParentRemindersHeaderId { get; set; }
        public string ItemNumber { get; set; }
        public bool? Each { get; set; }
        public string CatalogId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
