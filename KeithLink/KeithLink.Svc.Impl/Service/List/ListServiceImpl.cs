using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Service.List
{
    public class ListServiceImpl : IListService
    {
        #region attributes
        private readonly IListLogic _genericListLogic;
        private readonly IHistoryListLogic _historyListLogic;
        #endregion

        #region ctor
        public ListServiceImpl(IListLogic genericListLogic, IHistoryListLogic historyListLogic)
        {
            _genericListLogic = genericListLogic;
            // specific lists -
            _historyListLogic = historyListLogic;
        }
        #endregion

        public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false)
        {
            List<ListModel> list = new List<ListModel>();

            switch (type)
            {
                case ListType.Worksheet:
                {
                    list = _historyListLogic.ReadListByType(user, catalogInfo, headerOnly);
                }
                    break;
                default: // plug in legacy list implementation
                    list = _genericListLogic.ReadListByType(user, catalogInfo, type, headerOnly);
                    break;
            }

            return list;
        }
    }
}
