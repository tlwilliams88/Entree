using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Helpers;
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.Impl.Service.Invoices
{
    public class ExportInvoicesServiceImpl : IExportInvoicesService
    {
        #region " attributes "
        private readonly IExportSettingLogic _exportLogic;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderLogic _orderLogic;
        private readonly IOnlinePaymentsLogic _invLogic;
        private readonly IKPayInvoiceRepository _invoiceRepo;
        #endregion

        #region " constructor "
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="exportLogic"></param>
        /// <param name="orderLogic"></param>
        /// <param name="listRepo"></param>
        /// <param name="cache"></param>
        /// <param name="invLogic"></param>
        public ExportInvoicesServiceImpl(IExportSettingLogic exportLogic, IOrderLogic orderLogic, 
                                         IOnlinePaymentsLogic invLogic, IKPayInvoiceRepository invoiceRepo,
                                         ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _exportLogic = exportLogic;
            _orderLogic = orderLogic;
            _invLogic = invLogic;
            _invoiceRepo = invoiceRepo;
        }
        #endregion

        #region " methods "
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <param name="exportRequest"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="contractdictionary"></param>
        /// <returns></returns>
        public List<InvoiceItemModel> GetExportableInvoiceItems(UserProfile user,
                                                                UserSelectedContext context,
                                                                ExportRequestModel exportRequest,
                                                                string invoiceNumber,
                                                                Dictionary<string, string> contractdictionary)
        {
            if (exportRequest.Fields != null)
                _exportLogic.SaveUserExportSettings(user.UserId,
                                                    Core.Models.Configuration.EF.ExportType.InvoiceDetail,
                                                    KeithLink.Svc.Core.Enumerations.List.ListType.Custom,
                                                    exportRequest.Fields,
                                                    exportRequest.SelectedType);

            Order order = _orderLogic.GetOrder(context.BranchId, invoiceNumber);

            List<InvoiceItemModel> items = order.Items.Select(i => i.ToInvoiceItem()).ToList();

            items = items.Select(i => _invLogic.AssignContractCategory(contractdictionary, i)).ToList();

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <param name="exportRequest"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="contractdictionary"></param>
        /// <returns></returns>
        public InvoiceModel GetExportableInvoice(UserProfile user,
                                                 UserSelectedContext context,
                                                 ExportRequestModel exportRequest,
                                                 string invoiceNumber)
        {
            if (exportRequest.Fields != null)
                _exportLogic.SaveUserExportSettings(user.UserId,
                                                    Core.Models.Configuration.EF.ExportType.InvoiceDetail,
                                                    KeithLink.Svc.Core.Enumerations.List.ListType.Custom,
                                                    exportRequest.Fields,
                                                    exportRequest.SelectedType);

            var kpayInvoiceHeader = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(context.BranchId), context.CustomerId, invoiceNumber);
            var customer = _customerRepository.GetCustomerByCustomerNumber(context.CustomerId, context.BranchId);

            if (kpayInvoiceHeader == null) //Invoice not found
                return null;

            //Convert to invoice model
            var invoiceModel = kpayInvoiceHeader.ToInvoiceModel(customer);

            if (invoiceModel.DueDate < DateTime.Now)
            {
                invoiceModel.Status = InvoiceStatus.PastDue;
                invoiceModel.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(InvoiceStatus.PastDue);
            }

            return invoiceModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <param name="forAllCustomers"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetExportableInvoiceModels(UserProfile user, UserSelectedContext context, ExportRequestModel request, bool forAllCustomers)
        {
            PagingModel myPaging = new PagingModel() {
                Filter = request.Filter,
                DateRange = request.DateRange
            };
            if (request.Sort != null) {
                myPaging.Sort.Add(request.Sort);
            }

            var list = _invLogic.GetInvoiceHeaders(user, context, myPaging, forAllCustomers);

            if (request.Fields != null)
                _exportLogic.SaveUserExportSettings(user.UserId, Core.Models.Configuration.EF.ExportType.Invoice, 0, request.Fields, request.SelectedType);

            List<InvoiceModel> exportData = new List<InvoiceModel>();

            if (list != null && 
                list.PagedResults != null &&
                list.PagedResults.Results != null &&
                list.PagedResults.Results.Count > 0) {
                exportData.AddRange(list.PagedResults.Results);
            }

            if (list != null && 
                list.CustomersWithInvoices != null &&
                list.CustomersWithInvoices.Results != null &&
                list.CustomersWithInvoices.Results.Count > 0) {
                foreach (var customer in list.CustomersWithInvoices.Results)
                {
                    exportData.AddRange(customer.PagedResults.Results);
                }
            }

            return exportData;
        }

        #endregion
    }
}