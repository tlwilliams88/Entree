using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile
{
    public class CustomerNumberComparer : EqualityComparer<Customer>
    {
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override bool Equals(Customer x, Customer y)
        {
            return x.CustomerNumber == y.CustomerNumber;
        }

        public override int GetHashCode(Customer obj)
        {
            return int.Parse(obj.CustomerNumber);
        }
    }

    [DataContract(Name = "customer")]
    public class Customer
    {
        [DataMember(Name = "customerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "customerName")]
        public string CustomerName { get; set; }

		[DataMember(Name = "displayname")]
		public string DisplayName { get; set; }

        [DataMember(Name = "customerBranch")]
        public string CustomerBranch { get; set; }

        [DataMember(Name = "nationalOrRegionalAccountNumber")]
        public string NationalOrRegionalAccountNumber { get; set; }

        [DataMember(Name = "customerNationalId")]
        public string NationalId { get; set; }

        [DataMember(Name = "dsrNumber")]
        public string DsrNumber { get; set; }

        [DataMember(Name = "contractId")]
        public string ContractId { get; set; }

        [DataMember(Name = "isPoRequired")]
        public bool IsPoRequired { get; set; }

        [DataMember(Name = "isPowerMenu")]
        public bool IsPowerMenu { get; set; }

        [DataMember(Name = "customerId")]
        public Guid CustomerId { get; set; }

        [DataMember(Name = "accountId")]
        public Guid? AccountId { get; set; }

		[DataMember(Name="lastorderupdate")]
		public DateTime? LastOrderUpdate { get; set; }

        [DataMember(Name="address")]
        public Address Address { get; set; }

        [DataMember(Name="phone")]
        public string Phone { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "pointofcontact")]
        public string PointOfContact { get; set; }

        [DataMember(Name = "currentbalance")]
        public Decimal? CurrentBalance { get; set; }

        [DataMember(Name = "balanceage1")]
        public Decimal? BalanceAge1 { get; set; }
		[DataMember(Name = "balanceage1label")]
		public string BalanceAge1Label { get; set; }

        [DataMember(Name = "balanceage2")]
        public Decimal? BalanceAge2 { get; set; }
		[DataMember(Name = "balanceage2label")]
		public string BalanceAge2Label { get; set; }

        [DataMember(Name = "balanceage3")]
        public Decimal? BalanceAge3 { get; set; }
		[DataMember(Name = "balanceage3label")]
		public string BalanceAge3Label { get; set; }

        [DataMember(Name = "balanceage4")]
        public Decimal? BalanceAge4 { get; set; }
		[DataMember(Name = "balanceage4label")]
		public string BalanceAge4Label { get; set; }

		[DataMember(Name = "termcode")]
		public string TermCode { get; set; }
		[DataMember(Name = "termdescription")]
		public string TermDescription { get; set; }

        [DataMember(Name = "ach_type")]
        public string AchType { get; set; }
    }

    public class CustomerAddUserModel
    {
        public Guid customerId { get; set; }
        public Guid userId { get; set; }
        public string role { get; set; }
    }
}
