using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class DsrRepositoryImpl : EFBaseRepository<Dsr>, IDsrRepository {

        private KeithLink.Common.Impl.Logging.EventLogRepositoryImpl _log;

        public DsrRepositoryImpl( IUnitOfWork unitOfWork ) : base( unitOfWork ) {
            _log = new KeithLink.Common.Impl.Logging.EventLogRepositoryImpl( Configuration.ApplicationName );
        }

        public Dsr GetDsrByBranchAndDsrNumber( string branchId, string dsrNumber ) {
            if (branchId == null)
                throw new Exception( "Branch cannot be null" );
            if (dsrNumber == null)
                throw new Exception( "DsrNumber cannot be null" );

            return this.Entities.Where( d => d.BranchId == branchId && d.DsrNumber == dsrNumber ).FirstOrDefault();
        }


        public void SendImageToMultiDocs( string emailAddress, Byte[] fileBytes ) {
            string endpoint = String.Concat( Configuration.MultiDocsUrl, "userimages/" );

            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add( "EmailAddress", emailAddress );
            values.Add( "EncodedImageData", Convert.ToBase64String( fileBytes, Base64FormattingOptions.None ) );

            using (HttpClient http = new HttpClient()) {
                using (HttpResponseMessage response = http.PostAsJsonAsync( endpoint, values ).Result) {
                    if (!response.StatusCode.Equals( HttpStatusCode.OK ) && !response.StatusCode.Equals( HttpStatusCode.NoContent )) {
                        _log.WriteErrorLog(String.Format("Error uploadin BEK user image for user: {0} - HttpResponse: {1}", emailAddress, response.StatusCode));
                        throw new Exception("There was an error uploading the image for the BEK Employee");
                    }
                }
            }
        }

    }
}
