using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name="category_image")]
    public class CategoryImage
    {
        #region properties
        
        [DataMember(Name="filename")]
        public string FileName { get; set; }

        [DataMember(Name="url")]
        public string Url { get; set; }

        [DataMember(Name="width")]
        public string Width { get; set; }

        [DataMember(Name="height")]
        public string Height { get; set; }

        #endregion
    }
}
