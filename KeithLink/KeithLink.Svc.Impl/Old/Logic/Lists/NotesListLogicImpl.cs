using System;
using System.Collections.Generic;
using System.Linq;

using Entree.Core.Extensions;
using Entree.Core.Extensions.Lists;
using Entree.Core.Interface.Lists;
using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.Notes;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class NotesListLogicImpl : INotesListLogic {
        #region attributes
        private readonly INotesHeadersListRepository _headerRepo;
        private readonly INotesDetailsListRepository _detailRepo;
        #endregion

        #region ctor
        public NotesListLogicImpl(INotesHeadersListRepository headerRepo, INotesDetailsListRepository detailRepo) {
            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
        }
        #endregion

        #region methods
        public ListItemModel GetNote(UserSelectedContext catalogInfo, string itemNumber) {
            ListItemModel returnValue = null;
            NotesListHeader header = _headerRepo.GetHeadersForCustomer(catalogInfo);

            if (header != null) {
                 returnValue = _detailRepo.Get(header.Id, itemNumber)
                                                    .ToWebModel();
            }

            return returnValue;
        }

        public List<ListItemModel> GetNotes(UserProfile user, UserSelectedContext catalogInfo)
        {
            NotesListHeader header = _headerRepo.GetHeadersForCustomer(catalogInfo);

            List<ListItemModel> notes = new List<ListItemModel>();

            if (header != null) {
                var notedetails = _detailRepo.GetAll(header.Id);
                notes = notedetails.Select(d => d.ToWebModel())
                                   .ToList();
            }

            return notes;
        }

        public ListModel GetList(UserSelectedContext catalogInfo) {
            NotesListHeader header = _headerRepo.GetHeadersForCustomer(catalogInfo);
            List<NotesListDetail> details = new List<NotesListDetail>();

            if (header != null) {
                details = _detailRepo.GetAll(header.Id);
                return header.ToListModel(details);
            }
            return null;
        }

        public long SaveNote(UserSelectedContext catalogInfo, ListItemModel detail) {
            NotesListHeader header = _headerRepo.GetHeadersForCustomer(catalogInfo);

            if (header == null) {
                // Create a new header for the customer
                header = new NotesListHeader() {
                                                   BranchId = catalogInfo.BranchId,
                                                   CustomerNumber = catalogInfo.CustomerId
                                               };

                header.Id = _headerRepo.Save(header);
            }

            long detailId = 0;
            try { detailId = _detailRepo.Get(header.Id, detail.ItemNumber).Id; } catch { } 

            return _detailRepo.Save(detail.ToListModel(header.Id, detailId));
        }

        #endregion
    }
}