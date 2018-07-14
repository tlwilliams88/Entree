﻿using System;
using System.Runtime.Serialization;

namespace Entree.Core.SiteCatalog.Models
{
    [DataContract(Name="Price")]
    public class Price
    {
        #region properties
        [DataMember(Name="BranchId")]
        public string BranchId { get; set; }

        [DataMember(Name="CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name="ItemNumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name="CasePrice")]
        public double CasePrice { get; set; }

        [DataMember(Name="PackagePrice")]
        public double PackagePrice { get; set; }

        [DataMember(Name="DeviatedCost")]
        public bool DeviatedCost { get; set; }
        #endregion
    }
}
