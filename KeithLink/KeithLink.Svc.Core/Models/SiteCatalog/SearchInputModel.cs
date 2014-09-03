using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "searchinputmodel")]
    public class SearchInputModel
    {
        public SearchInputModel()
        {
            Facets = string.Empty;
        }

        [DataMember(Name = "from")]
        public int From { get; set; }
        [DataMember(Name = "size")]
        public int Size { get; set; }
        [DataMember(Name = "facets")]
        public string Facets { get; set; }
        [DataMember(Name = "sfield")]
        public string SField { get; set; }
        [DataMember(Name = "sdir")]
        public string SDir { get; set; }
    }
}