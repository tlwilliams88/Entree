using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Nest;

namespace KeithLink.Svc.Core.Models.Brand
{
    public class BrandsReturn
    {
        #region " attributes "
        #endregion

        #region " constructor "
        public BrandsReturn()
        {
        }
        #endregion

        #region " methods / functions "
        #endregion

        #region " properties "
        [DataMember(Name = "brands")]
        [ElasticProperty(Name = "brands")]
        [JsonProperty("brands")]
        public List<Brand> Brands { get; set; }
        #endregion
    }
}
