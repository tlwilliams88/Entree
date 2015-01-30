using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions
{
    public static class DsrExtensions
    {
        public static Dsr ToEFDsr(this KeithLink.Svc.Core.Models.Profile.Dsr dsr)
        {
            return new KeithLink.Svc.Core.Models.EF.Dsr()
            {
                
                DsrNumber = dsr.DsrNumber
                , Name = dsr.Name
                , BranchId = dsr.Branch
                , Phone = dsr.PhoneNumber
                , EmailAddress = dsr.EmailAddress
                , ImageUrl = dsr.ImageUrl
            };
        }

        public static ListModel ToDsr(this List list, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                //TODO:  Implement this
            };
        }
    }
}
