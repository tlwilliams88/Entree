using KeithLink.Svc.Core.Enumerations.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Enumerations {
    public static class UnitOfMeasureExtension {
        public static string ToShortString(this UnitOfMeasure value){
            switch (value) {
                case UnitOfMeasure.Case:
                    return "C";
                case UnitOfMeasure.Package:
                    return "P";
                default:
                    return string.Empty;
            }
        }
    }
}
