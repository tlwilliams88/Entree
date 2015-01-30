using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile {
    public class CustomerNumberComparer : EqualityComparer<Customer> {
        public override bool Equals( object obj ) {
            return base.Equals( obj );
        }

        public override bool Equals( Customer x, Customer y ) {
            return x.CustomerNumber == y.CustomerNumber;
        }

        public override int GetHashCode( Customer obj ) {
            return int.Parse( obj.CustomerNumber );
        }
    }
}