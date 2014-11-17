using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.ContentManagement
{
    [DataContract]
    public class ContentItemPostModel : ContentItemModelBase
    {
        [DataMember(Name = "imagedata")]
        public string Base64ImageData { get; set; }
        [DataMember(Name = "imagefilename")]
        public string ImageFileName { get; set; }
    }
}
