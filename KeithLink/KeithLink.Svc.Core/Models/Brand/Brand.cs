using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Core.Models.Brand
{
    
    public class Brand
    {
        #region " attributes "
        private StringBuilder _id;
        private StringBuilder _name;
        private StringBuilder _imageUrl;
        #endregion

        #region " constructor"
        public Brand()
        {
            _id = new StringBuilder();
            _name = new StringBuilder();
            _imageUrl = new StringBuilder();
        }
        #endregion

        #region " methods / functions "
        #endregion

        #region " properties "

        public string Id
        {
            get
            {
                return _id.ToString();
            }
            set
            {
                _id = new StringBuilder(value);
            }
        }

        public string Name
        {
            get
            {
                return _name.ToString();
            }
            set
            {
                _name = new StringBuilder(value);
            }
        }

        public string ImageURL
        {
            get
            {
                return _imageUrl.ToString();
            }
            set
            {
                _imageUrl = new StringBuilder(value);
            }
        }
        #endregion
    }
}
