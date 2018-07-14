﻿using System.Collections.Generic;
using System.Linq;

using Entree.Core.Extensions;
using Entree.Core.Extensions.Lists;
using Entree.Core.Interface.Lists;
using Entree.Core.Models.Lists;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using Entree.Core.Models.Lists.ReminderItems;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class ReminderItemsListLogicImpl : IRemindersListLogic
    {
        #region attributes
        private readonly IRemindersListDetailsRepository _detailsRepo;
        private readonly IRemindersListHeadersRepository _headersRepo;
        #endregion

        #region ctor
        public ReminderItemsListLogicImpl(IRemindersListHeadersRepository headersRepo, IRemindersListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods
        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            ListModel list = null;

            ReminderItemsListHeader header = _headersRepo.GetReminderItemsHeader(catalogInfo);

            if (header == null) {
                list = null;
            } else {
                List<ReminderItemsListDetail> items = _detailsRepo.GetRemindersDetails(header.Id);

                list = header.ToListModel(items);
            }

            return list;
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly) {
            ListModel list = null;

            ReminderItemsListHeader header = _headersRepo.GetReminderItemsHeader(catalogInfo);

            if (header == null) {
                list = null;
            }
            else {
                if (headerOnly) {
                    list = header.ToListModel();
                } else {
                    List<ReminderItemsListDetail> items = _detailsRepo.GetRemindersDetails(header.Id);

                    list = header.ToListModel(items);
                }
            }
            return list;
        }

        public ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list)
        {
            ReminderItemsListHeader header = _headersRepo.GetReminderItemsHeader(catalogInfo);

            if (header == null)
            {
                if (list.Items.Count == 0) {
                    ReminderItemsListHeader model = new ReminderItemsListHeader() {
                                                                                      BranchId = catalogInfo.BranchId,
                                                                                      CustomerNumber = catalogInfo.CustomerId
                                                                                  };
                    _headersRepo.SaveReminderListHeader(model);
                }
                else {
                    foreach (var item in list.Items)
                    {
                        ReminderItemsListDetail detail = item.ToReminderItemsListDetail(0);
                        detail.Active = !item.IsDelete;
                        Save(catalogInfo, detail);
                    }
                }
            }
            else
            {
                foreach (var item in list.Items)
                {
                    ReminderItemsListDetail detail = item.ToReminderItemsListDetail(header.Id);
                    detail.Active = !item.IsDelete;
                    Save(catalogInfo, detail);
                }
            }

            return GetListModel(user, catalogInfo, 0);
        }

        public void Save(UserSelectedContext catalogInfo, ReminderItemsListDetail model) {
            // try to find the parent header id if it is not in the model
            if(model.HeaderId == 0) {
                ReminderItemsListHeader header = _headersRepo.GetReminderItemsHeader(catalogInfo);

                if(header == null) {
                    // create the header
                    model.HeaderId = _headersRepo.SaveReminderListHeader(new ReminderItemsListHeader() {
                                                                                            BranchId = catalogInfo.BranchId,
                                                                                            CustomerNumber = catalogInfo.CustomerId
                                                                                        });
                } else {
                    model.HeaderId = header.Id;
                }
            }

            _detailsRepo.SaveReminderListDetail(model);
        }
        #endregion
    }
}
