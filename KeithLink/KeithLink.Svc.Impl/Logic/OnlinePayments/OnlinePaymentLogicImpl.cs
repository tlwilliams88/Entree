﻿using KeithLink.Common.Core.Enumerations;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Helpers;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core;

using KeithLink.Svc.Core.Enumerations;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Extensions.OnlinePayments;
using KeithLink.Svc.Core.Extensions.OnlinePayments.Customer;
using KeithLink.Svc.Core.Extensions.Orders.History;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Log;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using EFCustomer = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using EFInvoice = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.OnlinePayments {
    public class OnlinePaymentLogicImpl : IOnlinePaymentsLogic {
        #region attributes
        private readonly IAuditLogRepository _auditLog;
        private readonly ICustomerBankRepository _bankRepo;
        //private readonly ICatalogLogic _catalogLogic;
        private readonly ICustomerRepository _customerRepository;
        private readonly IKPayInvoiceRepository _invoiceRepo;
        private readonly IKPayLogRepository _kpaylog;
        private readonly IOrderHistoryHeaderRepsitory _orderHistoryRepo;
        private readonly IKPayPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IGenericQueueRepository _queue;
        #endregion

        #region ctor
        public OnlinePaymentLogicImpl(IKPayInvoiceRepository invoiceRepo, ICustomerBankRepository bankRepo, IOrderHistoryHeaderRepsitory orderHistoryrepo,
                                      ICustomerRepository customerRepository, IGenericQueueRepository queueRepo, IKPayPaymentTransactionRepository paymentTransactionRepository, 
                                      IKPayLogRepository kpayLogRepo, IAuditLogRepository auditLogRepo) {
            _invoiceRepo = invoiceRepo;
            _bankRepo = bankRepo;
            _orderHistoryRepo = orderHistoryrepo;
            //_catalogLogic = catalogLogic;
            _customerRepository = customerRepository;
            _queue = queueRepo;
            _paymentTransactionRepository = paymentTransactionRepository;
            _kpaylog = kpayLogRepo;
            _auditLog = auditLogRepo;
        }
        #endregion

        #region methods
        private FilterInfo BuildCustomerFilter(List<Core.Models.Profile.Customer> customers) {
            FilterInfo filter = new FilterInfo();
            filter.Filters = new List<FilterInfo>();
            filter.Condition = "||";
            //Build customer filter
            foreach(var cust in customers) {
                var inFilter = new FilterInfo();
                inFilter.Condition = "&&";
                inFilter.Filters = new List<FilterInfo>() { new FilterInfo() { Field = "Division", Value = DivisionHelper.GetDivisionFromBranchId(cust.CustomerBranch), FilterType = "eq" },
                                                            new FilterInfo() { Field = "CustomerNumber", Value = cust.CustomerNumber, FilterType = "eq" } };

                filter.Filters.Add(inFilter);
            }
            return filter;
        }

        private FilterInfo BuildStatusFilter(FilterInfo passedFilter) {
            if(passedFilter == null)
                return null;

            if(passedFilter.Filters != null) {
                foreach(var filter in passedFilter.Filters) {
                    var createdFilter = BuildStatusFilter(filter);
                    if(createdFilter != null)
                        return createdFilter;
                }
            }

            if(passedFilter.Field.Equals("StatusDescription", StringComparison.CurrentCultureIgnoreCase)) {
                switch(passedFilter.Value.ToUpper()) {
                    case "OPEN":
                        return new FilterInfo() { Field = "InvoiceStatus", Value = "O", FilterType = "eq" };
                    case "PAID":
                        return new FilterInfo() { Field = "InvoiceStatus", Value = "C", FilterType = "eq" };
                    case "PAST DUE":
                        return new FilterInfo() { Field = "InvoiceStatus", Value = "O", FilterType = "eq" };
                    case "PENDING":
                        return new FilterInfo() { Field = "InvoiceStatus", Value = "P", FilterType = "eq" };
                }
            }

            return null;
        }

        public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber) {
            _invoiceRepo.DeleteInvoice(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber);
        }

        public List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext) {
            List<EFCustomer.CustomerBank> bankEntities = _bankRepo.GetAllCustomerBanks(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId);

            List<CustomerBank> banks = new List<CustomerBank>();

            foreach(EFCustomer.CustomerBank entity in bankEntities) {
                if(entity != null) {
                    CustomerBank bank = new CustomerBank();
                    bank.Parse(entity);

                    banks.Add(bank);
                }
            }

            return banks;
        }

        public CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber) {
            EFCustomer.CustomerBank bankEntity = _bankRepo.GetBankAccount(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, accountNumber);

            if(bankEntity == null)
                return null;
            else {
                CustomerBank bank = new CustomerBank();
                bank.Parse(bankEntity);

                return bank;
            }
        }

        public CustomerAccountBalanceModel GetCustomerAccountBalance(string customerId, string branchId) {
            var invoices = _invoiceRepo.GetAllOpenInvoices(DivisionHelper.GetDivisionFromBranchId(branchId), customerId);

            var returnModel = new CustomerAccountBalanceModel() { CurrentBalance = 0, PastDue = 0, TotalBalance = 0 };

            if(invoices != null) {
                returnModel.TotalBalance = invoices.Sum(i => i.AmountDue);
                returnModel.CurrentBalance = invoices.Where(i => i.DueDate >= DateTime.Now).Sum(a => a.AmountDue);
                returnModel.PastDue = invoices.Where(i => i.DueDate < DateTime.Now).Sum(a => a.AmountDue);
            }

            return returnModel;
        }

        public InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber) {
            var kpayInvoiceHeader = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber);
            var customer = _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);

            if(kpayInvoiceHeader == null) //Invoice not found
                return null;

            //Convert to invoice model
            var invoiceModel = kpayInvoiceHeader.ToInvoiceModel(customer);

            invoiceModel.InvoiceLink = new Uri(Configuration.WebNowUrl.Inject(new { branch = userContext.BranchId, customer = userContext.CustomerId, invoice = invoiceNumber }));

            if(invoiceModel.DueDate < DateTime.Now) {
                invoiceModel.Status = InvoiceStatus.PastDue;
                invoiceModel.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(InvoiceStatus.PastDue);
            }

            //Get transactions
            invoiceModel.Transactions = GetInvoiceTransactions(userContext, invoiceNumber);

            return invoiceModel;
        }

        public List<InvoiceTransactionModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber) {
            List<InvoiceTransactionModel> transactions = new List<InvoiceTransactionModel>();

            transactions = _invoiceRepo.GetInvoiceTransactoin(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber).Select(t => t.ToTransationModel()).ToList();

            return transactions;
        }

        public InvoiceHeaderReturnModel GetInvoiceHeaders(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers) {
            var customers = new List<Core.Models.Profile.Customer>();

            if(forAllCustomers)
                customers = _customerRepository.GetCustomersForUser(user.UserId);
            else
                customers = new List<Core.Models.Profile.Customer>() { _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId) };

            FilterInfo statusFilter = BuildStatusFilter(paging.Filter);
            FilterInfo customerFilter = BuildCustomerFilter(customers);

            var kpayInvoices = _invoiceRepo.ReadAllHeaders().AsQueryable().Filter(customerFilter, null).Filter(statusFilter, null).ToList();

            var pagedInvoices = kpayInvoices.Select(i => i.ToInvoiceModel(customers.Where(c => c.CustomerNumber.Equals(i.CustomerNumber)).First())).AsQueryable<InvoiceModel>().GetPage(paging, defaultSortPropertyName: "InvoiceNumber");

            foreach(var invoice in pagedInvoices.Results) {
                invoice.InvoiceLink = new Uri(Configuration.WebNowUrl.Inject(new { branch = invoice.BranchId, customer = invoice.CustomerNumber, invoice = invoice.InvoiceNumber }));

                var customer = _customerRepository.GetCustomerByCustomerNumber(invoice.CustomerNumber, invoice.BranchId);
                invoice.CustomerStreetAddress = customer.Address.StreetAddress;
                invoice.CustomerCity = customer.Address.City;
                invoice.CustomerRegionCode = customer.Address.RegionCode;
                invoice.CustomerPostalCode = customer.Address.PostalCode;

                if (invoice.Status == InvoiceStatus.Pending) {
                    //Retrieve payment transaction record
                    var payment = _paymentTransactionRepository.ReadAll().Where(p => p.Division.Equals(DivisionHelper.GetDivisionFromBranchId(invoice.BranchId)) && p.CustomerNumber.Equals(invoice.CustomerNumber) && p.InvoiceNumber.Equals(invoice.InvoiceNumber)).FirstOrDefault();

                    if(payment != null) {
                        invoice.PendingTransaction = payment.ToPaymentTransactionModel(customers.Where(c => c.CustomerNumber.Equals(invoice.CustomerNumber)).FirstOrDefault(),
                                                                                                                            Configuration.BillPayCutOffTime);
                    }
                }

                //Get transactions
                var transactions = _invoiceRepo.GetInvoiceTransactoin(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoice.InvoiceNumber);
                invoice.InvoiceAmount = transactions
                    .Where(x => x.InvoiceType == KeithLink.Svc.Core.Constants.INVOICETRANSACTIONTYPE_INITIALINVOICE)
                    .Select(x => x.AmountDue).FirstOrDefault();

                if(transactions
                    .Where(x => x.InvoiceType == KeithLink.Svc.Core.Constants.INVOICETRANSACTIONTYPE_CREDITMEMO)
                    .Count() > 0) {
                    invoice.HasCreditMemos = true;
                } else {
                    invoice.HasCreditMemos = false;
                }

                var orderHistory = _orderHistoryRepo.ReadForInvoice(invoice.BranchId, invoice.InvoiceNumber).FirstOrDefault();
                if(orderHistory != null) {
                    invoice.PONumber = orderHistory.PONumber;
                }
            }

            if((paging != null) && (paging.Filter != null) && (paging.Filter.Filters != null)
                && (paging.Filter.Filters.Count > 0)
                && (paging.Filter.Filters.Where(f => f.Field == Constants.INVOICEREQUESTFILTER_CREDITMEMO_FIELDKEY).Count() > 0)) {
                var filter = paging.Filter.Filters
                    .Where(f => f.Field == Constants.INVOICEREQUESTFILTER_CREDITMEMO_FIELDKEY).FirstOrDefault();
                if(filter.Value.Equals(Constants.INVOICEREQUESTFILTER_CREDITMEMO_VALUECMONLY, StringComparison.CurrentCultureIgnoreCase)) {
                    pagedInvoices.Results = pagedInvoices.Results.Where(i => i.HasCreditMemos).ToList();
                } else if(filter.Value.Equals(Constants.INVOICEREQUESTFILTER_CREDITMEMO_VALUENOTCM, StringComparison.CurrentCultureIgnoreCase)) {
                    pagedInvoices.Results = pagedInvoices.Results.Where(i => i.HasCreditMemos == false).ToList();
                }
            }

            return new InvoiceHeaderReturnModel() {
                HasPayableInvoices = customers.Any(i => i.KPayCustomer) && kpayInvoices.Count > 0,
                PagedResults = pagedInvoices,
                TotalAmmountDue = kpayInvoices.Sum(i => i.AmountDue)
            };
        }

        //private void LookupProductDetails(InvoiceModel invoiceItem, UserSelectedContext catalogInfo) {
        //    if(invoiceItem.Items == null || invoiceItem.Items.Count == 0)
        //        return;

        //    var products = _catalogLogic.GetProductsByIds(invoiceItem.BranchId, invoiceItem.Items.Select(i => i.ItemNumber).Distinct().ToList());

        //    Parallel.ForEach(invoiceItem.Items, invoiceDetailItem => {
        //        var prod = products.Products.Where(p => p.ItemNumber.Equals(invoiceDetailItem.ItemNumber)).FirstOrDefault();
        //        if(prod != null) {
        //            invoiceDetailItem.IsValid = true;
        //            invoiceDetailItem.Name = prod.Name;
        //            invoiceDetailItem.Description = prod.Description;
        //            invoiceDetailItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
        //            invoiceDetailItem.Brand = prod.BrandExtendedDescription;
        //            invoiceDetailItem.BrandExtendedDescription = prod.BrandExtendedDescription;
        //            invoiceDetailItem.ReplacedItem = prod.ReplacedItem;
        //            invoiceDetailItem.ReplacementItem = prod.ReplacementItem;
        //            invoiceDetailItem.NonStock = prod.NonStock;
        //            invoiceDetailItem.ChildNutrition = prod.ChildNutrition;
        //            invoiceDetailItem.CatchWeight = prod.CatchWeight;
        //            invoiceDetailItem.TempZone = prod.TempZone;
        //            invoiceDetailItem.ItemClass = prod.ItemClass;
        //            invoiceDetailItem.CategoryId = prod.CategoryId;
        //            invoiceDetailItem.CategoryName = prod.CategoryName;
        //            invoiceDetailItem.UPC = prod.UPC;
        //            invoiceDetailItem.VendorItemNumber = prod.VendorItemNumber;
        //            invoiceDetailItem.Cases = prod.Cases;
        //            invoiceDetailItem.Kosher = prod.Kosher;
        //            invoiceDetailItem.ManufacturerName = prod.ManufacturerName;
        //            invoiceDetailItem.ManufacturerNumber = prod.ManufacturerNumber;
        //            invoiceDetailItem.Nutritional = new Nutritional() {
        //                CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
        //                GrossWeight = prod.Nutritional.GrossWeight,
        //                HandlingInstructions = prod.Nutritional.HandlingInstructions,
        //                Height = prod.Nutritional.Height,
        //                Length = prod.Nutritional.Length,
        //                Ingredients = prod.Nutritional.Ingredients,
        //                Width = prod.Nutritional.Width
        //            };

        //        }
        //    });

        //}

        public void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments) {
            _kpaylog.Write(emailAddress, string.Format("Paying invoices for customer ({0}-{1})", userContext.BranchId, userContext.CustomerId));

            var confId = _invoiceRepo.GetNextConfirmationId();

            foreach(var payment in payments) {
                var testInvoice = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(payment.BranchId), payment.CustomerNumber, payment.InvoiceNumber);
                if(testInvoice == null) {
                    throw new ApplicationException("Could find invoice for selected customer");
                }

                if(!payment.PaymentDate.HasValue) { payment.PaymentDate = DateTime.Now; }
                payment.ConfirmationId = (int)confId;

                _invoiceRepo.PayInvoice(new Core.Models.OnlinePayments.Payment.EF.PaymentTransaction() {
                    AccountNumber = payment.AccountNumber,
                    Division = DivisionHelper.GetDivisionFromBranchId(payment.BranchId),
                    ConfirmationId = payment.ConfirmationId,
                    CustomerNumber = payment.CustomerNumber,
                    InvoiceNumber = payment.InvoiceNumber,
                    PaymentAmount = payment.PaymentAmount,
                    PaymentDate = payment.PaymentDate.Value,
                    UserName = emailAddress
                });

                _invoiceRepo.MarkInvoiceAsPaid(DivisionHelper.GetDivisionFromBranchId(payment.BranchId), payment.CustomerNumber, payment.InvoiceNumber);

                string paymentInfo = string.Format("Invoice paid({0} - {1} - {2})", payment.InvoiceNumber, payment.PaymentDate.Value, payment.PaymentAmount);
                _kpaylog.Write(emailAddress, paymentInfo);
                _auditLog.WriteToAuditLog(AuditType.PaymentSubmitted, emailAddress, paymentInfo);
            }

            _kpaylog.Write(emailAddress, string.Concat("Payments for confirmation id: ", confId));

            // create payment notification
            PaymentConfirmationNotification notification = new PaymentConfirmationNotification();
            notification.BranchId = userContext.BranchId;
            notification.CustomerNumber = userContext.CustomerId;
            notification.Payments = payments;

            // write the payment notification to Rabbit MQ
            _queue.PublishToQueue(notification.ToJson(), Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNamePublisher,
                                  Configuration.RabbitMQNotificationUserPasswordPublisher, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotification);
        }

        public PagedResults<PaymentTransactionModel> PendingTransactionsAllCustomers(UserProfile user, PagingModel paging) {
            var customers = _customerRepository.GetCustomersForUser(user.UserId);
            var customerFilter = BuildCustomerFilter(customers);
            var transactions = _paymentTransactionRepository.ReadAll().AsQueryable().Filter(customerFilter, null);
            var transactionResults = transactions.ToList()
                                                 .Select(t => t.ToPaymentTransactionModel(customers.Where(c => c.CustomerNumber.Equals(t.CustomerNumber)).First(),
                                                                                                               Configuration.BillPayCutOffTime))
                                                 .AsQueryable()
                                                 .GetPage(paging);

            return transactionResults;
        }

        public PagedResults<PaymentTransactionModel> PendingTransactions(UserSelectedContext customer, PagingModel paging) {
            var currentCustomer = _customerRepository.GetCustomerByCustomerNumber(customer.CustomerId, customer.BranchId);
            var transaction = _paymentTransactionRepository.ReadAllByCustomer(customer.CustomerId, DivisionHelper.GetDivisionFromBranchId(customer.BranchId));
            var transactionModels = transaction.Select(t => t.ToPaymentTransactionModel(currentCustomer, Configuration.BillPayCutOffTime));
            var retVal = transactionModels.AsQueryable()
                                          .GetPage(paging);

            return retVal;
        }

        public List<PaymentTransactionModel> ValidatePayment(UserSelectedContext context, List<PaymentTransactionModel> payments) {
            List<PaymentTransactionModel> returnValue = new List<PaymentTransactionModel>();

            // Set default payment date for null payments to today.
            payments.ForEach(x => { if(!x.PaymentDate.HasValue) { x.PaymentDate = DateTime.Now; } });

            // Select all transactions grouped by accountnumber, paymentdate
            var transactions =
                from t in payments
                group t by new {
                    t.AccountNumber,
                    t.PaymentDate,
                } into tg
                select new {
                    AccountNumber = tg.Key.AccountNumber,
                    PaymentDate = tg.Key.PaymentDate.Value.ToShortDateString(),
                    payments = tg.ToList()
                };

            // Go through all daily transactions and validate the sum payment amount is greater than 0 (not negative)
            foreach(var t in transactions) {
                if(t.payments.Sum(x => x.PaymentAmount) < 0) {
                    returnValue.AddRange(t.payments);
                }
            }

            return returnValue;
        }

        #endregion
    }
}
