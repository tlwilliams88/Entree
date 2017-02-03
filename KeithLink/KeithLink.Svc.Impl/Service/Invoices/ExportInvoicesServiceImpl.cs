using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.OnlinePayments;
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

namespace KeithLink.Svc.Impl.Service.Invoices
{
    public class ExportInvoicesServiceImpl : IExportInvoicesService
    {
        #region " attributes "
        private readonly ICacheRepository _cache;
        private readonly IExportSettingLogic _exportLogic;
        private readonly IOrderLogic _orderLogic;
        private readonly IOnlinePaymentsLogic _invLogic;
        private readonly IListRepository _listRepo;
        #endregion

        #region " constructor "
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="exportLogic"></param>
        /// <param name="orderLogic"></param>
        /// <param name="listRepo"></param>
        /// <param name="cache"></param>
        public ExportInvoicesServiceImpl(IExportSettingLogic exportLogic, IOrderLogic orderLogic, IListRepository listRepo,
                                         ICacheRepository cache, IOnlinePaymentsLogic invLogic)
        {
            _exportLogic = exportLogic;
            _orderLogic = orderLogic;
            _invLogic = invLogic;
            _listRepo = listRepo;
            _cache = cache;
        }
        #endregion

        #region " methods "
        public List<InvoiceItemModel> GetExportableInvoiceItems(UserProfile user,
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

            Order order = _orderLogic.GetOrder(context.BranchId, invoiceNumber);
            List<InvoiceItemModel> items = order.Items.Select(i => i.ToInvoiceItem()).ToList();

            Dictionary<string, string> contractdictionary = 
                ContractInformationHelper.GetContractInformation(context, _listRepo, _cache);
            items = items.Select(i => _invLogic.AssignContractCategory(contractdictionary, i)).ToList();

            return items;
        }
        #endregion
    }
}
