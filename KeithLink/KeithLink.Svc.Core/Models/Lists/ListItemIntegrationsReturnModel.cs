// KeithLink
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;

// Core
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Spreadsheet;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "ListItem")]
    public class ListItemIntegrationsReturnModel
    {
        #region properties
        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Name = "brand", EmitDefaultValue = false)]
        public string Brand { get; set; }

        [DataMember(Name = "caseprice", EmitDefaultValue = false)]
        public string CasePrice { get; set; }

        [DataMember(Name = "upc", EmitDefaultValue = false)]
        public string UPC { get; set; }

        [DataMember(Name = "size", EmitDefaultValue = false)]
        public string Size { get; set; }

        [DataMember(Name = "pack", EmitDefaultValue = false)]
        public string Pack { get; set; }
        #endregion
    }
}