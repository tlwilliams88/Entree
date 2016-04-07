using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Core.Interface.Lists;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class NoteLogicImpl : INoteLogic {
        #region attributes
        private readonly IListRepository _repo;
        #endregion

        #region ctor
        public NoteLogicImpl(IListRepository listRepository) {
            _repo = listRepository;
        }
        #endregion

        #region methods
        public List<ListItemModel> GetNotes(UserProfile user, UserSelectedContext catalogInfo) {
            var notes = _repo.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId, StringComparison.CurrentCultureIgnoreCase) &&
                                        l.BranchId.Equals(catalogInfo.BranchId) &&
                                        l.Type == ListType.Notes,
                                    i => i.Items)
                             .FirstOrDefault();

            if(notes == null) {
                return new List<ListItemModel>();
            } else {
                return notes.Items.Select(x => new ListItemModel() { ItemNumber = x.ItemNumber, Notes = x.Note }).ToList();
            }
        }
        #endregion
    }
}
