using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Nest;

namespace KeithLink.Svc.Core.Models.Brand
{
    
    [DataContract(Name="brand")]
    [ElasticType(Name="brand")]
    public class Brand
    {
        #region " attributes "
        #endregion

        #region " constructor"
       
        #endregion

        #region " methods / functions "
        #endregion

        #region " properties "

        [DataMember(Name="brand_control_label")]
        [ElasticProperty(Name="brand_control_label")]
        public string BrandControlLabel {get;set;}

        [DataMember(Name="extended_description")]
        [ElasticProperty(Name="extended_description")]
		public string ExtendedDescription { get; set; }

        [DataMember(Name="imageurl")]
        [ElasticProperty(Name="imageurl")]
		public string ImageURL { get; set; }
        #endregion
    }
}
