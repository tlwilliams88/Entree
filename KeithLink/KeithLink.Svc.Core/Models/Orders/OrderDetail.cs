using System;
using System.Text;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders
{
    [DataContract(Name="OrderDetail")]
    public class OrderDetail
    {
        #region methods
        public string ToString()
        {
            if (LineNumber == 0) { throw new ArgumentException("LineNumber has not been set", "LineNumber"); }
            if (ItemNumber.Length == 0) { throw new ArgumentException("ItemNumber has not been set", "ItemNumber"); }

            StringBuilder output = new StringBuilder("D");

            output.Append(ItemNumber);
            output.Append(OrderedQuantity.ToString().PadLeft(3, '0'));

            switch (UnitOfMeasure)
            {
                case Orders.UnitOfMeasure.Case:
                    output.Append(" ");
                    break;
                case Orders.UnitOfMeasure.Package:
                    output.Append("P");
                    break;
                default:
                    throw new ArgumentException("UnitOfMeasure has an unrecognized value", "UnitOfMeasure");
            }

            output.Append(Catchweight ? "Y" : " ");
            output.Append(LineNumber.ToString().PadLeft(5, '0'));

            switch(ItemChange){
                case LineType.NoChange:
                    output.Append(" ");
                    break;
                case LineType.Change:
                    output.Append("C");
                    break;
                case LineType.Delete:
                    output.Append("D");
                    break;
                case LineType.Add:
                    output.Append("A");
                    break;
                default:
                    throw new ArgumentException("ItemChange has an unrecognized value", "ItemChange");
            }

            output.Append(SubOriginalItemNumber.PadRight(6));
            output.Append(ReplacedOriginalItemNumber.PadRight(6));
            output.Append(ItemStatus.PadRight(1));

            return output.ToString();
        }
        #endregion
        
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
