using System;
using System.Text;

namespace KeithLink.Svc.Core.Models.Orders
{
    class OrderDetail
    {
        #region properties
        public string ItemNumber { get; set; }
        public Int16 OrderedQuantity { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public double SellPrice { get; set; }
        public bool Catchweight { get; set; }
        public Int16 LineNumber { get; set; }
        public LineType ItemChange { get; set; }
        public string SubOriginalItemNumber { get; set; }
        public string ReplacedOriginalItemNumber { get; set; }
        public Int16 ShippedQuantity { get; set; }
        public string ItemStatus { get; set; }
        #endregion
    }
}
