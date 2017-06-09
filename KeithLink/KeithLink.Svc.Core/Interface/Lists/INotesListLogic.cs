using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface INotesListLogic : IBaseListLogic
    {
        Dictionary<string, string> GetNotesDictionary(UserProfile user, UserSelectedContext catalogInfo);

        void AddOrUpdateNote(UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            string note,
            bool active);
    }
}
