﻿using KeithLink.Svc.Core.Extensions.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using EF = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.InternalSvc.Interfaces.OnlinePayments.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc {
    public class CustomerBankService : ICustomerBankService {
        #region attributes
        private readonly ICustomerBankRepository _bankRepo;
        #endregion

        #region ctor
        public CustomerBankService(ICustomerBankRepository customerBankRepository) {
            _bankRepo = customerBankRepository;
        }
        #endregion

        #region methods
        public List<CustomerBank> GetAllCustomerBanks(string branchId, string customerNumber) {
            List<EF.CustomerBank> bankEntities = _bankRepo.GetAllCustomerBanks(GetDivision(branchId), customerNumber);

            List<CustomerBank> banks = new List<CustomerBank>();

            foreach (EF.CustomerBank entity in bankEntities) {
                if (entity != null) {
                    CustomerBank bank = new CustomerBank();
                    bank.Parse(entity);

                    banks.Add(bank);
                }
            }

            return banks;
        }

        public CustomerBank GetBankAccount(string branchId, string customerNumber, string accountNumber) {
            EF.CustomerBank bankEntity = _bankRepo.GetBankAccount(GetDivision(branchId), customerNumber, accountNumber);

            if (bankEntity == null)
                return null;
            else {
                CustomerBank bank = new CustomerBank();
                bank.Parse(bankEntity);

                return bank;
            }
                
        }

        private string GetDivision(string branchId) {
            if (branchId.Length == 5) {
                return branchId;
            } else if (branchId.Length == 3) {
                switch (branchId.ToUpper()) {
                    case "FAM":
                        return "FAM04";
                    case "FAQ":
                        return "FAQ08";
                    case "FAR":
                        return "FAR09";
                    case "FDF":
                        return "FDF01";
                    case "FHS":
                        return "FHS03";
                    case "FLR":
                        return "FLR05";
                    case "FOK":
                        return "FOK06";
                    case "FSA":
                        return "FSA07";
                    default:
                        return null;
                }
            } else {
                return null;
            }
        }
        #endregion
    }
}
