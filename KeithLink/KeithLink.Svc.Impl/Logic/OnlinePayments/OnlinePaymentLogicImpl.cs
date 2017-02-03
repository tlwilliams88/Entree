using KeithLink.Common.Core.Enumerations;
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
using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.Cache;
using Newtonsoft.Json;
using System.Transactions;

namespace KeithLink.Svc.Impl.Logic.OnlinePayments
{
    public class OnlinePaymentLogicImpl : IOnlinePaymentsLogic
    {
        #region attributes
        private readonly ICacheRepository _cacheRepo;
        private readonly IEventLogRepository _log;
        private readonly IAuditLogRepository _auditLog;
        private readonly IInternalUserAccessRepository _internalUserAccessRepo;
        private readonly ICustomerBankRepository _bankRepo;
        //private readonly ICatalogLogic _catalogLogic;
        private readonly ICustomerRepository _customerRepository;
        private readonly IKPayInvoiceRepository _invoiceRepo;
        private readonly IKPayLogRepository _kpaylog;
        private readonly IOrderHistoryHeaderRepsitory _orderHistoryRepo;
        private readonly IKPayPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IGenericQueueRepository _queue;

        protected string CACHE_GROUPNAME { get { return "Invoices"; } }
        protected string CACHE_NAME { get { return "Invoices"; } }
        protected string CACHE_PREFIX { get { return "Default"; } }
        #endregion

        #region ctor
        public OnlinePaymentLogicImpl(IKPayInvoiceRepository invoiceRepo, ICustomerBankRepository bankRepo, IOrderHistoryHeaderRepsitory orderHistoryrepo,
                                      ICustomerRepository customerRepository, IGenericQueueRepository queueRepo, 
                                      IKPayPaymentTransactionRepository paymentTransactionRepository,
                                      IKPayLogRepository kpayLogRepo, IAuditLogRepository auditLogRepo, IEventLogRepository log,
                                      IInternalUserAccessRepository internalUserAccessRepo, ICacheRepository cacheRepo)
        {
            _log = log;
            _invoiceRepo = invoiceRepo;
            _bankRepo = bankRepo;
            _orderHistoryRepo = orderHistoryrepo;
            _internalUserAccessRepo = internalUserAccessRepo;
            //_catalogLogic = catalogLogic;
            _customerRepository = customerRepository;
            _queue = queueRepo;
            _paymentTransactionRepository = paymentTransactionRepository;
            _kpaylog = kpayLogRepo;
            _auditLog = auditLogRepo;
            _cacheRepo = cacheRepo;
        }
        #endregion

        #region methods
        public InvoiceItemModel AssignContractCategory
            (Dictionary<string, string> contractdictionary, InvoiceItemModel item)
        {
            string itmcategory = ContractInformationHelper.AddContractInformationIfInContract
                (contractdictionary, item.ItemNumber);
            item.Category = itmcategory;
            return item;
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
                inFilter.Filters = new List<FilterInfo>() { new FilterInfo() { Field = "Division", Value = DivisionHelper.GetDivisionFromBranchId(cust.CustomerBranch), FilterType = "eq" },
                                                            new FilterInfo() { Field = "CustomerNumber", Value = cust.CustomerNumber, FilterType = "eq" } };

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
                    if (createdFilter != null)
                        return createdFilter;
                }
            }

            if (passedFilter.Field != null)
            {
                if (passedFilter.Field.Equals("StatusDescription", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MapStatusDecriptionFilterInfo(passedFilter);
                }
                if (passedFilter.Field.Equals(Constants.INVOICEREQUESTFILTER_DATERANGE_YEARQTRKEY, StringComparison.CurrentCultureIgnoreCase))
                {
                    return MapYearQtrDateRangeFilterInfo(passedFilter);
                }
                if (passedFilter.Field.Equals(Constants.INVOICEREQUESTFILTER_DATERANGE_YEARMONTHKEY, StringComparison.CurrentCultureIgnoreCase))
                {
                    return MapYearMonthDateRangeFilterInfo(passedFilter);
                }
                if (passedFilter.Field.Equals(Constants.INVOICEREQUESTFILTER_INVOICENUMBER_FIELDKEY, StringComparison.CurrentCultureIgnoreCase))
                {
                    return new FilterInfo()
                    {
                        Field = Constants.INVOICEREQUESTFILTER_INVOICENUMBER_FIELDKEY,
                        Value = passedFilter.Value.ToUpper(),
                        FilterType = "contains"
                    };
                }
            }

            return null;
        }

