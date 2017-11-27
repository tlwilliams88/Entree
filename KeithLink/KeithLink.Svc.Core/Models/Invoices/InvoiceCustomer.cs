using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices
{
    [DataContract(Name = "invoicecustomer")]
    [Serializable]
    public class InvoiceCustomer
    {
        [DataMember(Name = "haspayableinvoices")]
        public bool HasPayableInvoices { get; set; }

        [DataMember(Name = "totaldue")]
        public decimal TotalAmountDue { get; set; }

        [DataMember(Name = "numberInvoices")]
        public int NumberInvoices { get; set; }

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
        [DataMember(Name = "contractId")]
        public string ContractId { get; set; }

        [DataMember(Name = "customerId")]
        public Guid CustomerId { get; set; }

        [DataMember(Name = "accountId")]
        public Guid? AccountId { get; set; }

        [DataMember(Name = "address")]
        public Address Address { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "pointofcontact")]
        public string PointOfContact { get; set; }

        [DataMember(Name = "nationalId")]
        public string NationalId { get; set; }

        [DataMember(Name = "nationalNumber")]
        public string NationalNumber { get; set; }

        [DataMember(Name = "nationalSubNumber")]
        public string NationalSubNumber { get; set; }

        [DataMember(Name = "regionalId")]
        public string RegionalId { get; set; }

        [DataMember(Name = "regionalNumber")]
        public string RegionalNumber { get; set; }

        [DataMember(Name = "nationalIdDesc")]
        public string NationalIdDesc { get; set; }

        [DataMember(Name = "nationalNumberSubDesc")]
        public string NationalNumberSubDesc { get; set; }

        [DataMember(Name = "regionalIdDesc")]
        public string RegionalIdDesc { get; set; }

        [DataMember(Name = "regionalNumberDesc")]
        public string RegionalNumberDesc { get; set; }
    }
}
