using KeithLink.Svc.Core.Enumerations.List;
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
    public class ListModelIntegrationsReturnModel
    {
        #region ctor
        public ListModelIntegrationsReturnModel() {
            Items = new List<ListItemIntegrationsReturnModel>();
        }
        #endregion


        #region properties
        [DataMember(Name = "listid")]
        public long ListId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
		public ListType Type { get; set; }

        [DataMember(Name = "items")]
        public List<ListItemIntegrationsReturnModel> Items { get; set; }

        #endregion
    }
}