        private FilterInfo MapYearMonthDateRangeFilterInfo(FilterInfo passedFilter)
        {
            FilterInfo fi = new FilterInfo();
            fi.Condition = "and";
            fi.Filters = new List<FilterInfo>();
            string year = passedFilter.Value.Substring(0, passedFilter.Value.IndexOf(','));
            string month = passedFilter.Value.Substring(passedFilter.Value.IndexOf(',') + 1);
            DateTime dtStart = DateTime.Parse(month + "/1/" + year);
            DateTime dtEnd = dtStart.AddMonths(1);
            fi.Filters.Add(new FilterInfo() { Field = "InvoiceDate", Value = dtStart.ToShortDateString(), FilterType = "gte" });
            fi.Filters.Add(new FilterInfo() { Field = "InvoiceDate", Value = dtEnd.ToShortDateString(), FilterType = "lt" });
            return fi;
        }

        private FilterInfo MapYearQtrDateRangeFilterInfo(FilterInfo passedFilter)
        {
            FilterInfo fi = new FilterInfo();
            fi.Condition = "and";
            fi.Filters = new List<FilterInfo>();
            string year = passedFilter.Value.Substring(0, passedFilter.Value.IndexOf(','));
            string qtr = passedFilter.Value.Substring(passedFilter.Value.IndexOf(',') + 1);
            DateTime dtStart;
            DateTime dtEnd;
            switch (qtr)
            {
                case "1":
                    dtStart = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ1 + "/" + year);
                    dtEnd = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ2 + "/" + year);
                    break;
                case "2":
                    dtStart = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ2 + "/" + year);
                    dtEnd = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ3 + "/" + year);
                    break;
                case "3":
                    dtStart = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ3 + "/" + year);
                    dtEnd = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ4 + "/" + year);
                    break;
                default:
                    dtStart = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ4 + "/" + year);
                    dtEnd = DateTime.Parse(Constants.INVOICEREQUESTFILTER_DATERANGE_STARTQ1 + "/" + (int.Parse(year) + 1));
                    break;
            }
            fi.Filters.Add(new FilterInfo() { Field = "InvoiceDate", Value = dtStart.ToShortDateString(), FilterType = "gte" });
            fi.Filters.Add(new FilterInfo() { Field = "InvoiceDate", Value = dtEnd.ToShortDateString(), FilterType = "lt" });
            return fi;
        }

        private FilterInfo MapStatusDecriptionFilterInfo(FilterInfo passedFilter)
        {
            switch (passedFilter.Value.ToUpper())
            {
                case "OPEN":
                    return new FilterInfo() { Field = "InvoiceStatus", Value = "O", FilterType = "eq" };
                case "PAID":
                    return new FilterInfo() { Field = "InvoiceStatus", Value = "C", FilterType = "eq" };
                case "PAST DUE":
                    return new FilterInfo() { Field = "InvoiceStatus", Value = "O", FilterType = "eq" };
                case "PENDING":
                    return new FilterInfo() { Field = "InvoiceStatus", Value = "P", FilterType = "eq" };
                default:
                    return null;
            }
        }

        public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber)
        {
            _invoiceRepo.DeleteInvoice(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber);
        }

        public List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext)
        {
            List<EFCustomer.CustomerBank> bankEntities = _bankRepo.GetAllCustomerBanks(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId);

            List<CustomerBank> banks = new List<CustomerBank>();

            foreach (EFCustomer.CustomerBank entity in bankEntities)
            {
                if (entity != null)
                {
                    CustomerBank bank = new CustomerBank();
                    bank.Parse(entity);

                    banks.Add(bank);
                }
            }

            return banks;
        }

        public CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber)
        {
            EFCustomer.CustomerBank bankEntity = _bankRepo.GetBankAccount(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, accountNumber);

            if (bankEntity == null)
                return null;
            else
            {
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

        public InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber)
        {
            var kpayInvoiceHeader = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber);
            var customer = _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);

            if (kpayInvoiceHeader == null) //Invoice not found
                return null;

            //Convert to invoice model
            var invoiceModel = kpayInvoiceHeader.ToInvoiceModel(customer);

            invoiceModel.InvoiceLink = new Uri(Configuration.WebNowUrl.Inject(new { branch = userContext.BranchId, customer = userContext.CustomerId, invoice = invoiceNumber }));

            if (invoiceModel.DueDate < DateTime.Now)
            {
                invoiceModel.Status = InvoiceStatus.PastDue;
                invoiceModel.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(InvoiceStatus.PastDue);
            }

            //Get transactions
            invoiceModel.Transactions = GetInvoiceTransactions(userContext, invoiceNumber);

            return invoiceModel;
        }

        public List<InvoiceTransactionModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber)
        {
            List<InvoiceTransactionModel> transactions = new List<InvoiceTransactionModel>();

            transactions = _invoiceRepo.GetInvoiceTransactoin(DivisionHelper.GetDivisionFromBranchId(userContext.BranchId), userContext.CustomerId, invoiceNumber).Select(t => t.ToTransationModel()).ToList();

            return transactions;
        }

        public InvoiceHeaderReturnModel GetInvoiceHeaders(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers)
        {
            var customers = new List<Core.Models.Profile.Customer>();

            FilterInfo statusFilter = BuildStatusFilter(paging.Filter);

            if (paging.IsNotNullAndHasDateRange())
            {
                if (statusFilter != null)
                { // if there is a statusFilter append the daterange to it, it was working before but pulling back all
                  // the invoices for the month and later filtering just the ones with a certain status
                  // this just optimizes it so we pull back the right ones at the start
                    statusFilter.Condition = "&&";
                    statusFilter.Filters = new List<FilterInfo>();
                    statusFilter.Filters.Add(BuildStatusFilter(paging.DateRange));
                }
                else
                {
                    statusFilter = BuildStatusFilter(paging.DateRange);
                }
            }

            InvoiceHeaderReturnModel retInvoiceHeaders = new InvoiceHeaderReturnModel();
            if (forAllCustomers)
            {
                customers = _customerRepository.GetCustomersForUser(user.UserId);
                if (customers.Count == 0) // in the case of internal users, the relation of customers to users is different, so the above doesn't work for some
                                          // in that case we work with the selected customer
                {
                    customers = new List<Core.Models.Profile.Customer>()
                                            { _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId) };
                }
                retInvoiceHeaders.CustomersWithInvoices = new PagedResults<CustomerWithInvoices>();
                retInvoiceHeaders.CustomersWithInvoices.Results = new List<CustomerWithInvoices>();

                foreach (var customer in customers)
                {
                    if (customer != null)
                    {
                        CustomerWithInvoices ci = customer.ToCustomerWithInvoices();
                        MapInvoicesToCustomer(paging, statusFilter, customer, ci);
                        if (ci.HasPayableInvoices) // we want to "bubble up" if there are payable invoices
                        {
                            retInvoiceHeaders.HasPayableInvoices = ci.HasPayableInvoices;
                        }
                        retInvoiceHeaders.CustomersWithInvoices.Results.Add(ci);
                        retInvoiceHeaders.CustomersWithInvoices.TotalInvoices += ci.PagedResults.TotalInvoices;
                    }
                    retInvoiceHeaders.CustomersWithInvoices.TotalResults = retInvoiceHeaders.CustomersWithInvoices
                                                                                            .Results
                                                                                            .Count;
                }
            }
            else
            {
                customers = MapInvoicesToReturn(userContext, paging, statusFilter, retInvoiceHeaders);
            }
            return retInvoiceHeaders;
        }

        private List<Core.Models.Profile.Customer> MapInvoicesToReturn(UserSelectedContext userContext, PagingModel paging, FilterInfo statusFilter, InvoiceHeaderReturnModel retInvoiceHeaders)
        {
            List<Core.Models.Profile.Customer> customers = new List<Core.Models.Profile.Customer>() { _customerRepository.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId) };

            List<EFInvoice.InvoiceHeader> kpayInvoices;
            retInvoiceHeaders.PagedResults = GetInvoicesForCustomer(paging,
                                                                    customers,
                                                                    statusFilter,
                                                                    out kpayInvoices);

            retInvoiceHeaders.HasPayableInvoices = customers.Any(i => i.KPayCustomer) && kpayInvoices.Count > 0;
            retInvoiceHeaders.TotalAmmountDue = kpayInvoices.Sum(i => i.AmountDue);
            return customers;
        }

        public void MapInvoicesToCustomer(PagingModel paging, FilterInfo statusFilter, Core.Models.Profile.Customer customer, CustomerWithInvoices ci)
        {
            try
            {
                List<Core.Models.Profile.Customer> clist = new List<Core.Models.Profile.Customer>() { customer };
                List<EFInvoice.InvoiceHeader> kpayInvoices;
                ci.PagedResults = GetInvoicesForCustomer(paging,
                                                         clist,
                                                         statusFilter,
                                                         out kpayInvoices);
                ci.HasPayableInvoices = clist.Any(i => i.KPayCustomer) && kpayInvoices.Count > 0;
                ci.TotalAmountDue = kpayInvoices.Sum(i => i.AmountDue);
            }
            catch(Exception ex)
            {
                _log.WriteErrorLog("OnlinePaymentLogicImpl_MapInvoicesToCustomer", ex);
            }
        }

        private string CustomerInvoiceHeadersListCacheKey(Core.Models.Profile.Customer customer, FilterInfo statusFilter)
        {
             return String.Format("InvoiceHeadersList_{0}_{1}_{2}", 
                                  customer.CustomerId,
                                  customer.CustomerBranch, 
                                  JsonConvert.SerializeObject(statusFilter));
        }

        private string InvoiceModelCacheKey(InvoiceModel invoice)
        {
            return String.Format("InvoiceHeader_{0}",
                                 invoice.Id);
        }

        private PagedResults<InvoiceModel> GetInvoicesForCustomer(
            PagingModel paging,
            List<Core.Models.Profile.Customer> customers, // customers will always be a list with 1 customer
            FilterInfo statusFilter,
            out List<EFInvoice.InvoiceHeader> kpayInvoices)
        {
            System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(gettiming: false);
            FilterInfo customerFilter = BuildCustomerFilter(customers);
            stopWatch.Read(_log, "GetInvoicesForCustomer - BuildCustomerFilter");

            kpayInvoices = _invoiceRepo.ReadFilteredHeaders(customerFilter, statusFilter);
            stopWatch.Read(_log, "GetInvoicesForCustomer - ReadFilteredHeaders");

            PagedResults<InvoiceModel> pagedInvoices = kpayInvoices.Select(i => i.ToInvoiceModel(customers.Where(c => c.CustomerNumber.Equals(i.CustomerNumber)).First()))
                                                                   .AsQueryable<InvoiceModel>()
                                                                   .GetPage(paging, defaultSortPropertyName: "InvoiceNumber");
            stopWatch.Read(_log, "GetInvoicesForCustomer - GetPagedResults");

            DecorateInvoiceModels(customers, pagedInvoices);
            stopWatch.Read(_log, "GetInvoicesForCustomer - DecorateInvoiceModels");

            ApplyAfterMarketFiltersToPagedInvoicesAndCounts(paging, pagedInvoices);
            stopWatch.Read(_log, "GetInvoicesForCustomer - ApplyAfterMarketFiltersToPagedInvoicesAndCounts");

            pagedInvoices.TotalInvoices = pagedInvoices.Results.Count();
            stopWatch.Read(_log, "GetInvoicesForCustomer - Count Total Invoices");

            return pagedInvoices;
        }

        private void ApplySortingToPagedInvoices(PagingModel paging, PagedResults<InvoiceModel> pagedInvoices)
        {
            if ((paging.IsNotNullAndHasSort()) &&
                (paging.Sort[0].Field.Equals
                    (Constants.INVOICEREQUESTSORT_INVOICEAMOUNT, StringComparison.CurrentCultureIgnoreCase)))
            {
                if (paging.Sort[0].Order.Equals
                    (Constants.INVOICEREQUESTSORT_INVOICEAMOUNT_ASCENDING, StringComparison.CurrentCultureIgnoreCase))
                {
                    pagedInvoices.Results = pagedInvoices.Results
                                                         .OrderBy(i => i.InvoiceAmount)
                                                         .ToList();
                }
                else
                {
                    pagedInvoices.Results = pagedInvoices.Results
                                                         .OrderByDescending(i => i.InvoiceAmount)
                                                         .ToList();
                }
            }
        }

        private void ApplyAfterMarketFiltersToPagedInvoicesAndCounts(PagingModel paging, PagedResults<InvoiceModel> pagedInvoices)
        {
            if (paging.IsNotNullAndHasSearch())
            {
                if (paging.Search.Field == Constants.INVOICEREQUESTFILTER_PONUMBER_FIELDKEY)
                {
                    ApplyPONumberFilter(paging, pagedInvoices);
                }
                else if (paging.Search.Field == Constants.INVOICEREQUESTFILTER_TYPEDESCRIPTION_FIELDKEY)
                {
                    ApplyTypeDescriptionFilter(paging, pagedInvoices);
                }
                else if (paging.Search.Field == Constants.INVOICEREQUESTFILTER_CREDITMEMO_FIELDKEY)
                {
                    ApplyCreditMemoFilter(paging, pagedInvoices);
                }
                pagedInvoices.TotalResults = pagedInvoices.Results.Count;
            }

            pagedInvoices.TotalInvoices = pagedInvoices.Results.Count;
        }

        private void ApplyCreditMemoFilterToPagedInvoices(PagingModel paging, PagedResults<InvoiceModel> pagedInvoices)
        {
            if ((paging.IsNotNullAndHasNestedFilters()) && 
                (paging.Filter.Filters.Where
                    (f => f.Field == Constants.INVOICEREQUESTFILTER_CREDITMEMO_FIELDKEY).Count() > 0))
            {
                ApplyCreditMemoFilter(paging, pagedInvoices);
                pagedInvoices.TotalResults = pagedInvoices.Results.Count;
            }
        }

        private PagedResults<InvoiceModel> GetPagedInvoices(PagingModel paging, List<Core.Models.Profile.Customer> customers, List<EFInvoice.InvoiceHeader> kpayInvoices)
        {
            PagedResults<InvoiceModel> pagedInvoices = kpayInvoices.Select(i => i.ToInvoiceModel(customers.Where(c => c.CustomerNumber.Equals(i.CustomerNumber)).First()))
                                                                   .AsQueryable<InvoiceModel>()
                                                                   .GetPage(paging, defaultSortPropertyName: "InvoiceNumber");

            DecorateInvoiceModels(customers, pagedInvoices);
            return pagedInvoices;
        }

        private List<EFInvoice.InvoiceHeader> GetFilteredInvoiceHeaders(List<Core.Models.Profile.Customer> customers, FilterInfo statusFilter)
        {
            List<EFInvoice.InvoiceHeader> kpayInvoices;
            FilterInfo customerFilter = BuildCustomerFilter(customers);
            List<EFInvoice.InvoiceHeader> cachedInvoiceHeaders = _cacheRepo.GetItem<List<EFInvoice.InvoiceHeader>>
              (CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, CustomerInvoiceHeadersListCacheKey(customers[0], statusFilter));
            if (cachedInvoiceHeaders == null)
            {
                kpayInvoices = _invoiceRepo.ReadFilteredHeaders(customerFilter, statusFilter);
                _cacheRepo.AddItem<List<EFInvoice.InvoiceHeader>>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME,
                    CustomerInvoiceHeadersListCacheKey(customers[0], statusFilter), TimeSpan.FromMinutes(10),
                    kpayInvoices);
            }
            else
            {
                kpayInvoices = cachedInvoiceHeaders;
            }

            return kpayInvoices;
        }

        private void DecorateInvoiceModels(List<Core.Models.Profile.Customer> customers, PagedResults<InvoiceModel> pagedInvoices)
        {
            System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(gettiming: false);
            foreach (var invoice in pagedInvoices.Results)
            {
                InvoiceModel cachedInvoice = _cacheRepo.GetItem<InvoiceModel>
                  (CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, InvoiceModelCacheKey(invoice));
                stopWatch.Read(_log, "GetInvoicesForCustomer - GetCachedInvoice");

                if (cachedInvoice == null)
                {
                    DecorateNewInvoiceModel(customers, invoice);
                    stopWatch.Read(_log, "GetInvoicesForCustomer - DecorateNewInvoiceModel");

                    _cacheRepo.AddItem<InvoiceModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME,
                        InvoiceModelCacheKey(invoice), TimeSpan.FromMinutes(10),
                        invoice);
                    stopWatch.Read(_log, "GetInvoicesForCustomer - Add To Cache");
                }
                else
                {
                    RestoreCachedInvoiceModel(invoice, cachedInvoice);
                    stopWatch.Read(_log, "GetInvoicesForCustomer - RestoreCachedInvoiceModel");
                }
            }
        }

        private void RestoreCachedInvoiceModel(InvoiceModel invoice, InvoiceModel cachedInvoice)
        {
            // add link
            invoice.InvoiceLink = cachedInvoice.InvoiceLink;
            // map customer info
            invoice.CustomerCity = cachedInvoice.CustomerCity;
            invoice.CustomerPostalCode = cachedInvoice.CustomerPostalCode;
            invoice.CustomerRegionCode = cachedInvoice.CustomerRegionCode;
            invoice.CustomerStreetAddress = cachedInvoice.CustomerRegionCode;
            // get pending transaction
            invoice.PendingTransaction = cachedInvoice.PendingTransaction;
            // get transactions
            invoice.InvoiceAmount = cachedInvoice.InvoiceAmount;
            invoice.HasCreditMemos = cachedInvoice.HasCreditMemos;
            // get po number
            invoice.PONumber = cachedInvoice.PONumber;
            // get banks
            invoice.Banks = cachedInvoice.Banks;
            // check for credit memos and get invoice amount
            GetInvoiceTransactions(invoice);
        }

        private void DecorateNewInvoiceModel(List<Core.Models.Profile.Customer> customers, InvoiceModel invoice)
        {
            System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(gettiming: false);
            AddInvoiceLink(invoice);
            stopWatch.Read(_log, "DecorateNewInvoiceModel - AddInvoiceLink");

            MapCustomerInfoToInvoice(invoice);
            stopWatch.Read(_log, "DecorateNewInvoiceModel - MapCustomerInfoToInvoice");

            if (invoice.Status == InvoiceStatus.Pending)
            {
                GetPendingTransactionForInvoice(customers, invoice);
                stopWatch.Read(_log, "DecorateNewInvoiceModel - GetPendingTransactionForInvoice");
            }

            GetInvoiceTransactions(invoice);
            stopWatch.Read(_log, "DecorateNewInvoiceModel - GetInvoiceTransactions");

            GetInvoicePONumber(invoice);
            stopWatch.Read(_log, "DecorateNewInvoiceModel - GetInvoicePONumber");

            // To help the UI, we pull in the bank accounts that can be used to pay an invoice here.
            invoice.Banks = GetAllBankAccounts
                (new UserSelectedContext() { BranchId = invoice.BranchId, CustomerId = invoice.CustomerNumber });
            stopWatch.Read(_log, "DecorateNewInvoiceModel - GetAllBankAccounts");
        }

        private void AddInvoiceLink(InvoiceModel invoice)
        {
            invoice.InvoiceLink = new Uri(Configuration.WebNowUrl.Inject(new
            {
                branch = invoice.BranchId,
                customer = invoice.CustomerNumber,
                invoice = invoice.InvoiceNumber
            }));
        }

        private void ApplyCreditMemoFilter(PagingModel paging, PagedResults<InvoiceModel> pagedInvoices)
        {
            if (paging.Search.Value.Equals
                (Constants.INVOICEREQUESTFILTER_CREDITMEMO_VALUECMONLY, StringComparison.CurrentCultureIgnoreCase))
            {
                pagedInvoices.Results = pagedInvoices.Results.Where(i => i.HasCreditMemos).ToList();
            }
        }

        private void ApplyPONumberFilter(PagingModel paging, PagedResults<InvoiceModel> pagedInvoices)
        {
            var filter = paging.Search;
            pagedInvoices.Results = pagedInvoices.Results.Where(i => i.PONumber == filter.Value).ToList();
        }

        private void ApplyTypeDescriptionFilter(PagingModel paging, PagedResults<InvoiceModel> pagedInvoices)
        {
            var filter = paging.Search;
            pagedInvoices.Results = pagedInvoices.Results.Where(i => i.TypeDescription.ToLower().StartsWith(filter.Value.ToLower())).ToList();
        }

        private void GetInvoicePONumber(InvoiceModel invoice)
        {
            var orderHistory = _orderHistoryRepo.ReadForInvoiceHeader(invoice.BranchId, invoice.InvoiceNumber)
                                                .FirstOrDefault();
            if (orderHistory != null)
            {
                invoice.PONumber = orderHistory.PONumber;
            }
        }

        private void GetInvoiceTransactions(InvoiceModel invoice)
        {
            var transactions = _invoiceRepo.GetInvoiceTransactoin(DivisionHelper.GetDivisionFromBranchId(invoice.BranchId), invoice.CustomerNumber, invoice.InvoiceNumber);
            invoice.InvoiceAmount = transactions.Where(x => x.InvoiceType == KeithLink.Svc.Core.Constants.INVOICETRANSACTIONTYPE_INITIALINVOICE)
                                                .Select(x => x.AmountDue)
                                                .FirstOrDefault();

            if (transactions.Where(x => x.InvoiceType == KeithLink.Svc.Core.Constants.INVOICETRANSACTIONTYPE_CREDITMEMO)
                           .Count() > 0)
            {
                invoice.HasCreditMemos = true;
            }
            else
            {
                invoice.HasCreditMemos = false;
            }
        }

        private void GetPendingTransactionForInvoice(List<Core.Models.Profile.Customer> customers, InvoiceModel invoice)
        {
            //Retrieve payment transaction record
            var payment = _paymentTransactionRepository.ReadAll()
                                                       .Where(p => p.Division.Equals(DivisionHelper.GetDivisionFromBranchId(invoice.BranchId)) &&
                                                                   p.CustomerNumber.Equals(invoice.CustomerNumber) &&
                                                                   p.InvoiceNumber.Equals(invoice.InvoiceNumber))
                                                       .FirstOrDefault();
            if (payment != null)
            {
                invoice.PendingTransaction = payment.ToPaymentTransactionModel(customers.Where(c => c.CustomerNumber.Equals(invoice.CustomerNumber) &&
                                                                                                    c.CustomerBranch.Equals(invoice.BranchId, StringComparison.InvariantCultureIgnoreCase))
                                                                                        .FirstOrDefault(),
                                                                               Configuration.BillPayCutOffTime);
            }
        }

        private void MapCustomerInfoToInvoice(InvoiceModel invoice)
        {
            var customer = _customerRepository.GetCustomerByCustomerNumber(invoice.CustomerNumber, invoice.BranchId);
            invoice.CustomerStreetAddress = customer.Address.StreetAddress;
            invoice.CustomerCity = customer.Address.City;
            invoice.CustomerRegionCode = customer.Address.RegionCode;
            invoice.CustomerPostalCode = customer.Address.PostalCode;
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

        public void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments)
        {
            // To be safe; reset all cacheitems in the invoice headers group upon paying invoices
            _cacheRepo.ResetAllItems(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME);

            _kpaylog.Write(emailAddress, string.Format("Paying invoices for customer ({0}-{1})", userContext.BranchId, userContext.CustomerId));

            var confId = _invoiceRepo.GetNextConfirmationId();

            if(payments.Where(p => p.PaymentAmount == 0).Any()) {
                throw new ApplicationException("$0 payments are not allowed");
            }

            var transactionErrors = ValidatePayment(userContext, payments);

            if(transactionErrors.Count > 0) {
                throw new ApplicationException("Payments failed validation");
            }

            foreach (var payment in payments)
            {
                var testInvoice = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(payment.BranchId), payment.CustomerNumber, payment.InvoiceNumber);
                if (testInvoice == null)
                {
                    throw new ApplicationException("Could find invoice for selected customer");
                }

                if (!payment.PaymentDate.HasValue) { payment.PaymentDate = DateTime.Now; }
                payment.ConfirmationId = (int)confId;

                _invoiceRepo.PayInvoice(new Core.Models.OnlinePayments.Payment.EF.PaymentTransaction()
                {
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
            notification.SubmittedBy = emailAddress;

            // write the payment notification to Rabbit MQ
            _queue.PublishToDirectedExchange(notification.ToJson(), Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNamePublisher,
                                  Configuration.RabbitMQNotificationUserPasswordPublisher, Configuration.RabbitMQVHostNotification, 
                                  Configuration.RabbitMQExchangeNotificationV2, Constants.RABBITMQ_NOTIFICATION_PAYMENTNOTIFICATION_ROUTEKEY);
        }

        public PagedResults<PaymentTransactionModel> PendingTransactionsAllCustomers(UserProfile user, PagingModel paging)
        {
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

        public PagedResults<PaymentTransactionModel> PendingTransactions(UserSelectedContext customer, PagingModel paging)
        {
            var currentCustomer = _customerRepository.GetCustomerByCustomerNumber(customer.CustomerId, customer.BranchId);
            var transaction = _paymentTransactionRepository.ReadAllByCustomer(customer.CustomerId, DivisionHelper.GetDivisionFromBranchId(customer.BranchId));
            var transactionModels = transaction.Select(t => t.ToPaymentTransactionModel(currentCustomer, Configuration.BillPayCutOffTime));
            var retVal = transactionModels.AsQueryable()
                                          .GetPage(paging);

            return retVal;
        }

        public List<PaymentTransactionModel> ValidatePayment(UserSelectedContext context, List<PaymentTransactionModel> payments)
        {
            List<PaymentTransactionModel> returnValue = new List<PaymentTransactionModel>();

            // Set default payment date for null payments to today.
            payments.ForEach(x => { if (!x.PaymentDate.HasValue) { x.PaymentDate = DateTime.Now; } });

            // Select all transactions grouped by accountnumber, paymentdate
            var transactions =
                from t in payments
                group t by new
                {
                    t.AccountNumber,
                    t.PaymentDate,
                } into tg
                select new
                {
                    AccountNumber = tg.Key.AccountNumber,
                    PaymentDate = tg.Key.PaymentDate.Value.ToShortDateString(),
                    payments = tg.ToList()
                };

            // Go through all daily transactions and validate the sum payment amount is greater than 0 (not negative)
            foreach (var t in transactions)
            {
                if (t.payments.Sum(x => x.PaymentAmount) < 0)
                {
                    returnValue.AddRange(t.payments);
                }
            }

            return returnValue;
        }

        #endregion
    }
}
