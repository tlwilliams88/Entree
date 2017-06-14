﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.Notes
{
    public class NotesListDetail
    {
        public long Id { get; set; }
        public long ParentNotesHeaderId { get; set; }
        public string ItemNumber { get; set; }
        public bool? Each { get; set; }
        public string CatalogId { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
