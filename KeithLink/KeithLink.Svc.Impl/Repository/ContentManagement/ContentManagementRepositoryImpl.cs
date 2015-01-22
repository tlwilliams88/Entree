using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.ContentManagement.EF;
using System.Data.Entity;
using KeithLink.Svc.Core.Models.ContentManagement;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace KeithLink.Svc.Impl.Repository.Lists
{
	public class ContentManagementItemRepositoryImpl : EFBaseRepository<ContentItem>, IContentManagementItemRepository
	{
        private const string MULTIDOCS_ACTION = "Campaign/";
        private KeithLink.Common.Impl.Logging.EventLogRepositoryImpl _log;

        public ContentManagementItemRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) {
            _log = new KeithLink.Common.Impl.Logging.EventLogRepositoryImpl( Configuration.ApplicationName );
        }

        public IEnumerable<ContentItem> ReadActiveContentItemsByBranch(string branchId, int count)
        {
            return this.Entities.Where(c => c.BranchId.Equals(branchId, StringComparison.CurrentCultureIgnoreCase)
                && c.ActiveDateStart <= DateTime.UtcNow && c.ActiveDateEnd >= DateTime.UtcNow).OrderByDescending(c => c.ActiveDateEnd).Take(count).ToList();
        }

        public IEnumerable<ContentItem> ReadContentItemsByBranch(string branchId, int count)
        {
            return this.Entities.Where(c => c.BranchId.Equals(branchId, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(c => c.CreatedUtc).Take(count).ToList();
        }

        public string SaveContentImage( long contentId, string fileName, string base64file ) {
            string returnValue = null;

            string postUrl = String.Concat( Configuration.MultiDocsUrl, MULTIDOCS_ACTION );

            Dictionary<string, string> postData = new Dictionary<string, string>();

            postData.Add( "ContentId", contentId.ToString() );
            postData.Add( "FileName", fileName );
            postData.Add( "EncodedImageData", base64file );

            using (HttpClient client = new HttpClient()) {
                System.Net.Http.HttpResponseMessage response = client.PostAsJsonAsync( postUrl, postData ).Result;

                if ( response.StatusCode.Equals(System.Net.HttpStatusCode.OK) || response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent)) {
                    returnValue = String.Concat(Configuration.MultiDocsProxyUrl, MULTIDOCS_ACTION, contentId);
                } else {
                    _log.WriteErrorLog(String.Format("Error uploading content item. Filename {0}, ContentId: {1}", fileName, contentId));
                    throw new Exception("There was an error uploading this content item");
                }
            }

            return returnValue;
        }

    }
}
