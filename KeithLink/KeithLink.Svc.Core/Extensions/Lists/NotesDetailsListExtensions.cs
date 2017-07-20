using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class NotesDetailsListExtensions {
        public static ListItemModel ToWebModel(this NotesListDetail value) {
            return new ListItemModel {
                                         ListItemId = value.Id,
                                         Type = ListType.Notes,
                                         ItemNumber = value.ItemNumber,
                                         Notes = value.Note,
                                         Each = value.Each ?? false,
                                         CatalogId = value.CatalogId,
                                         Position = value.LineNumber,
                                         ModifiedUtc = value.ModifiedUtc,
                                         CreatedUtc = value.CreatedUtc
                                     };
        }

        public static NotesListDetail ToListModel(this ListItemModel model, long headerId = 0, long detailId = 0) {
            NotesListDetail item = new NotesListDetail {
                                                           CatalogId = model.CatalogId,
                                                           Each = model.Each ?? false,
                                                           Id = (detailId != 0)? detailId : model.ListItemId,
                                                           ItemNumber = model.ItemNumber,
                                                           Note = model.Notes,
                                                           HeaderId = headerId,
                                                           LineNumber = model.Position
                                                       };

            return item;
        }
    }
}