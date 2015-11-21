using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract(Name = "OrderLine")]
	public class OrderLine : BaseProductInfo, IExportableModel {
        [DataMember(Name = "linenumber")]
        public int LineNumber { get; set; }

		[DataMember(Name = "linetotal")]
        public double LineTotal {
            get
            {
				if (this.IsDeleted)
					return 0;

                if (this.CatchWeight)
                {
					if (!string.IsNullOrEmpty(this.OrderStatus) && this.OrderStatus.Equals("i", StringComparison.CurrentCultureIgnoreCase) && this.TotalShippedWeight > 0)
						return (double)this.TotalShippedWeight * this.Price;
					else
					{
						if (this.Each) //package catchweight
						{
                            return PricingHelper.GetCatchweightPriceForPackage(QantityShipped, int.Parse(Pack), AverageWeight, Price);
						}
						else //case catchweight
						{
                            return PricingHelper.GetCatchweightPriceForCase(QantityShipped, AverageWeight, Price);
						}
					}
                }
                else
                {
					return this.QantityShipped * this.Price; 
                }
            }
            set { } 
        }

		private int _quantity;
		[DataMember(Name = "quantity")]
		[Description("# Requested")]
		public int Quantity
		{
			get { if (IsDeleted) return 0; return _quantity; }
			set { _quantity = value; }
		}
				
        //[DataMember(Name = "packsize")]
        //public string PackSize { get; set; }
		
		[DataMember(Name = "each")]
		public bool Each { get; set; }

		[DataMember(Name = "eachyn")]
		[Description("Each")]
        public string EachYN { get { return this.Each ? "Y" : "N"; } set { } }
		
		[DataMember(Name = "storagetemp")]
		public string StorageTemp { get; set; }

		[DataMember(Name = "price")]
		public double Price { get; set; }

		private int _quantityOrders;
        [DataMember(Name = "quantityordered")]
		[Description("# Ordered")]
		public int QuantityOrdered
		{
			get { if (IsDeleted) return 0; return _quantityOrders; }
			set { _quantityOrders = value; }
		}

		private int _quantityShipped;
        [DataMember(Name = "quantityshipped")]
		[Description("# Shipped")]
        public int QantityShipped { get; set; }

        [DataMember(Name = "status")]
        public string Status { 
            get {

				if (this.IsDeleted)
					return Constants.ITEM_DELETED_STATUS;

                string mfStatus = string.IsNullOrEmpty(MainFrameStatus) ? string.Empty : MainFrameStatus.ToUpper().Trim();

                switch (mfStatus) {
                    case Constants.CONFIRMATION_DETAIL_FILLED_CODE:
                        return Constants.CONFIRMATION_DETAIL_FILLED_STATUS;
                    case Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_CODE:
                        return Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_STATUS;
                    case Constants.CONFIRMATION_DETAIL_OUT_OF_STOCK_CODE:
                        return Constants.CONFIRMATION_DETAIL_OUT_OF_STOCK_STATUS;
                    case Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_CODE:
                        return Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_STATUS;
                    case Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_OUT_OF_STOCK_CODE:
                        return Constants.CONFIRMATION_DETAIL_ITEM_REPLACED_OUT_OF_STOCK_STATUS;
                    case Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_REPLACED_CODE:
                        return Constants.CONFIRMATION_DETAIL_PARTIAL_SHIP_REPLACED_STATUS;
                    case Constants.CONFIRMATION_DETAIL_ITEM_SUBBED_CODE:
                        return Constants.CONFIRMATION_DETAIL_ITEM_SUBBED_STATUS;
                    default:
                        // this could contain messages such as "added", "changed", or "deleted"
                        return mfStatus;
                }
            }
            set { }
        }

        [DataMember(Name = "mainframestatus")]
        public string MainFrameStatus { get; set; }

		[DataMember(Name = "changeorderstatus")]
		public string ChangeOrderStatus { get; set; }

        [DataMember(Name = "substituteditemnumber")]
        public string SubstitutedItemNumber { get; set; }

		[DataMember(Name = "totalshippedqeight")]
		public decimal TotalShippedWeight { get; set; }

		[DataMember(Name = "orderstatus")]
		public string OrderStatus { get; set; }

		[DataMember(Name = "isdeleted")]
		public bool IsDeleted { get; set; }

		public List<ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Notes", Order = 10 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "PackSize", Order = 20 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "BrandExtendedDescription", Order = 30 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 40 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Quantity", Order = 50 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "QuantityOrdered", Order = 60 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "QantityShipped", Order = 70 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "EachYN", Order = 80 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Price", Order = 90 });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Status", Order = 100 });


			return defaultConfig;
		}
	}
}
