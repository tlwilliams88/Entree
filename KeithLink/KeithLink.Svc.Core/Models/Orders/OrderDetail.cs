using System;
using System.Text;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Enumerations.Order;

namespace KeithLink.Svc.Core.Models.Orders
{
    [DataContract(Name="OrderDetail")]
    public class OrderDetail
    {
        #region properties
        [DataMember(Name="ItemNumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name="OrderedQuantity")]
        public Int16 OrderedQuantity { get; set; }

        [DataMember(Name="UnitOfMeasure")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [DataMember(Name = "SellPrice")]
        public double SellPrice { get; set; }

        [DataMember(Name = "Catchweight")]
        public bool Catchweight { get; set; }

        [DataMember(Name = "LineNumber")]
        public Int16 LineNumber { get; set; }

        [DataMember(Name = "ItemChange")]
        public LineType ItemChange { get; set; }

        [DataMember(Name = "SubOriginalItemNumber")]
        public string SubOriginalItemNumber { get; set; }

        [DataMember(Name = "ReplacedOriginalItemNumber")]
        public string ReplacedOriginalItemNumber { get; set; }

        [DataMember(Name = "ShippedQuantity")]
        public Int16 ShippedQuantity { get; set; }

        [DataMember(Name = "ItemStatus")]
        public string ItemStatus { get; set; }

        #endregion
    }
}
