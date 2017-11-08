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

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.WebApi.Helpers
{
    /// <summary>
    /// ListPrintHelper
    /// </summary>
    public class ListPrintHelper
    {
        /// <summary>
        /// BuildReportFromList in ListPrintHelper
        /// </summary>
        /// <param name="options"></param>
        /// <param name="listId"></param>
        /// <param name="userContext"></param>
        /// <param name="userProfile"></param>
        /// <param name="_listLogic"></param>
        /// <param name="_profileLogic"></param>
        /// <param name="_elRepo"></param>
        /// <returns></returns>
        public static Stream BuildReportFromList(PrintListModel options, ListType type, long listId, UserSelectedContext userContext,
            UserProfile userProfile, IListService listService, IUserProfileLogic _profileLogic, IEventLogRepository _elRepo)
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

            ListModel list = listService.ReadList(userProfile, userContext, type, listId, options.ShowPrices);

            var NotesHash = listService.GetNotesHash(userContext);
            foreach (var item in list.Items) {
                if (NotesHash.ContainsKey(item.ItemNumber)) {
                    item.Notes = NotesHash[item.ItemNumber].Notes;
                }
            }

            if (list == null)
                return null;

            ListModel printlist = list.Clone();
            printlist.ListId = 0;

            if (options.Filter != null)
            {
                printlist.Items = printlist.Items.AsQueryable()
                                 .Filter(options.Filter, null)
                                 .ToList();
            }

            StringBuilder sortinfo = new StringBuilder();
            foreach (SortInfo si in options.Paging.Sort)
            {
                if (sortinfo.Length > 0)
                    sortinfo.Append(";");
                sortinfo.Append(si.Field);
                sortinfo.Append(",");
                sortinfo.Append(si.Order);
            }
            printlist.Items = SortOrderItems(sortinfo.ToString(), printlist.Items);
            int ind = 1;
            foreach (ListItemModel item in printlist.Items)
            {
                item.Position = ind++;
            }

            ListReportModel printModel = printlist.ToReportModel();

            Customer customer = _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);

            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;


            Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");

            string rptName = ChooseReportFromOptions(options, userContext, customer);
            Stream rdlcStream = assembly.GetManifestResourceStream(rptName);

            rv.LocalReport.LoadReportDefinition(rdlcStream);

            rv.LocalReport.SetParameters
                (MakeReportOptionsForPrintListReport(options, printModel.Name, userContext, customer));
            GatherInfoAboutItems(type, listId, options, printModel, userContext, userProfile, customer, listService);

            rv.LocalReport.DataSources.Add(new ReportDataSource("ListItems", printModel.Items));

            string deviceInfo = (options.Landscape) ? KeithLink.Svc.Core.Constants.SET_REPORT_SIZE_LANDSCAPE 
                                                    : KeithLink.Svc.Core.Constants.SET_REPORT_SIZE_PORTRAIT;
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
                if (options.Landscape) {
                    if ((options.ShowParValues) & (options.ShowPrices) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeYesParYesPriceYesNotes;
                    else if ((options.ShowParValues) & (options.ShowPrices) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeYesParYesPriceNoNotes;
                    else if ((options.ShowParValues) & (options.ShowPrices == false) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeYesParNoPriceYesNotes;
                    else if ((options.ShowParValues) & (options.ShowPrices == false) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeYesParNoPriceNoNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeNoParYesPriceYesNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeNoParYesPriceNoNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices == false) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeNoParNoPriceYesNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices == false) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_LandscapeNoParNoPriceNoNotes;
                } else {
                    if ((options.ShowParValues) & (options.ShowPrices) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitYesParYesPriceYesNotes;
                    else if ((options.ShowParValues) & (options.ShowPrices) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitYesParYesPriceNoNotes;
                    else if ((options.ShowParValues) & (options.ShowPrices == false) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitYesParNoPriceYesNotes;
                    else if ((options.ShowParValues) & (options.ShowPrices == false) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitYesParNoPriceNoNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitNoParYesPriceYesNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitNoParYesPriceNoNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices == false) & (options.ShowNotes))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitNoParNoPriceYesNotes;
                    else if ((options.ShowParValues == false) & (options.ShowPrices == false) & (options.ShowNotes == false))
                        return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_PortraitNoParNoPriceNoNotes;
                }
            }
            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST;
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

        private static void GatherInfoAboutItems(ListType type, long listId, PrintListModel options, ListReportModel printModel, UserSelectedContext userContext,
            UserProfile userProfile, Customer customer, IListService listService)
        {
            ListModel listModel = listService.ReadList(userProfile, userContext, type, listId, true);
            List<ListItemModel> itemHash = listModel.Items.ToList();
            string[] itemkeys = itemHash.Select(i => i.ItemNumber).ToArray();
            ItemHistory[] itemHistories = listService.GetItemsHistoryList(userContext, itemkeys);
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