using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.ContentManagement
{
    [DataContract]
    public class ContentItemViewModel
    {
        [DataMember(Name="imageurl")]
        public Int64 ImageUrl { get; set; }
    }
}
