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
				returnValue = ToDsrModel(d);
            } else {
                returnValue = GetDefault( branchId ); 
            }

            return returnValue;
        }

        private Dsr GetDefault(string branchId) {
            return ToDsrModel(_dsrRepository.Read( d => d.DsrNumber == "000" && d.BranchId == branchId ).First());
        }

		private Dsr ToDsrModel(Core.Models.EF.Dsr d)
		{
			return new Dsr()
			{
				DsrNumber = d.DsrNumber,
				EmailAddress = d.EmailAddress,
				Name = d.Name,
				ImageUrl = d.ImageUrl.Inject(new { baseUrl = Configuration.MultiDocsProxyUrl }),
				PhoneNumber = d.Phone == null ? returnDefaultDsrPhone(d.BranchId) : d.Phone,
				Branch = d.BranchId
			};
		}

		public List<Dsr> GetAllDsrInfo()
		{
			var dsrs = _dsrRepository.ReadAll();

			return dsrs.Select(d => ToDsrModel(d)).ToList();
		}

        public void CreateOrUpdateDsr(Dsr dsr)
        {
			var existingDsr = _dsrRepository.Read(d => d.BranchId.Equals(dsr.Branch, StringComparison.CurrentCultureIgnoreCase) && d.DsrNumber.Equals(dsr.DsrNumber)).FirstOrDefault();

			if (existingDsr == null)
			{
				_dsrRepository.Create(DsrExtensions.ToEFDsr(dsr));
			}
			else
			{
				existingDsr.EmailAddress = dsr.EmailAddress;
				existingDsr.ImageUrl = dsr.ImageUrl;
				existingDsr.Name = dsr.Name;
				existingDsr.Phone = dsr.PhoneNumber;
				_dsrRepository.Update(existingDsr);
			}

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
