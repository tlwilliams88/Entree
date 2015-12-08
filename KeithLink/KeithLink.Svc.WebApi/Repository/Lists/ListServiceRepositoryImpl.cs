using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using KeithLink.Svc.Core.Models.Customers.EF;

namespace KeithLink.Svc.WebApi.Repository.Lists
{
    public class ListServiceRepositoryImpl : IListServiceRepository
    {
        #region attributes
        private com.benekeith.ListService.IListServcie serviceClient;
        protected IUserProfileLogic _profileLogic;
        #endregion

        #region ctor
        public ListServiceRepositoryImpl(com.benekeith.ListService.IListServcie serviceClient, IUserProfileLogic UserProfileLogic)
        {
            this.serviceClient = serviceClient;
            _profileLogic = UserProfileLogic;
        }
        #endregion

        #region methods
        public long? AddItem(long listId, ListItemModel item)
        {
            return serviceClient.AddItem(listId, item);
        }

        public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items)
        {
            return serviceClient.AddItems(user, catalogInfo, listId, items.ToArray());
        }

        public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote)
        {
            serviceClient.AddNote(user, catalogInfo, newNote);
        }

        public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
        {
            serviceClient.AddRecentlyViewedItem(user, catalogInfo, itemNumber);
        }

        public List<ListCopyResultModel> CopyList(ListCopyShareModel copyListModel)
        {
            var result = serviceClient.CopyList(copyListModel);
            return result == null ? new List<ListCopyResultModel>() : result.ToList();
        }

        public long CreateList(Guid? userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
        {
            return serviceClient.CreateList(userId, catalogInfo, list, type);
        }

        public void DeleteItem(long Id)
        {
            serviceClient.DeleteItem(Id);
        }

        public void DeleteItemNumberFromList(long Id, string itemNumber)
        {
            serviceClient.DeleteItemNumberFromList(Id, itemNumber);
        }

        public void DeleteItems(List<long> itemIds)
        {
            foreach (var Id in itemIds)
                serviceClient.DeleteItem(Id);
        }

        public void DeleteList(long Id)
        {
            serviceClient.DeleteList(Id);
        }

        public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber)
        {
            serviceClient.DeleteNote(user, catalogInfo, ItemNumber);
        }

        public void DeleteLists(List<long> listIds)
        {
            foreach (var Id in listIds)
                serviceClient.DeleteList(Id);
        }

        public List<Core.Models.Reports.ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            var list = serviceClient.GetBarcodeForList(user, catalogInfo, Id);

            if (list == null)
                return null;
            return list.ToList();
        }

        public List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers)
        {
            var list = serviceClient.ItemsInHistoryList(catalogInfo, itemNumbers.ToArray());

            if (list == null)
                return new List<InHistoryReturnModel>();
            return list.ToList();
        }

        public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo)
        {
            return serviceClient.ReadFavorites(user, catalogInfo).ToList();
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool includePrice = true)
        {
            return serviceClient.ReadList(user, catalogInfo, Id, includePrice);
        }

        public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false)
        {
            return serviceClient.ReadListByType(user, catalogInfo, type, headerOnly).ToList();
        }

        public List<string> ReadListLabels(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo)
        {
            return serviceClient.ReadListLabels(user, catalogInfo).ToList();
        }

        public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
        {
            return serviceClient.ReadNotes(user, catalogInfo).ToList();
        }

        public PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, Core.Models.Paging.PagingModel paging)
        {
            return serviceClient.ReadPagedList(user, catalogInfo, Id, paging);
        }

        public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
        {
            return serviceClient.ReadRecent(user, catalogInfo).ToList();
        }

        public List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo)
        {
            return serviceClient.ReadRecommendedItemsList(catalogInfo).ToList();
        }

        public List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo)
        {
            return serviceClient.ReadReminders(user, catalogInfo).ToList();
        }

        public List<ListModel> ReadUserList(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            var list = serviceClient.ReadUserList(user, catalogInfo, headerOnly);

            if (list == null)
                return null;
            return list.ToList();
        }

        public void ShareList(ListCopyShareModel shareListModel)
        {
            serviceClient.ShareList(shareListModel);
        }

        public void UpdateItem(ListItemModel item)
        {
            serviceClient.UpdateItem(item);
        }

        public void UpdateList(ListModel userList)
        {
            serviceClient.UpdateList(userList);
        }

        //public Stream BuildReportFromList(PrintListModel options, long listId, ListReportModel printModel, UserSelectedContext userContext, UserProfile userProfile)
        //{
        //    return serviceClient.BuildReportFromList(options, listId, printModel, userContext, userProfile);
        //}
        public Stream BuildReportFromList(PrintListModel options, long listId, UserSelectedContext userContext, UserProfile userProfile)
        {
            if (!string.IsNullOrEmpty(options.Paging.Terms))
            {
                //Build filter
                options.Paging.Filter = new FilterInfo()
                {
                    Field = "ItemNumber",
                    FilterType = "contains",
                    Value = options.Paging.Terms,
                    Condition = "||",
                    Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = options.Paging.Terms, FilterType = "contains" }, 
                        new FilterInfo() { Condition = "||", Field = "Name", Value = options.Paging.Terms, FilterType = "contains" } }
                };
            }

            options.Paging.Size = int.MaxValue;
            options.Paging.From = 0;

            if (options.Paging.Sort.Count == 1 && options.Paging.Sort[0].Field == null)
            {
                options.Paging.Sort = new List<SortInfo>();
            }

            ListModel list = ReadList(userProfile, userContext, listId, true);

            if (list == null)
                return null;

            ListReportModel printModel = list.ToReportModel();

            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            string deviceInfo = KeithLink.Svc.Core.Constants.SET_REPORT_SIZE_LANDSCAPE;
            Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
            // HACK for dynamically changing column widths doesn't work in run-time reportviewer.  choosing from multiple reports.
            Stream rdlcStream = assembly.GetManifestResourceStream(ChooseReportFromOptions(options, userContext));
            rv.LocalReport.LoadReportDefinition(rdlcStream);
            rv.LocalReport.SetParameters(MakeReportOptionsForPrintListReport(options, printModel.Name, userContext));
            GatherInfoAboutItems(listId, options, printModel, userContext, userProfile);
            rv.LocalReport.DataSources.Add(new ReportDataSource("ListItems", printModel.Items));
            byte[] bytes = rv.LocalReport.Render("PDF", deviceInfo);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        private string ChooseReportFromOptions(PrintListModel options, UserSelectedContext userContext)
        { // Choose different Report for different columns ; grouping doesn't change column widths so no different name
            Customer customer = _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
            if ((options.ShowParValues) & (customer.CanViewPricing) & (options.ShowNotes))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParYesPriceYesNotes;
            else if ((options.ShowParValues) & (customer.CanViewPricing) & (options.ShowNotes == false))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParYesPriceNoNotes;
            else if ((options.ShowParValues) & (customer.CanViewPricing == false) & (options.ShowNotes))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParNoPriceYesNotes;
            else if ((options.ShowParValues) & (customer.CanViewPricing == false) & (options.ShowNotes == false))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParNoPriceNoNotes;
            else if ((options.ShowParValues == false) & (customer.CanViewPricing) & (options.ShowNotes))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParYesPriceYesNotes;
            else if ((options.ShowParValues == false) & (customer.CanViewPricing) & (options.ShowNotes == false))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParYesPriceNoNotes;
            else if ((options.ShowParValues == false) & (customer.CanViewPricing == false) & (options.ShowNotes))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceYesNotes;
            else if ((options.ShowParValues == false) & (customer.CanViewPricing == false) & (options.ShowNotes == false))
                return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceNoNotes;
            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST;
        }

        private ReportParameter[] MakeReportOptionsForPrintListReport(PrintListModel options, string listName, UserSelectedContext userContext)
        {
            Customer customer = _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
            ReportParameter[] parameters = new ReportParameter[5];
            parameters[0] = new ReportParameter("ListName", customer.CustomerName + ", " + listName);
            parameters[1] = new ReportParameter("ShowNotes", options.ShowNotes.ToString());
            parameters[2] = new ReportParameter("ShowPar", options.ShowParValues.ToString());
            bool groupbylabel = false;
            if ((options.Paging != null) && (options.Paging.Sort != null) && (options.Paging.Sort.Count > 0) &&
                (options.Paging.Sort[0].Field.Equals("label", StringComparison.CurrentCultureIgnoreCase)))
            {
                groupbylabel = true;
            }
            parameters[3] = new ReportParameter("GroupByLabel", (groupbylabel).ToString());
            parameters[4] = new ReportParameter("ShowPrices", customer.CanViewPricing.ToString());
            return parameters;
        }

        private void GatherInfoAboutItems(long listId, PrintListModel options, ListReportModel printModel, UserSelectedContext userContext, UserProfile userProfile)
        {
            var listModel = ReadList(userProfile, userContext, listId, true);
            var itemHash = listModel.Items.ToDictionary(p => p.ItemNumber);
            var container = DependencyMap.GetContainer();
            IItemHistoryRepository _itemHistoryRepository = container.Resolve<IItemHistoryRepository>();
            List<ItemHistory> itemStatistics = _itemHistoryRepository
                                                   .Read(f => f.BranchId.Equals(userContext.BranchId) && f.CustomerNumber.Equals(userContext.CustomerId))
                                                   .Where(f => itemHash.Keys.Contains(f.ItemNumber))
                                                   .ToList();
            foreach (ListItemReportModel item in printModel.Items)
            {
                StringBuilder priceInfo = new StringBuilder();
                var itemInfo = itemHash[item.ItemNumber];
                if (itemInfo.PackagePrice.Equals("0.00") == false)
                {
                    priceInfo.Append("$");
                    priceInfo.Append(itemInfo.PackagePrice);
                    priceInfo.Append("/Pack");
                    priceInfo.Append(" - ");
                }
                priceInfo.Append("$");
                priceInfo.Append(itemInfo.CasePrice);
                priceInfo.Append("/Case");
                item.Price = priceInfo.ToString();
                // HACK to make the option not to sort by label not reorder the items
                if ((options.Paging != null) && (options.Paging.Sort != null) && (options.Paging.Sort.Count > 0) &&
                    (options.Paging.Sort[0].Field.Equals("label", StringComparison.CurrentCultureIgnoreCase)))
                {
                    item.Label = itemInfo.Label;
                }
                else
                {
                    item.Label = null;
                }
                ItemHistory itemStats = itemStatistics.Where(f => f.ItemNumber == item.ItemNumber).FirstOrDefault();
                if (itemStats != null)
                {
                    string AVG8WK = "";
                    AVG8WK += itemStats.AverageUse;
                    if (itemStats.UnitOfMeasure.Equals(KeithLink.Svc.Core.Constants.ITEMHISTORY_AVERAGEUSE_PACKAGE)) AVG8WK += " Pack";
                    else if (itemStats.UnitOfMeasure.Equals(KeithLink.Svc.Core.Constants.ITEMHISTORY_AVERAGEUSE_CASE)) AVG8WK += " Case";
                    if ((itemStats.AverageUse > 1) | (itemStats.AverageUse == 0)) AVG8WK += "s";
                    item.AvgUse = AVG8WK;
                }
            }
        }
        #endregion
    }
}
