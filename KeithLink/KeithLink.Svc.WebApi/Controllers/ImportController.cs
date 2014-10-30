using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class ImportController : BaseController
    {

		private readonly IImportLogic importLogic;

		public ImportController(IUserProfileLogic profileLogic, IImportLogic importLogic)
			: base(profileLogic)
		{
			this.importLogic = importLogic;
        }
		
		[HttpPost]
		[ApiKeyedRoute("import/list")]
		public async Task<ListImportModel> List()
		{
			if (!Request.Content.IsMimeMultipartContent())
				throw new InvalidOperationException();

			var provider = new MultipartMemoryStreamProvider();
			await Request.Content.ReadAsMultipartAsync(provider);

			var file = provider.Contents.First();
			var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
			var buffer = await file.ReadAsByteArrayAsync();
			var stream = new MemoryStream(buffer);

			using (var s = new StreamReader(stream))
			{
				var fileContents = s.ReadToEnd();

				return importLogic.ImportList(this.AuthenticatedUser, this.SelectedUserContext, fileContents);
			}
		}


		[HttpPost]
		[ApiKeyedRoute("import/order")]
		public async Task<OrderImportModel> Order()
		{
			if (!Request.Content.IsMimeMultipartContent())
				throw new InvalidOperationException();

			String stringFileContent = null;
			OrderImportOptions importOptions = null;

			var provider = new MultipartMemoryStreamProvider();
			await Request.Content.ReadAsMultipartAsync(provider);

			foreach (var content in provider.Contents)
			{
				var file = content;
				var paramName = file.Headers.ContentDisposition.Name.Trim('\"');
				var buffer = await file.ReadAsByteArrayAsync();
				var stream = new MemoryStream(buffer);

				using (var s = new StreamReader(stream))
				{
					switch (paramName)
					{
						case "file":
							stringFileContent = s.ReadToEnd();
							break;
						case "options":
							importOptions = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderImportOptions>(s.ReadToEnd());
							break;
					}					
				}
			}

			if (string.IsNullOrEmpty(stringFileContent) || importOptions == null)
				return new OrderImportModel() { Success = false, ErrorMessage = "Invalid request" };

			return importLogic.ImportOrder(this.AuthenticatedUser, this.SelectedUserContext, importOptions, stringFileContent);
			//}
		}
    }
}
