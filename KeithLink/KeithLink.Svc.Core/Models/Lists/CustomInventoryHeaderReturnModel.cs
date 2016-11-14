using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Lists {
    [Serializable]
    [DataContract(Name = "custominventory")]
    public class CustomInventoryHeaderReturnModel {

        #region attribute
        private string _listId;
        private bool _isCustomInventory;
        private List<CustomInventoryItemReturnModel> _items;
        #endregion

        #region constructor
        public CustomInventoryHeaderReturnModel() {
            _listId = "nonbeklist";
            _isCustomInventory = true;
            _items = new List<CustomInventoryItemReturnModel>();
        }
        #endregion

        #region properties
        [DataMember(Name = "listid")]
        public string ListId { get { return _listId; } set { _listId = value; } }

        /// <summary>
        /// This gets defaulted to true for all return models of this type
        /// </summary>
        [DataMember(Name = "iscustominventory")]
        public bool IsCustomInventory { get { return _isCustomInventory; } set { _isCustomInventory = value; } }

        /// <summary>
        /// List of items in the custom inventory collection
        /// </summary>
        [DataMember(Name = "items")]
        public List<CustomInventoryItemReturnModel> Items {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }
        #endregion
    }
}
