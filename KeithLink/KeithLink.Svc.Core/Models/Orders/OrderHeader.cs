using System;
using System.Text;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders
{
    [DataContract(Name="OrderHeader")]
    public class OrderHeader
    {
        #region methods
        public string ToString()
        {
            if (Branch.Length != 3) { throw new ArgumentException("Branch is an invalid format", "Branch"); }
            if (CustomerNumber.Length != 6) { throw new ArgumentException("CustomerNumber is in an invalid format", "CustomerNumber"); }

            StringBuilder output = new StringBuilder("H");

            switch (OrderingSystem)
            {
                case OrderSource.KeithCom:
                    output.Append("B");
                    break;
                default:
                    throw new ArgumentException("Unkown OrderingSystem", "OrderingSystem");
            }

            output.Append(Branch);
            output.Append(CustomerNumber);
            output.Append(DeliveryDate.ToString("yyyyMMdd"));
            output.Append(PONumber.PadRight(20));
            output.Append(Specialinstructions.PadRight(80));
            output.Append(ControlNumber.ToString().PadLeft(7, '0'));

            switch (OrderType)
            {
                case Orders.OrderType.NormalOrder:
                    output.Append(" ");
                    break;
                case Orders.OrderType.ChangeOrder:
                    output.Append("C");
                    break;
                case Orders.OrderType.DeleteOrder:
                    output.Append("D");
                    break;
                default:
                    throw new ArgumentException("Unknown OrderType", "OrderType");
            }

            output.Append(InvoiceNumber.PadLeft(8, '0'));
            output.Append(OrderCreateDateTime.ToString("yyyyMMddHHmmss"));
            output.Append(OrderSendDateTime.ToString("yyyyMMddHHmmss"));
            output.Append(UserId.PadRight(50));
            output.Append(OrderFilled ? "Y" : " ");
            output.Append(FutureOrder ? "Y" : " ");

            return output.ToString();
        }
        #endregion

        #region properties
        [DataMember(Name="OrderingSystem")]
        public OrderSource OrderingSystem { get; set; }

        [DataMember(Name = "Branch")]
        public string Branch { get; set; }

        [DataMember(Name = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "DeliveryDate")]
        public DateTime DeliveryDate { get; set; }

        [DataMember(Name = "PONumber")]
        public string PONumber { get; set; }

        [DataMember(Name = "Specialinstructions")]
        public string Specialinstructions { get; set; }

        [DataMember(Name = "ControlNumber")]
        public int ControlNumber { get; set; }

        [DataMember(Name = "OrderType")]
        public OrderType OrderType { get; set; }

        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "OrderCreateDateTime")]
        public DateTime OrderCreateDateTime { get; set; }

        [DataMember(Name = "OrderSendDateTime")]
        public DateTime OrderSendDateTime { get; set; }

        [DataMember(Name = "UserId")]
        public string UserId { get; set; }

        [DataMember(Name = "OrderFilled")]
        public bool OrderFilled { get; set; }

        [DataMember(Name = "FutureOrder")]
        public bool FutureOrder { get; set; }
        #endregion
    }
}
