using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class NotesListLogicImpl : INotesListLogic
    {
        #region attributes
        private readonly INotesListRepository _notesRepo;
        #endregion

        #region ctor
        public NotesListLogicImpl(INotesListRepository notesRepo)
        {
            _notesRepo = notesRepo;
        }
        #endregion

        #region methods
        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string,string> GetNotesDictionary(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = ReadList(user, catalogInfo, false);

            return list[0].Items.ToDictionary(i => i.ItemNumber, i => i.Notes);
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            return _notesRepo.GetNotesList(catalogInfo, false);
        }

        public void AddOrUpdateNote(UserSelectedContext catalogInfo,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                string note,
                                bool active)
        {
            _notesRepo.AddOrUpdateNote(catalogInfo.CustomerId,
                catalogInfo.BranchId,
                itemNumber,
                each,
                catalogId,
                note, 
                active);
        }

        #endregion
    }
}
