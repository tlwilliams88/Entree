using Entree.Core.Lists.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Lists.Models
{
    [DataContract]
    public class PrintListModel
    {
        [DataMember(Name = "landscape")]
        public bool Landscape { get; set; }
        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }
        [DataMember(Name = "showparvalues")]
        public bool ShowParValues { get; set; }
        [DataMember(Name = "shownotes")]
        public bool ShowNotes { get; set; }
        [DataMember(Name = "showprices")]
        public bool ShowPrices { get; set; }
        [DataMember(Name = "filter")]
        public FilterInfo Filter { get; set; }
    }
}
