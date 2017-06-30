using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.WebApi.Helpers
{
    public class ImportHelper
    {
        public static async Task<ListImportFileModel> GetFileFromContent(HttpContent httpcontent)
        {
            if (!httpcontent.IsMimeMultipartContent())
                throw new InvalidOperationException();

            var provider = new MultipartMemoryStreamProvider();
            await httpcontent.ReadAsMultipartAsync(provider);

            ListImportFileModel fileModel = new ListImportFileModel();

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
                            stream.CopyTo(fileModel.Stream);
                            fileModel.FileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                            stream.Seek(0, SeekOrigin.Begin);
                            fileModel.Contents = s.ReadToEnd();
                            break;
                        case "options":
                            // Figure out what to do here
                            fileModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ListImportFileModel>(s.ReadToEnd());
                            break;
                    }
                }
            }
            return fileModel;
        }
    }
}