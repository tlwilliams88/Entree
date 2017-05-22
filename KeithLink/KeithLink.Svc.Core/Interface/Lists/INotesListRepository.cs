using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface INotesListRepository
    {
        List<ListModel> GetNotesList(UserSelectedContext catalogInfo, bool headerOnly);

        void AddOrUpdateNote(string customerNumber,
            string branchId,
            string itemNumber,
            bool each,
            string catalogId,
            string note,
            bool active);
    }
}
