using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class DsrLogic : IDsrLogic {
        #region attributes

        IDsrRepository _dsrRepository;
        private readonly IUnitOfWork unitOfWork;

        #endregion

        public DsrLogic(IDsrRepository dsrRepository, IUnitOfWork unitOfWork)
        {
            _dsrRepository = dsrRepository;
            this.unitOfWork = unitOfWork;
        }

        public Dsr GetDsr( string branchId, string dsrNumber ) {
            Dsr returnValue = new Dsr();

            var d = _dsrRepository.GetDsrByBranchAndDsrNumber( branchId, dsrNumber );

            if (d != null) {
                returnValue.DsrNumber = d.DsrNumber;
                returnValue.EmailAddress = d.EmailAddress;
                returnValue.Name = d.Name;
                returnValue.ImageUrl = d.ImageUrl.Inject( new { baseUrl = Configuration.MultiDocsProxyUrl } );
                returnValue.PhoneNumber = d.Phone == null ? returnDefaultDsrPhone(branchId) : d.Phone;
            } else {
                // Will be used to populate branch specific information
                returnValue.PhoneNumber = returnDefaultDsrPhone(branchId);
                returnValue.DsrNumber = "000";
                returnValue.EmailAddress = String.Concat(branchId, "@benekeith.com");
                returnValue.Name = "Ben E. Keith";
                returnValue.ImageUrl = String.Concat(Configuration.MultiDocsProxyUrl, "userimages/", branchId, "@benekeith.com");
            }

            return returnValue;
        }

        public void CreateOrUpdateDsr(Dsr dsr)
        {
            var newDsr = DsrExtensions.ToEFDsr(dsr);
            _dsrRepository.CreateOrUpdate(newDsr);
            unitOfWork.SaveChanges();
            
        }

        public void SendImageToMultiDocs( string emailAddress, Byte[] fileBytes ) {
            _dsrRepository.SendImageToMultiDocs( emailAddress, fileBytes );
        }

        #region Helper Methods
        private string returnDefaultDsrPhone(string branchId)
        {
            string phone = "";
            switch (branchId.ToLower())
            {
                case "fam":
                    phone = "8006589790";
                    break;
                case "faq":
                    phone = "8006752949";
                    break;
                case "far":
                    phone = "8007772356";
                    break;
                case "fdf":
                    phone = "8773176100";
                    break;
                case "fhs":
                    phone = "8553275500";
                    break;
                case "flr":
                    phone = "5019071518";
                    break;
                case "fok":
                    phone = "8004753484";
                    break;
                case "fsa":
                    phone = "8004888456";
                    break;
                default:
                    phone = "8177596800"; //if all else fails, call go
                    break;
            }
                
            return phone;

        }

        #endregion
    }
}
