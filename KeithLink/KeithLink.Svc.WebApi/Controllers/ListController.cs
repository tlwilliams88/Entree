using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Configuration;
using Microsoft.Reporting.WinForms;
using System.Reflection;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class ListController : BaseController {
        #region attributes
        private readonly IListServiceRepository listServiceRepository;
		private readonly IExportSettingServiceRepository exportSettingRepository;
		private readonly IEventLogRepository elRepo;
        #endregion

        #region ctor
		public ListController(IUserProfileLogic profileLogic, IListServiceRepository listServiceRepository, IExportSettingServiceRepository exportSettingRepository, IEventLogRepository elRepo)
			: base(profileLogic)
		{
            this.listServiceRepository = listServiceRepository;
			this.exportSettingRepository = exportSettingRepository;
			this.elRepo = elRepo;
        }
        #endregion

        #region methods

		[HttpPost]
		[ApiKeyedRoute("list/export/{listId}")]
		public HttpResponseMessage ExportList(long listId, ExportRequestModel exportRequest)
		{
			var list = listServiceRepository.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId);

			if (exportRequest.Fields != null)
				exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, list.Type, exportRequest.Fields, exportRequest.SelectedType);
			return ExportModel<ListItemModel>(list.Items, exportRequest);				
		}

		[HttpGet]
		[ApiKeyedRoute("list/export/{listId}")]
		public ExportOptionsModel ExportList(long listId)
		{
			return exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, listId);
		}

		[HttpGet]
		[ApiKeyedRoute("list/recommended")]
		public List<RecommendedItemModel> ReadRecommendedItemsList()
		{
			return listServiceRepository.ReadRecommendedItemsList(this.SelectedUserContext);
		}

        [HttpGet]
		[ApiKeyedRoute("list/")]
        public List<ListModel> List(bool header = false)
        {
			return listServiceRepository.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, header);
        }
		
        [HttpGet]
		[ApiKeyedRoute("list/{listId}")]
		public ListModel List(long listId)
        {
			var list = listServiceRepository.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId);

			if (list != null)
				list.ReadOnly = (!this.AuthenticatedUser.IsInternalUser && list.Type == ListType.RecommendedItems) ||
					(!this.AuthenticatedUser.IsInternalUser && list.Type == ListType.Mandatory);

			return list;

        }

        [HttpGet]
		[ApiKeyedRoute("list/labels")]
        public List<string> ListLabels()
        {
			return listServiceRepository.ReadListLabels(this.AuthenticatedUser, this.SelectedUserContext);
        }

        [HttpGet]
        [ApiKeyedRoute("list/reminders")]
        public List<ListModel> ListReminders() {
            return listServiceRepository.ReadReminders(this.AuthenticatedUser, this.SelectedUserContext);
        }

        [HttpPost]
		[ApiKeyedRoute("list/")]
		public NewListItem List(ListModel list, bool isMandatory = false, bool isRecommended = false)
        {
			return new NewListItem() { Id = listServiceRepository.CreateList(this.AuthenticatedUser.UserId, this.SelectedUserContext, list, isMandatory ? ListType.Mandatory : isRecommended ? ListType.RecommendedItems : ListType.Custom) };
        }
		
        [HttpPost]
		[ApiKeyedRoute("list/{listId}/item")]
		public NewListItem AddItem(long listId, ListItemModel newItem)
        {
			return new NewListItem() { Id = listServiceRepository.AddItem(listId, newItem) };
        }

		[HttpPost]
		[ApiKeyedRoute("list/{listId}/items")]
		public ListModel AddItems(long listId, List<ListItemModel> newItems, bool allowDuplicates = false)
		{
			return listServiceRepository.AddItems(this.AuthenticatedUser, this.SelectedUserContext, listId, newItems);
		}
		
		[HttpPost]
		[ApiKeyedRoute("list/copy")]
		public void CopyList(ListCopyShareModel copyListModel)
		{
			listServiceRepository.CopyList(copyListModel);
		}

		[HttpPost]
		[ApiKeyedRoute("list/share")]
		public void ShareList(ListCopyShareModel copyListModel)
		{
			listServiceRepository.ShareList(copyListModel);
		}

		[HttpPost]
		[ApiKeyedRoute("list/{listId}")]
		public PagedListModel pagedList(long listId, PagingModel paging)
		{
			if (!string.IsNullOrEmpty(paging.Terms))
			{
				//Build filter
				paging.Filter = new FilterInfo()
				{
					Field = "ItemNumber",
					FilterType = "contains",
					Value = paging.Terms,
					Condition = "||",
					Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = paging.Terms, FilterType = "contains" }, new FilterInfo() { Condition = "||", Field = "Name", Value = paging.Terms, FilterType = "contains" } }
				};
			}
			var stopWatch = new System.Diagnostics.Stopwatch(); //Temp: Remove
			stopWatch.Start();
			var list =  listServiceRepository.ReadPagedList(this.AuthenticatedUser, this.SelectedUserContext, listId, paging);
			stopWatch.Stop();
			elRepo.WriteInformationLog(string.Format("Total time to retrieve List {0}: {1}ms", listId, stopWatch.ElapsedMilliseconds));

			return list;
		}

		
        [HttpPut]
		[ApiKeyedRoute("list/item")]
		public void UpdateItem(ListItemModel updatedItem)
        {
			listServiceRepository.UpdateItem(updatedItem);
        }

		[HttpPut]
		[ApiKeyedRoute("list/")]
		public void Put(ListModel updatedList)
		{
			listServiceRepository.UpdateList(updatedList);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/{listId}")]
		public void DeleteList(long listId)
		{
			listServiceRepository.DeleteList(listId);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/")]
		public void DeleteList(List<long> listIds)
		{
			listServiceRepository.DeleteLists(listIds);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/item/{itemId}")]
		public void DeleteItem(long itemId)
		{
			listServiceRepository.DeleteItem(itemId);
		}

		[HttpDelete]
		[ApiKeyedRoute("list/item")]
		public void DeleteItem(List<long> itemIds)
		{
			listServiceRepository.DeleteItems(itemIds);
		}

		[HttpGet]
		[ApiKeyedRoute("list/barcode/{listId}")]
		public HttpResponseMessage Barcode(long listId)
		{
			try
			{
				var list = listServiceRepository.GetBarcodeForList(this.AuthenticatedUser, this.SelectedUserContext, listId);

				if (list == null)
					return new HttpResponseMessage() { StatusCode = HttpStatusCode.Gone };


				ReportViewer rv = new ReportViewer();

				rv.ProcessingMode = ProcessingMode.Local;
				//rv.LocalReport.ReportPath = "Report1.rdlc";

				Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
				Stream rdlcStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Reports.Itembarcode5160.rdlc");
				rv.LocalReport.LoadReportDefinition(rdlcStream);
				rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", list));

				var bytes = rv.LocalReport.Render("PDF");

				Stream stream = new MemoryStream(bytes);

				HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
				result.Content = new StreamContent(stream);
				result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");


				result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

				return result;



			}
			catch (Exception ex)
			{
				//TODO: This is test code to determine issue in dev. This should be removed.
				elRepo.WriteErrorLog("e", ex);
				if (ex.InnerException != null)
				{
					elRepo.WriteErrorLog("b", ex.InnerException);
					if (ex.InnerException.InnerException != null)
						elRepo.WriteErrorLog("c", ex.InnerException.InnerException);
				}
			}
			return null;


		}

		[HttpPost]
		[ApiKeyedRoute("list/print/{listId}")]
		public HttpResponseMessage Print(long listId, PagingModel paging)
		{
			try
			{

				if (!string.IsNullOrEmpty(paging.Terms))
				{
					//Build filter
					paging.Filter = new FilterInfo()
					{
						Field = "ItemNumber",
						FilterType = "contains",
						Value = paging.Terms,
						Condition = "||",
						Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = paging.Terms, FilterType = "contains" }, new FilterInfo() { Condition = "||", Field = "Name", Value = paging.Terms, FilterType = "contains" } }
					};
				}

				paging.Size = int.MaxValue;
				paging.From = 0;

				var list = listServiceRepository.ReadPagedList(this.AuthenticatedUser, this.SelectedUserContext, listId, paging);

				if (list == null)
					return new HttpResponseMessage() { StatusCode = HttpStatusCode.Gone };

				var printModel = list.ToReportModel();

				//TODO: Cleanup, some is test code
				ReportViewer rv = new ReportViewer();

				rv.ProcessingMode = ProcessingMode.Local;
				//rv.LocalReport.ReportPath = "Report1.rdlc";

				Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
				Stream rdlcStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Reports.ListReport.rdlc");
				rv.LocalReport.LoadReportDefinition(rdlcStream);
				ReportParameter[] parameters = new ReportParameter[1];
				parameters[0] = new ReportParameter("ListName", printModel.Name);

				rv.LocalReport.SetParameters(parameters);
				rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", printModel.Items));

				var bytes = rv.LocalReport.Render("PDF");

				Stream stream = new MemoryStream(bytes);

				HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
				result.Content = new StreamContent(stream);
				result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");


				result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

				return result;



			}
			catch (Exception ex)
			{
				//TODO: This is test code to determine issue in dev. This should be removed.
				elRepo.WriteErrorLog("e", ex);
				if (ex.InnerException != null)
				{
					elRepo.WriteErrorLog("b", ex.InnerException);
					if (ex.InnerException.InnerException != null)
						elRepo.WriteErrorLog("c", ex.InnerException.InnerException);
				}
			}
			return null;


		}
		
		
		
        #endregion
    }
}
