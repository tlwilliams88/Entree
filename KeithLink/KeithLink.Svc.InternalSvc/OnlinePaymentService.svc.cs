using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Models.SiteCatalog;
using EFCustomer = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using EFInvoice = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc {
    public class OnlinePaymentService : IOnlinePaymentService {
        #region attributes
        private readonly ICustomerBankRepository _bankRepo;
        private readonly IKPayInvoiceRepository _invoiceRepo;
        #endregion

        #region ctor
        public OnlinePaymentService(ICustomerBankRepository customerBankRepository, IKPayInvoiceRepository kpayInvoiceRepository) {
            _bankRepo = customerBankRepository;
            _invoiceRepo = kpayInvoiceRepository;
        }
        #endregion

        #region methods
        public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber) {
            _invoiceRepo.DeleteInvoice(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);
        }

        public List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext) {
            List<EFCustomer.CustomerBank> bankEntities = _bankRepo.GetAllCustomerBanks(GetDivision(userContext.BranchId), userContext.CustomerId);

            List<CustomerBank> banks = new List<CustomerBank>();

            foreach (EFCustomer.CustomerBank entity in bankEntities) {
                if (entity != null) {
                    CustomerBank bank = new CustomerBank();
                    bank.Parse(entity);

                    banks.Add(bank);
                }
            }

            return banks;
        }

        public CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber) {
            EFCustomer.CustomerBank bankEntity = _bankRepo.GetBankAccount(GetDivision(userContext.BranchId), userContext.CustomerId, accountNumber);

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

        public List<InvoiceModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber) {
            List<EFInvoice.Invoice> kpayInvoices = _invoiceRepo.GetInvoiceTransactoin(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);
            List<InvoiceModel> returnInvoices = new List<InvoiceModel>();

			return kpayInvoices.Select(i => i.ToInvoiceModel()).ToList();
        }

        public List<InvoiceModel> GetOpenInvoiceHeaders(UserSelectedContext userContext) {
            List<EFInvoice.Invoice> kpayInvoices = _invoiceRepo.GetMainInvoices(GetDivision(userContext.BranchId), userContext.CustomerId);
            List<InvoiceModel> returnInvoices = kpayInvoices.Select(i => i.ToInvoiceModel()).ToList();
			
			//TODO: add check to see if customer is KPay customer
            returnInvoices.Select(i => { i.IsPayable = true; return i; }).ToList();

            return returnInvoices;
        }
        #endregion
    }
}
