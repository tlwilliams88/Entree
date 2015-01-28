using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.BEKWarmup.Models
{
	public class UserCustomer
	{
		public string customerNumber { get; set; }
		public string customerName { get; set; }
		public string customerBranch { get; set; }
		public string nationalOrRegionalAccountNumber { get; set; }
		public object customerNationalId { get; set; }
		public string dsrNumber { get; set; }
		public string contractId { get; set; }
		public bool isPoRequired { get; set; }
		public bool isPowerMenu { get; set; }
		public string customerId { get; set; }
		public object accountId { get; set; }
		public string lastorderupdate { get; set; }
		public Address address { get; set; }
		public string phone { get; set; }
		public string email { get; set; }
		public string pointofcontact { get; set; }
		public double? currentbalance { get; set; }
		public double? balanceage1 { get; set; }
		public object balanceage1label { get; set; }
		public double? balanceage2 { get; set; }
		public object balanceage2label { get; set; }
		public double? balanceage3 { get; set; }
		public object balanceage3label { get; set; }
		public double? balanceage4 { get; set; }
		public object balanceage4label { get; set; }
		public object termcode { get; set; }
		public object termdescription { get; set; }
	}
}
