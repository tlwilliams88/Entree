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

            returnValue.DsrNumber = d.DsrNumber;
            returnValue.EmailAddress = d.EmailAddress;
            returnValue.Name = d.Name;
            returnValue.ImageUrl = d.ImageUrl.Inject( new { baseUrl = Configuration.MultiDocsProxyUrl } );
            returnValue.PhoneNumber = d.Phone;

            return returnValue;
        }

        public void CreateOrUpdateDsr(Dsr dsr)
        {
            var newDsr = DsrExtensions.ToEFDsr(dsr);
            _dsrRepository.CreateOrUpdate(newDsr);
            unitOfWork.SaveChanges();
            
        }


    }
}
