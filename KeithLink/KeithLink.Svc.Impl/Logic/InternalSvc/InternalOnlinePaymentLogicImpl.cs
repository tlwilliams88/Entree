﻿using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.OnlinePayments;
using KeithLink.Svc.Core.Extensions.OnlinePayments.Customer;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.SiteCatalog;
using EFCustomer = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using EFInvoice = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalOnlinePaymentLogicImpl: IOnlinePaymentsLogic {

        #region attributes
        private readonly IKPayInvoiceRepository _invoiceRepo;
		private readonly ICustomerBankRepository _bankRepo;
		private readonly IOrderHistoryHeaderRepsitory _orderHistoryRepo;
		private readonly ICatalogLogic _catalogLogic;
		private readonly ICustomerRepository _customerRepository;
		private readonly IKPayPaymentTransactionRepository _paymentTransactionRepository;
        #endregion

        #region ctor
        public InternalOnlinePaymentLogicImpl(IKPayInvoiceRepository invoiceRepo, ICustomerBankRepository bankRepo, IOrderHistoryHeaderRepsitory orderHistoryrepo,
			ICatalogLogic catalogLogic, ICustomerRepository customerRepository, IKPayPaymentTransactionRepository paymentTransactionRepository)
		{
			this._invoiceRepo = invoiceRepo;
			this._bankRepo = bankRepo;
			this._orderHistoryRepo = orderHistoryrepo;
			this._catalogLogic = catalogLogic;
			this._customerRepository = customerRepository;
			this._paymentTransactionRepository = paymentTransactionRepository;
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

		private string GetDivision(string branchId)
		{
			if (branchId.Length == 5)
			{
				return branchId;
			}
			else if (branchId.Length == 3)
			{
				switch (branchId.ToUpper())
				{
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
			}
			else
			{
				return null;
			}
		}

        public InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber) {
            var kpayInvoiceHeader = _invoiceRepo.GetInvoiceHeader(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);
            var customer = _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId);

            if (kpayInvoiceHeader == null) //Invoice not found
                return null;

            //Convert to invoice model
            var invoiceModel = kpayInvoiceHeader.ToInvoiceModel(customer);

			

            invoiceModel.InvoiceLink =new Uri(Configuration.WebNowUrl.Inject(new { branch = userContext.BranchId, customer = userContext.CustomerId, invoice = invoiceNumber }));

            if (invoiceModel.DueDate < DateTime.Now) {
                invoiceModel.Status = InvoiceStatus.PastDue;
                invoiceModel.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(InvoiceStatus.PastDue);
            }

            //Get transactions
            var transactions = _invoiceRepo.GetInvoiceTransactoin(GetDivision(userContext.BranchId), userContext.CustomerId, invoiceNumber);
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
				customers = new List<Core.Models.Profile.Customer>() { _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId) };

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
					var payment = _paymentTransactionRepository.ReadAll().Where(p => p.Division.Equals(GetDivision(invoice.BranchId)) && p.CustomerNumber.Equals(invoice.CustomerNumber)).FirstOrDefault();

					if (payment != null)
					{
						invoice.PendingTransaction = payment.ToPaymentTransactionModel(customers.Where(c => c.CustomerNumber.Equals(invoice.CustomerNumber)).FirstOrDefault());

					}
				}

			}
			
            return new InvoiceHeaderReturnModel() {
                HasPayableInvoices = customers.Any(i => i.KPayCustomer) && kpayInvoices.Count > 0,
                PagedResults = pagedInvoices,
				TotalAmmountDue = customers.Sum(c => c.CurrentBalance)
            };
        }

		private FilterInfo BuildCustomerFilter(List<Core.Models.Profile.Customer> customers)
		{
			FilterInfo filter = new FilterInfo();
			filter.Filters = new List<FilterInfo>();
			filter.Condition = "||";
			//Build customer filter
			foreach (var cust in customers)
			{
				var inFilter = new FilterInfo();
				inFilter.Condition = "&&";
				inFilter.Filters = new List<FilterInfo>() { new FilterInfo() { Field = "Division", Value = GetDivision(cust.CustomerBranch), FilterType = "eq" }, new FilterInfo() { Field = "CustomerNumber", Value = cust.CustomerNumber, FilterType = "eq" } };

				filter.Filters.Add(inFilter);
			}
			return filter;
		}

		private FilterInfo BuildStatusFilter(FilterInfo passedFilter)
		{
			if (passedFilter == null)
				return null;

			if (passedFilter.Filters != null)
			{
				foreach (var filter in passedFilter.Filters)
				{
					var createdFilter = BuildStatusFilter(filter);
					if (createdFilter != null) return createdFilter;
				}
			}

			if (passedFilter.Field.Equals("StatusDescription", StringComparison.CurrentCultureIgnoreCase))
			{
				switch (passedFilter.Value.ToUpper())
				{
					case "OPEN":
						return new FilterInfo() { Field = "InvoiceStatus", Value = "O", FilterType = "eq" };
					case "PAID":
						return new FilterInfo() { Field = "InvoiceStatus", Value = "C", FilterType = "eq" };
					case "PAST DUE":
						return new FilterInfo() { Field = "InvoiceStatus", Value = "O", FilterType = "eq" };
				}
			}

			return null;
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
			var confId = _invoiceRepo.GetNextConfirmationId();

            foreach (var payment in payments) {
                _invoiceRepo.PayInvoice(new Core.Models.OnlinePayments.Payment.EF.PaymentTransaction() {
                    AccountNumber = payment.AccountNumber,
                    Division = GetDivision(userContext.BranchId),
                    ConfirmationId = (int)confId,
                    CustomerNumber = userContext.CustomerId,
                    InvoiceNumber = payment.InvoiceNumber,
                    PaymentAmount = payment.PaymentAmount,
                    PaymentDate = payment.PaymentDate.HasValue ? payment.PaymentDate.Value : DateTime.Now,
                    UserName = emailAddress
                });

                _invoiceRepo.MarkInvoiceAsPaid(GetDivision(userContext.BranchId), userContext.CustomerId, payment.InvoiceNumber);
            }
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
