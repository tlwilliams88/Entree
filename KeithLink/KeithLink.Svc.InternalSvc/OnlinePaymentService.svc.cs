using KeithLink.Common.Core.Extensions;
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
using KeithLink.Svc.Impl;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.InternalSvc {
    public class OnlinePaymentService : IOnlinePaymentService {
        #region attributes
        private readonly ICustomerBankRepository _bankRepo;
        private readonly IKPayInvoiceRepository _invoiceRepo;
		private readonly IOnlinePaymentsLogic _onlinePaymentsLogic;
        #endregion

        #region ctor
        public OnlinePaymentService(ICustomerBankRepository customerBankRepository, IKPayInvoiceRepository kpayInvoiceRepository, IOnlinePaymentsLogic onlinePaymentsLogic) {
            _bankRepo = customerBankRepository;
            _invoiceRepo = kpayInvoiceRepository;
			_onlinePaymentsLogic = onlinePaymentsLogic;
        }
        #endregion

        #region methods
        public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber) {
			_onlinePaymentsLogic.DeleteInvoice(userContext, invoiceNumber);
        }

        public List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext) {
			return _onlinePaymentsLogic.GetAllBankAccounts(userContext);
        }

        public InvoiceHeaderReturnModel GetAllOpenInvoices(UserSelectedContext userContext, PagingModel paging) {
            return _onlinePaymentsLogic.GetAllOpenInvoices(userContext, paging);
        }

        public InvoiceHeaderReturnModel GetAllPaidInvoices(UserSelectedContext userContext, PagingModel paging) {
            return _onlinePaymentsLogic.GetAllPaidInvoices(userContext, paging);
        }

        public InvoiceHeaderReturnModel GetAllPastDueInvoices(UserSelectedContext userContext, PagingModel paging) {
            return _onlinePaymentsLogic.GetAllPastDueInvoices(userContext, paging);
        }

        public InvoiceHeaderReturnModel GetAllPayableInvoices(UserSelectedContext userContext, PagingModel paging) {
            return _onlinePaymentsLogic.GetAllPayableInvoices(userContext, paging);
        }

        public CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber) {
			return _onlinePaymentsLogic.GetBankAccount(userContext, accountNumber);
                
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

        public InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber) {
            return _onlinePaymentsLogic.GetInvoiceDetails(userContext, invoiceNumber.Trim());
        }

		public InvoiceHeaderReturnModel GetInvoiceHeaders(UserSelectedContext userContext, PagingModel paging)
		{
			return _onlinePaymentsLogic.GetInvoiceHeaders(userContext, paging);
        }

        public List<InvoiceModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber) {
            List<EFInvoice.Invoice> kpayInvoices = _invoiceRepo.GetInvoiceTransactoin(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);
            List<InvoiceModel> returnInvoices = kpayInvoices.Select(i => i.ToInvoiceModel()).ToList();

            foreach(InvoiceModel inv in returnInvoices){
                // set link to web now
                System.Collections.Hashtable dictionary = new System.Collections.Hashtable();
                dictionary.Add("branch", userContext.BranchId);
                dictionary.Add("customer", userContext.CustomerId);
                dictionary.Add("invoice", inv.InvoiceNumber);

                inv.InvoiceLink = new Uri(Configuration.WebNowUrl.Inject(dictionary));
            }

            return returnInvoices;
        }
        
        public void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<Core.Models.OnlinePayments.Payment.PaymentTransactionModel> payments)
		{
			_onlinePaymentsLogic.MakeInvoicePayment(userContext, emailAddress, payments);
		}
		#endregion				
	

	}
}
