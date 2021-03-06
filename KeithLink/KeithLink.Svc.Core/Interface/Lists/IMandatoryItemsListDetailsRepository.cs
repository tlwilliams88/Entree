﻿using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IMandatoryItemsListDetailsRepository
    {
        List<MandatoryItemsListDetail> GetAllByHeader(long parentHeaderId);

        long Save(MandatoryItemsListDetail model);

        void Delete(long id);
    }
}
