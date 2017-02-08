using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Helpers;
using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.History {
    [DataContract()]
    public class OrderHistoryDetail {
        #region properties
        [DataMember()]
        public int LineNumber { get; set; }

        [DataMember(Name = "linetotal")]
        public double LineTotal
        {
            get
            {
                double total = 0;

                if (this.ItemDeleted == false
                    && this.ItemStatus.Equals(Constants.CONFIRMATION_DETAIL_OUT_OF_STOCK_CODE,
                                                       StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    total = PricingHelper.GetFixedPrice(ShippedQuantity, SellPrice, CatchWeight, TotalShippedWeight, AverageWeight);
                }

                return total;
            }
            set { }
        }
        [DataMember()]
        public string ItemNumber { get; set; }

        [DataMember()]
        public int OrderQuantity { get; set; }

        [DataMember()]
        public int ShippedQuantity { get; set; }

        [DataMember()]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [DataMember()]
        public decimal UnitCost { get; set; }

        [DataMember()]
        public double SellPrice { get; set; }

        [DataMember()]
        public bool CatchWeight { get; set; }

        [DataMember()]
        public bool ItemDeleted { get; set; }

        [DataMember()]
        public string SubbedOriginalItemNumber { get; set; }

        [DataMember()]
        public string ReplacedOriginalItemNumber { get; set; }

        [DataMember()]
        public string ItemStatus { get; set; }

        [DataMember()]
        public bool FutureItem { get; set; }

        [DataMember()]
        public double TotalShippedWeight { get; set; }

        [DataMember()]
        public double AverageWeight { get; set; }

        [DataMember()]
        public string Source { get; set; }

        [DataMember()]
        public string ManufacturerId { get; set; }

        [DataMember()]
        public string SpecialOrderHeaderId { get; set; }

        [DataMember()]
        public string SpecialOrderLineNumber { get; set; }
        #endregion
    }
}
