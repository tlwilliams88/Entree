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
    [DataContract(Name = "invoicecustomers")]
    [Serializable]
    public class InvoiceCustomers
    {
        [DataMember(Name = "totaldue")]
        public decimal TotalAmountDue { get; set; }

        [DataMember(Name = "numbercustomers")]
        public int NumberCustomers { get; set; }

        [DataMember(Name = "totalnumberinvoices")]
        public long TotalNumberInvoices { get; set; }

        [DataMember(Name = "customers")]
        public List<InvoiceCustomer> customers { get; set; }
    }
}
