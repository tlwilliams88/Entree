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
        [Description("Item")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "caseprice", EmitDefaultValue = false)]
        [Description("Price")]
        public string CasePrice { get; set; }
        #endregion
    }
}