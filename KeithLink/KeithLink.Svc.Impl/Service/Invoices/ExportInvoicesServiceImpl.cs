﻿using KeithLink.Svc.Core.Extensions.Orders;
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

namespace KeithLink.Svc.Impl.Service.Invoices
{
    public class ExportInvoicesServiceImpl : IExportInvoicesService
    {
        #region " attributes "
        private readonly ICacheRepository _cache;
        private readonly IExportSettingLogic _exportLogic;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderLogic _orderLogic;
        private readonly IOnlinePaymentsLogic _invLogic;
        private readonly IListRepository _listRepo;
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
        public ExportInvoicesServiceImpl(IExportSettingLogic exportLogic, IOrderLogic orderLogic, IListRepository listRepo,
                                         ICacheRepository cache, IOnlinePaymentsLogic invLogic, IKPayInvoiceRepository invoiceRepo,
                                         ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _exportLogic = exportLogic;
            _orderLogic = orderLogic;
            _invLogic = invLogic;
            _listRepo = listRepo;
            _invoiceRepo = invoiceRepo;
            _cache = cache;
        }
        #endregion

        #region " methods "
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

        public InvoiceModel GetExportableInvoice(UserProfile user,
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
        #endregion
    }
}