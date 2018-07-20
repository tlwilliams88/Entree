﻿using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.Lists {
    [DataContract(Name = "List")]
    public class ListModelShallowPrices {
        #region ctor
        public ListModelShallowPrices() {
            Items = new List<ListItemModelShallowPrice>();
        }
        #endregion


        #region properties
        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }

        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember]
		public ListType Type { get; set; }

        [DataMember(Name = "items")]
        public List<ListItemModelShallowPrice> Items { get; set; }

        #endregion
    }
}

