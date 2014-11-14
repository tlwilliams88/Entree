using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.OnlinePayments.Customer {
    public static class CustomerBankExtension {
        public static void Parse(this KeithLink.Svc.Core.Models.OnlinePayments.Customer.CustomerBank domainValue, KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF.CustomerBank dbValue) {
            domainValue.AccountNumber = dbValue.AccountNumber;
            domainValue.Address1 = dbValue.Address1;
            domainValue.Address2 = dbValue.Address2;
            domainValue.City = dbValue.City;
            domainValue.CustomerNumber = dbValue.CustomerNumber;
            domainValue.DefaultAccount = dbValue.DefaultAccount;
            domainValue.BranchId = (dbValue.Division != null && dbValue.Division.Length > 3 ? dbValue.Division.Substring(0, 3) : null);
            domainValue.Name = dbValue.Name;
            domainValue.State = dbValue.State;
            domainValue.TransitNumber = dbValue.TransitNumber;
            domainValue.Zip = dbValue.Zip;
        }
    }
}
