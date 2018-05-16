using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Extensions.Customers
{
    public static class GrowthAndRecoveryModelExtensions
    {
        public static List<GrowthAndRecoveriesReturnModel> ToWebModel(this List<GrowthAndRecoveriesModel> growthAndRecoveriesModels) {
            List<GrowthAndRecoveriesReturnModel> returnValue = new List<GrowthAndRecoveriesReturnModel>();

            foreach (GrowthAndRecoveriesModel model in growthAndRecoveriesModels) {
                GrowthAndRecoveriesReturnModel returnModel = new GrowthAndRecoveriesReturnModel();

                returnModel.GroupingCode = model.GroupingCode;
                returnModel.GrowthAndReccoveryTypeKey = model.GrowthAndRecoveryTypeKey;
                returnModel.GroupingDescription = model.GroupingDescription;

                returnValue.Add(returnModel);
            }

            return returnValue;
        }
    }
}
