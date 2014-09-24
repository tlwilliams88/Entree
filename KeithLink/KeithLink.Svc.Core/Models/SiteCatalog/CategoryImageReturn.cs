using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract]
    public class CategoryImageReturn
    {
        #region ctor

        public CategoryImageReturn()
        {
            CategoryImage = new CategoryImage();
        }

        #endregion

        #region properties

        [DataMember(Name="category_image")]
        public CategoryImage CategoryImage { get; set; }

        #endregion
    }
}
