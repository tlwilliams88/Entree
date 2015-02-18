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

            ListImportFileModel fileModel = new ListImportFileModel();

            foreach (var content in provider.Contents) {
                var file = content;
                var paramName = file.Headers.ContentDisposition.Name.Trim( '\"' );
                var buffer = await file.ReadAsByteArrayAsync();
                var stream = new MemoryStream( buffer );

                using (var s = new StreamReader( stream )) {
                    switch (paramName) {
                        case "file":
                            stream.CopyTo( fileModel.Stream );
                            fileModel.FileName = file.Headers.ContentDisposition.FileName.Trim( '\"' );
                            stream.Seek( 0, SeekOrigin.Begin );
                            fileModel.Contents = s.ReadToEnd();
                            break;
                        case "options":
                            // Figure out what to do here
                            break;
                    }
                }
            }

            if (string.IsNullOrEmpty( fileModel.Contents ))
                return new ListImportModel() { Success = false, ErrorMessage = "Invalid request" };

            return importLogic.ImportList( this.AuthenticatedUser, this.SelectedUserContext, fileModel );
		}


		[HttpPost]
		[ApiKeyedRoute("import/order")]
		public async Task<OrderImportModel> Order()
		{
			if (!Request.Content.IsMimeMultipartContent())
				throw new InvalidOperationException();

            OrderImportFileModel fileModel = new OrderImportFileModel();

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
                            stream.CopyTo( fileModel.Stream );
                            fileModel.FileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                            stream.Seek(0, SeekOrigin.Begin); // Return to the start of the stream
							fileModel.Contents = s.ReadToEnd();
							break;
						case "options":
							fileModel.Options = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderImportOptions>(s.ReadToEnd());
							break;
					}					
				}
			}

			if (string.IsNullOrEmpty(fileModel.Contents) || fileModel.Options == null)
				return new OrderImportModel() { Success = false, ErrorMessage = "Invalid request" };

			return importLogic.ImportOrder(this.AuthenticatedUser, this.SelectedUserContext, fileModel);
		}
    }
}
