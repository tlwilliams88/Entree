using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
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

		//[HttpPost]
		//[ApiKeyedRoute("import/list")]
		//public long? List()
		//{
		//	if (Request.Content.IsMimeMultipartContent())
		//	{

		//		Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider()).ContinueWith((tsk) =>
		//		{
		//			MultipartMemoryStreamProvider prvdr = tsk.Result;

		//			foreach (HttpContent ctnt in prvdr.Contents)
		//			{
		//				// You would get hold of the inner memory stream here
		//				Stream stream = ctnt.ReadAsStreamAsync().Result;
		//				var sr = new StreamReader(stream);
		//				var myStr = sr.ReadToEnd();
		//				importLogic.ImportList(myStr);
		//				// do something witht his stream now
		//			}
		//		});
		//	}
		//	else
		//		throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
		//	return 1;
		//}

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
    }
}
