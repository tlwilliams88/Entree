using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace KeithLink.Svc.WebApi.Helpers
{
    public class ListPrintHelper
    {
        public static Stream BuildReportFromList(PrintListModel options, long listId, UserSelectedContext userContext,
            UserProfile userProfile, IListLogic _listLogic, IUserProfileLogic _profileLogic, IEventLogRepository _elRepo)
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

            ListModel list = _listLogic.ReadList(userProfile, userContext, listId, options.ShowPrices);
            _elRepo.WriteInformationLog(string.Format("BuildReportFromList {0}", JsonConvert.SerializeObject(list)));

            if (list == null)
                return null;

            StringBuilder sortinfo = new StringBuilder();
            foreach (SortInfo si in options.Paging.Sort)
            {
                if (sortinfo.Length > 0)
                    sortinfo.Append(";");
                sortinfo.Append(si.Field);
                sortinfo.Append(",");
                sortinfo.Append(si.Order);
            }
            list.Items = SortOrderItems(sortinfo.ToString(), list.Items);
            int ind = 1;
            foreach (ListItemModel item in list.Items)
            {
                item.Position = ind++;
            }

            ListReportModel printModel = list.ToReportModel();

            Customer customer = _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);

            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            string deviceInfo = KeithLink.Svc.Core.Constants.SET_REPORT_SIZE_LANDSCAPE;
            Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
            string rptName = ChooseReportFromOptions(options, userContext, customer);
            Stream rdlcStream = assembly.GetManifestResourceStream(rptName);
            rv.LocalReport.LoadReportDefinition(rdlcStream);
            rv.LocalReport.SetParameters
                (MakeReportOptionsForPrintListReport(options, printModel.Name, userContext, customer));
            GatherInfoAboutItems(listId, options, printModel, userContext, userProfile, customer, _listLogic);
            _elRepo.WriteInformationLog
                (string.Format("BuildReportFromList_reportdata {0}", JsonConvert.SerializeObject(printModel.Items)));
            rv.LocalReport.DataSources.Add(new ReportDataSource("ListItems", printModel.Items));
            byte[] bytes = rv.LocalReport.Render("PDF", deviceInfo);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        ///// <summary>
        ///// Sort list items given an unparsed string with sort information
        ///// </summary>
        ///// <param name="sortinfo">A string with unparsed sort info for the list items (of the form "fld1,ord1[;fld2,ord2]")</param>
        ///// <param name="items">A list of shopping cart items</param>
        ///// <returns>A list of shopping cart items in the order described by sortinfo with position calibrated to that sort</returns>
        private static List<ListItemModel> SortOrderItems(string sortinfo, List<ListItemModel> items)
        {
            IQueryable<ListItemModel> stmt = items.AsQueryable();
            string[] sortpairs = sortinfo.Split(';');
            int ind = 0;
            foreach (string sortpair in sortpairs)
            {
                string fld = sortpair.Substring(0, sortpair.IndexOf(','));
                string ord = sortpair.Substring(sortpair.IndexOf(',') + 1);
                stmt = stmt.OrderingHelper(fld, ord.Equals("desc", StringComparison.CurrentCultureIgnoreCase), ind > 0);
                ind++;
            }
            return stmt.ToList();
        }

        private static string ChooseReportFromOptions(PrintListModel options, UserSelectedContext userContext, Customer customer)
        { // Choose different Report for different columns ; grouping doesn't change column widths so no different name
            if (customer != null)
            {
                if ((options.ShowParValues) & (options.ShowPrices) & (options.ShowNotes))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParYesPriceYesNotes;
                else if ((options.ShowParValues) & (options.ShowPrices) & (options.ShowNotes == false))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParYesPriceNoNotes;
                else if ((options.ShowParValues) & (options.ShowPrices == false) & (options.ShowNotes))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParNoPriceYesNotes;
                else if ((options.ShowParValues) & (options.ShowPrices == false) & (options.ShowNotes == false))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParNoPriceNoNotes;
                else if ((options.ShowParValues == false) & (options.ShowPrices) & (options.ShowNotes))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParYesPriceYesNotes;
                else if ((options.ShowParValues == false) & (options.ShowPrices) & (options.ShowNotes == false))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParYesPriceNoNotes;
                else if ((options.ShowParValues == false) & (options.ShowPrices == false) & (options.ShowNotes))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceYesNotes;
                else if ((options.ShowParValues == false) & (options.ShowPrices == false) & (options.ShowNotes == false))
                    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceNoNotes;
            }
            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceNoNotes;
        }

        private static ReportParameter[] MakeReportOptionsForPrintListReport(PrintListModel options, string listName, UserSelectedContext userContext,
            Customer customer)
        {
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
            parameters[4] = new ReportParameter("ShowPrices", options.ShowPrices.ToString());
            return parameters;
        }

        private static void GatherInfoAboutItems(long listId, PrintListModel options, ListReportModel printModel, UserSelectedContext userContext,
            UserProfile userProfile, Customer customer, IListLogic _listLogic)
        {
            ListModel listModel = _listLogic.ReadList(userProfile, userContext, listId, true);
            List<ListItemModel> itemHash = listModel.Items.ToList();
            string[] itemkeys = itemHash.Select(i => i.ItemNumber).ToArray();
            ItemHistory[] itemHistories = _listLogic.GetItemsHistoryList(userContext, itemkeys);
            foreach (ListItemReportModel item in printModel.Items)
            {
                var itemInfo = itemHash.Where(i => i.ItemNumber == item.ItemNumber).FirstOrDefault();
                if ((customer != null) && (options.ShowPrices))
                {
                    StringBuilder priceInfo = new StringBuilder();
                    if (itemInfo != null)
                    {
                        if ((itemInfo.PackagePrice != null) && (itemInfo.PackagePrice.Equals("0.00") == false))
                        {
                            priceInfo.Append("$");
                            priceInfo.Append(itemInfo.PackagePrice);
                            priceInfo.Append("/Pack");
                            item.Price = priceInfo.ToString();
                            priceInfo.Append(" - ");
                        }
                        if (itemInfo.CasePrice != null)
                        {
                            priceInfo.Append("$");
                            priceInfo.Append(itemInfo.CasePrice);
                            priceInfo.Append("/Case");
                            item.Price = priceInfo.ToString();
                        }
                    }
                }
                //        // to make the option not to sort by label not reorder the items we null the label
                if ((options.Paging != null) && (options.Paging.Sort != null) && (options.Paging.Sort.Count > 0) &&
                    (options.Paging.Sort[0].Field.Equals("label", StringComparison.CurrentCultureIgnoreCase)))
                {
                    item.Label = itemInfo.Label;
                }
                else {
                    item.Label = null;
                }
                ItemHistory itemStats = itemHistories.Where(f => f.ItemNumber == item.ItemNumber).FirstOrDefault();
                if (itemStats != null)
                {
                    StringBuilder AVG8WK = new StringBuilder();
                    AVG8WK.Append(itemStats.AverageUse);
                    if (itemStats.UnitOfMeasure.Equals(KeithLink.Svc.Core.Constants.ITEMHISTORY_AVERAGEUSE_PACKAGE))
                        AVG8WK.Append(" Pack");
                    else if (itemStats.UnitOfMeasure.Equals(KeithLink.Svc.Core.Constants.ITEMHISTORY_AVERAGEUSE_CASE))
                        AVG8WK.Append(" Case");
                    if ((itemStats.AverageUse > 1) | (itemStats.AverageUse == 0))
                        AVG8WK.Append("s");
                    item.AvgUse = AVG8WK.ToString();
                }
                else {
                    item.AvgUse = "0 Cases";
                }
            }
        }
    }
}