using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.WebApi.Repository.Profile {
    public class DsrServiceRepositoryImpl : IDsrServiceRepository {
        #region attributes
        private com.benekeith.DsrService.IDsrService _client;
        #endregion

        #region ctor
        public DsrServiceRepositoryImpl( com.benekeith.DsrService.IDsrService client ) {
            _client = client;
        }
        #endregion

        #region methods
        public Dsr GetDsr( string branchId, string dsrNumber ) {
            return _client.GetDsr( branchId, dsrNumber );
        }
        #endregion
    }
}