﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class DsrLogic : IDsrLogic {
        #region attributes

        IDsrRepository _dsrRepository;

        #endregion

        public DsrLogic( IDsrRepository dsrRepository ) {
            _dsrRepository = dsrRepository;
        }

        public Dsr GetDsr( string branchId, string dsrNumber ) {
            Dsr returnValue = new Dsr();

            var d = _dsrRepository.GetDsrByBranchAndDsrNumber( branchId, dsrNumber );

            if (d != null) {
                returnValue.DsrNumber = d.DsrNumber;
                returnValue.EmailAddress = d.EmailAddress;
                returnValue.Name = d.Name;
                returnValue.ImageUrl = d.ImageUrl.Inject( new { baseUrl = Configuration.MultiDocsProxyUrl } );
                returnValue.PhoneNumber = d.Phone;
            } else {
                // Will be used to populate branch specific information
                switch (branchId) {
                    default:
                        returnValue.PhoneNumber = "0000000000";
                        break;
                }

                returnValue.DsrNumber = "000";
                returnValue.EmailAddress = String.Concat(branchId, "@benekeith.com");
                returnValue.Name = "Ben E. Keith";
                returnValue.ImageUrl = String.Concat(Configuration.MultiDocsProxyUrl, "userimages/", branchId, "@benekeith.com");
            }

            return returnValue;
        }


    }
}
