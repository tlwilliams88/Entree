using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
	[DataContract(Name="ShoppingCart")]
	public class ShoppingCart
	{
		private DateTime? _requestedShipDate { get; set; }
        
		[DataMember(Name="id")]
		public Guid CartId { get; set; }
		[DataMember(Name="name")]
		public string Name { get; set; }
		public string BranchId { get; set; }
        [DataMember(Name = "requestedshipdate")]
        public DateTime? RequestedShipDate //{ get; set; }
        {
            get { return _requestedShipDate; }
            set 
            {
                if (value == null)
                {
                    _requestedShipDate = null;
                }
                else
                {
                    _requestedShipDate = DateTime.Parse(value.Value.Date.ToString()); //always set returned time to T00:00:00Z, we don't care about timezone
                }
                
            }
        }
		[DataMember(Name="ponumber")]
		public string PONumber { get; set; }
		[DataMember(Name="active")]
		public bool Active { get; set; }

		[DataMember(Name = "itemcount")]
		public int ItemCount { get; set; }

		[DataMember(Name = "subtotal")]
		public decimal SubTotal {get;set;}

		[DataMember(Name = "createddate")]
		public DateTime CreatedDate { get; set; }

		[DataMember(Name="items")]
		public List<ShoppingCartItem> Items { get; set; }
	}
}
