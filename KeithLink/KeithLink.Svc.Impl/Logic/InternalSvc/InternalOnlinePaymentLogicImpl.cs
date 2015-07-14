using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Extensions.OnlinePayments;
using KeithLink.Svc.Core.Extensions.OnlinePayments.Customer;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Common.Core.Helpers;
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
using EFCustomer = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using EFInvoice = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.Impl.Logic.InternalSvc {
	public class InternalOnlinePaymentLogicImpl: IOnlinePaymentsLogic {

        #region attributes
		private readonly ICustomerBankRepository _bankRepo;
		private readonly ICatalogLogic _catalogLogic;
		private readonly ICustomerRepository _customerRepository;
        private readonly IKPayInvoiceRepository _invoiceRepo;
        private readonly IKPayLogRepository _kpaylog;
        private readonly IOrderHistoryHeaderRepsitory _orderHistoryRepo;
		private readonly IKPayPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IGenericQueueRepository _queue;
        #endregion

        #region ctor
        public InternalOnlinePaymentLogicImpl(IKPayInvoiceRepository invoiceRepo, ICustomerBankRepository bankRepo, IOrderHistoryHeaderRepsitory orderHistoryrepo,
			ICatalogLogic catalogLogic, ICustomerRepository customerRepository, IGenericQueueRepository queueRepo, IKPayPaymentTransactionRepository paymentTransactionRepository,
            IKPayLogRepository kpayLogRepo) {
			this._invoiceRepo = invoiceRepo;
			this._bankRepo = bankRepo;
			this._orderHistoryRepo = orderHistoryrepo;
			this._catalogLogic = catalogLogic;
			this._customerRepository = customerRepository;
            _queue = queueRepo;
			this._paymentTransactionRepository = paymentTransactionRepository;
            _kpaylog = kpayLogRepo;
		}
        #endregion

        #region methods
        private FilterInfo BuildCustomerFilter(List<Core.Models.Profile.Customer> customers) {
            FilterInfo filter = new FilterInfo();
            filter.Filters = new List<FilterInfo>();
            filter.Condition = "||";
            //Build customer filter
            foreach (var cust in customers) {
                var inFilter = new FilterInfo();
                inFilter.Condition = "&&";
				inFilter.Filters = new List<FilterInfo>() { new FilterInfo() { Field = "Division", Value = DivisionHelper.GetDivisionFromBranchId(cust.CustomerBranch), FilterType = "eq" }, new FilterInfo() { Field = "CustomerNumber", Value = cust.CustomerNumber, FilterType = "eq" } };

                filter.Filters.Add(inFilter);
            }
            return filter;
        }

        private FilterInfo BuildStatusFilter(FilterInfo passedFilter) {
            if (passedFilter == null)
                return null;

            if (passedFilter.Filters != null) {
                foreach (var filter in passedFilter.Filters) {
                    var createdFilter = BuildStatusFilter(filter);
                    if (createdFilter != null)
                        return createdFilter;
                }
            }

            if (passedFilter.Field.Equals("StatusDescription", StringComparison.CurrentCultureIgnoreCase)) {
                switch (passedFilter.Value.ToUpper()) {
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
			EFCustomer.CustomerBank bankEntity = _bankRepo.GetBankAccount(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, accountNumber);

            if (bankEntity == null)
                return null;
            else {
                CustomerBank bank = new CustomerBank();
                bank.Parse(bankEntity);

                return bank;
            }
        }

		public CustomerAccountBalanceModel GetCustomerAccountBalance(string customerId, string branchId)
		{
			var invoices = _invoiceRepo.GetAllOpenInvoices(DivisionHelper.GetDivisionFromBranchId(branchId), customerId);
			
			var returnModel = new CustomerAccountBalanceModel() { CurrentBalance = 0, PastDue = 0, TotalBalance = 0 };

			if (invoices != null)
			{
				returnModel.TotalBalance = invoices.Sum(i => i.AmountDue);
				returnModel.CurrentBalance = invoices.Where(i => i.DueDate >= DateTime.Now).Sum(a => a.AmountDue);
				returnModel.PastDue = invoices.Where(i => i.DueDate < DateTime.Now).Sum(a => a.AmountDue);
			}

			return returnModel;
		}

        public InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber) {
            var kpayInvoiceHeader = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber);
            var customer = _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);

            if (kpayInvoiceHeader == null) //Invoice not found
                return null;

            //Convert to invoice model
            var invoiceModel = kpayInvoiceHeader.ToInvoiceModel(customer);

            invoiceModel.InvoiceLink =new Uri(Configuration.WebNowUrl.Inject(new { branch = userContext.BranchId, customer = userContext.CustomerId, invoice = invoiceNumber }));

            if (invoiceModel.DueDate.Value.Date < DateTime.Now.Date && invoiceModel.Amount != 0) {
                invoiceModel.Status = InvoiceStatus.PastDue;
                invoiceModel.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(InvoiceStatus.PastDue);
            }

            //Get transactions
			var transactions = _invoiceRepo.GetInvoiceTransactoin(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber);
            invoiceModel.Transactions = transactions.Select(t => t.ToTransationModel()).ToList();

            //Retrieve invoice details, from order history

            var details = _orderHistoryRepo.Read(o => o.InvoiceNumber.Equals(invoiceNumber), d => d.OrderDetails).FirstOrDefault();

            if (details != null && details.OrderDetails != null) {
                invoiceModel.Items = details.OrderDetails.Select(d => d.ToInvoiceItem()).ToList();
            }
            
            //look up product details
            LookupProductDetails(invoiceModel, userContext);

            return invoiceModel;
        }

        public InvoiceHeaderReturnModel GetInvoiceHeaders(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers) {
			var kpayInvoices = new List<EFInvoice.Invoice>();
					
			var customers = new List<Core.Models.Profile.Customer>();

			if (forAllCustomers)
				customers = _customerRepository.GetCustomersForUser(user.UserId);
			else
				customers = new List<Core.Models.Profile.Customer>() { _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId) };

			FilterInfo isfilter = new FilterInfo();
			isfilter.Filters = new List<FilterInfo>();
			isfilter.Filters.Add(new FilterInfo() { Field = "ItemSequence", Value = "0", FilterType = "eq" }); //Header information is is ItemSequence = 0
			var statusFilter = BuildStatusFilter(paging.Filter);
			if (statusFilter != null)
				isfilter.Filters.Add(statusFilter);


			FilterInfo filter = BuildCustomerFilter(customers);
			
			kpayInvoices = _invoiceRepo.ReadAll().AsQueryable().Filter(isfilter, null).Filter(filter, null).ToList();

			var pagedInvoices = kpayInvoices.Select(i => i.ToInvoiceModel(customers.Where(c => c.CustomerNumber.Equals(i.CustomerNumber)).First())).AsQueryable<InvoiceModel>().GetPage(paging, defaultSortPropertyName: "InvoiceNumber");

			foreach(var invoice in pagedInvoices.Results)
			{
				invoice.InvoiceLink = new Uri(Configuration.WebNowUrl.Inject(new { branch = invoice.BranchId, customer = invoice.CustomerNumber, invoice = invoice.InvoiceNumber }));

				if (invoice.Status == InvoiceStatus.Pending)
				{
					//Retrieve payment transaction record
					var payment = _paymentTransactionRepository.ReadAll().Where(p => p.Division.Equals(DivisionHelper.GetDivisionFromBranchId(invoice.BranchId)) && p.CustomerNumber.Equals(invoice.CustomerNumber) && p.InvoiceNumber.Equals(invoice.InvoiceNumber)).FirstOrDefault();

					if (payment != null)
					{
						invoice.PendingTransaction = payment.ToPaymentTransactionModel(customers.Where(c => c.CustomerNumber.Equals(invoice.CustomerNumber)).FirstOrDefault());

					}
				}

				var orderHistory = _orderHistoryRepo.ReadForInvoice(invoice.BranchId, invoice.InvoiceNumber).FirstOrDefault();
				if (orderHistory != null)
				{
					invoice.PONumber = orderHistory.PONumber;
				}

			}
			
            return new InvoiceHeaderReturnModel() {
                HasPayableInvoices = customers.Any(i => i.KPayCustomer) && kpayInvoices.Count > 0,
                PagedResults = pagedInvoices,
				TotalAmmountDue = kpayInvoices.Sum(i => i.AmountDue)
            };
        }		

		private void LookupProductDetails(InvoiceModel invoiceItem, UserSelectedContext catalogInfo)
		{
			if (invoiceItem.Items == null || invoiceItem.Items.Count == 0)
				return;

			var products = _catalogLogic.GetProductsByIds(invoiceItem.BranchId, invoiceItem.Items.Select(i => i.ItemNumber).Distinct().ToList());

			Parallel.ForEach(invoiceItem.Items, invoiceDetailItem =>
			{
				var prod = products.Products.Where(p => p.ItemNumber.Equals(invoiceDetailItem.ItemNumber)).FirstOrDefault();
				if (prod != null)
				{
					invoiceDetailItem.Name = prod.Name;
					invoiceDetailItem.Description = prod.Description;
					invoiceDetailItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
					invoiceDetailItem.Brand = prod.BrandExtendedDescription;
					invoiceDetailItem.BrandExtendedDescription = prod.BrandExtendedDescription;
					invoiceDetailItem.ReplacedItem = prod.ReplacedItem;
					invoiceDetailItem.ReplacementItem = prod.ReplacementItem;
					invoiceDetailItem.NonStock = prod.NonStock;
					invoiceDetailItem.ChildNutrition = prod.ChildNutrition;
					invoiceDetailItem.CatchWeight = prod.CatchWeight;
					invoiceDetailItem.TempZone = prod.TempZone;
					invoiceDetailItem.ItemClass = prod.ItemClass;
					invoiceDetailItem.CategoryId = prod.CategoryId;
					invoiceDetailItem.CategoryName = prod.CategoryName;
					invoiceDetailItem.UPC = prod.UPC;
					invoiceDetailItem.VendorItemNumber = prod.VendorItemNumber;
					invoiceDetailItem.Cases = prod.Cases;
					invoiceDetailItem.Kosher = prod.Kosher;
					invoiceDetailItem.ManufacturerName = prod.ManufacturerName;
					invoiceDetailItem.ManufacturerNumber = prod.ManufacturerNumber;
					invoiceDetailItem.Nutritional = new Nutritional()
					{
						CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
						GrossWeight = prod.Nutritional.GrossWeight,
						HandlingInstructions = prod.Nutritional.HandlingInstructions,
						Height = prod.Nutritional.Height,
						Length = prod.Nutritional.Length,
						Ingredients = prod.Nutritional.Ingredients,
						Width = prod.Nutritional.Width
					};

				}				
			});

		}

		public void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments)
		{
            _kpaylog.Write(emailAddress, string.Format("Paying invoices for customer ({0}-{1})", userContext.BranchId, userContext.CustomerId));
			
            var confId = _invoiceRepo.GetNextConfirmationId();

            foreach (var payment in payments) {
                if (!payment.PaymentDate.HasValue) { payment.PaymentDate = DateTime.Now; }
                payment.ConfirmationId = (int)confId;

                _invoiceRepo.PayInvoice(new Core.Models.OnlinePayments.Payment.EF.PaymentTransaction() {
                    AccountNumber = payment.AccountNumber,
					Division = DivisionHelper.GetDivisionFromBranchId(userContext.BranchId),
                    ConfirmationId = payment.ConfirmationId,
                    CustomerNumber = userContext.CustomerId,
                    InvoiceNumber = payment.InvoiceNumber,
                    PaymentAmount = payment.PaymentAmount,
                    PaymentDate = payment.PaymentDate.Value,
                    UserName = emailAddress
                });

				_invoiceRepo.MarkInvoiceAsPaid(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, payment.InvoiceNumber);

                _kpaylog.Write(emailAddress, string.Format("Invoice paid({0} - {1} - {2})", payment.InvoiceNumber, payment.PaymentDate.Value, payment.PaymentAmount));
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

		public PagedResults<PaymentTransactionModel> PendingTransactionsAllCustomers(UserProfile user, PagingModel paging)
		{
			var customers = _customerRepository.GetCustomersForUser(user.UserId);

			var customerFilter = BuildCustomerFilter(customers);

			var transactions = _paymentTransactionRepository.ReadAll().AsQueryable().Filter(customerFilter, null);

			var transactionResults = transactions.ToList().Select(t => t.ToPaymentTransactionModel(customers.Where(c => c.CustomerNumber.Equals(t.CustomerNumber)).First())).AsQueryable().GetPage(paging);

			return transactionResults;
		}
		
		#endregion
	}
}
